/*
The MIT License(MIT)
Copyright (C) 2018 devMobile Software

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;


namespace devMobile.Windows10IoTCore.I2CDevicePinger
{
	public sealed class StartupTask : IBackgroundTask
	{
		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			string aqs = I2cDevice.GetDeviceSelector();
			DeviceInformationCollection I2CBusControllers = await DeviceInformation.FindAllAsync(aqs);

			if (I2CBusControllers.Count < 1)
			{
				Debug.WriteLine("No I2C bus controllers found");
				return;
			}

			// Address to be checked
			//I2cConnectionSettings settings = new I2cConnectionSettings(0x04) // PI Grove base hat
			I2cConnectionSettings settings = new I2cConnectionSettings(0x53) // ADXL345
			{
				BusSpeed = I2cBusSpeed.StandardMode,
			   SharingMode = I2cSharingMode.Shared,
		   };

			foreach (var I2CBusContoller in I2CBusControllers)
			{
				using (I2cDevice device = I2cDevice.FromIdAsync(I2CBusContoller.Id, settings).AsTask().GetAwaiter().GetResult())
				{
					try
					{
						// Check value in register 0x0 
						// Grove Base Hat for RPI from python 
						//		RPI_HAT_PID = 0x0004
						//		RPI_ZERO_HAT_PID = 0x0005
						//
						// ADXL345 from datasheet 
						//		DEVID 0xE5
						//
						byte[] writeBuffer = new byte[1] { 0 };
						byte[] readBuffer = new byte[1] { 0 };
						device.WriteRead(writeBuffer, readBuffer);

						Debug.WriteLine($"DeviceID 0X{readBuffer[0]:X}");
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.Message);
					}
				}
			}
		}
	}
}
