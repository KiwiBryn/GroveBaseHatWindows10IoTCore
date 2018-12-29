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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;


namespace devMobile.Windows10IoTCore.I2CDeviceScanner
{
	public sealed class StartupTask : IBackgroundTask
	{
		const byte I2CAddressMinimim = 0x01;
		const byte I2CAddressMaximum = 0x7F;

		public async void Run(IBackgroundTaskInstance taskInstance)
		{
			string aqs = I2cDevice.GetDeviceSelector();
			DeviceInformationCollection I2CBusControllers = await DeviceInformation.FindAllAsync(aqs);

			if (I2CBusControllers.Count < 1)
			{
				Debug.WriteLine("No I2C bus controllers found");
				return;
			}

			I2cConnectionSettings settings = new I2cConnectionSettings(I2CAddressMinimim)
			{
				SharingMode = I2cSharingMode.Shared,
				BusSpeed = I2cBusSpeed.StandardMode
			};

			foreach (var I2CBusContoller in I2CBusControllers)
			{
				IList<int> validAddresses = new List<int>();

				for (byte slaveAddress = I2CAddressMinimim; slaveAddress <= I2CAddressMaximum; slaveAddress++)
				{
					settings.SlaveAddress = slaveAddress;

					using (I2cDevice device = I2cDevice.FromIdAsync(I2CBusContoller.Id, settings).AsTask().GetAwaiter().GetResult())
					{
						if (device != null)
						{
							try
							{
								// See if there is a device at the address
								byte[] writeBuffer = new byte[1] { 0 };
								device.Write(writeBuffer);

								validAddresses.Add(settings.SlaveAddress);
							}
							catch
							{
							}
						}
					}
				}

				Debug.WriteLine("");
				Debug.WriteLine($"I2C Controller {I2CBusContoller.Id} has {validAddresses.Count()} devices");
				foreach (byte deviceAddress in validAddresses)
				{
					Debug.WriteLine($" Address 0x{deviceAddress:X}");
				}
			}
		}
	}
}
