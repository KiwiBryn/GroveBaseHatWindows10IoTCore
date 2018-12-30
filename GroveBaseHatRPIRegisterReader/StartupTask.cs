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
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.I2c;
using Windows.Devices.Enumeration;


namespace devMobile.Windows10IoTCore.GroveBaseHatRPIRegisterReader
{
	public sealed class StartupTask : IBackgroundTask
	{
		private BackgroundTaskDeferral backgroundTaskDeferral = null;

		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			string aqs = I2cDevice.GetDeviceSelector();
			DeviceInformationCollection I2CBusControllers = await DeviceInformation.FindAllAsync(aqs);

			if (I2CBusControllers.Count != 1)
			{
				Debug.WriteLine("Unexpect number of I2C bus controllers found");
				return;
			}

			I2cConnectionSettings settings = new I2cConnectionSettings(0x04)
			{
				BusSpeed = I2cBusSpeed.StandardMode,
				SharingMode = I2cSharingMode.Shared,
			};

			using (I2cDevice device = I2cDevice.FromIdAsync(I2CBusControllers[0].Id, settings).AsTask().GetAwaiter().GetResult())
			{
				try
				{
					ushort value = 0;
					// From the Seeedstudio python
					// 0x10 ~ 0x17: ADC raw data
					// 0x20 ~ 0x27: input voltage
					// 0x29: output voltage (Grove power supply voltage)
					// 0x30 ~ 0x37: input voltage / output voltage						
					do
					{
						for (byte register = 0; register < 0x10; register++)
						{
							byte[] writeBuffer = new byte[1] { register };
							byte[] readBuffer1 = new byte[1] { 0 };
							byte[] readBuffer2 = new byte[2] { 0,0 };

							device.WriteRead(writeBuffer, readBuffer1);
							device.WriteRead(writeBuffer, readBuffer2);
							value = BitConverter.ToUInt16(readBuffer2, 0);

							Debug.Write($"{register},{readBuffer1[0]},{value} ");
							Task.Delay(1000).GetAwaiter().GetResult();
						}
						Debug.WriteLine("");
					}
					while (true);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			}
		}
	}
}
