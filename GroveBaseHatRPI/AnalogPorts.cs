/*
The MIT License(MIT)
Copyright (C) December 2018 devMobile Software

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
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace devMobile.Windows10IoTCore.GroveBaseHatRPI
{
	public class AnalogPorts : IDisposable
	{
		private const int I2CAddress = 0x04;
		private const byte RegisterDeviceId = 0x0;
		private const byte RegisterVersion = 0x02;
		private const byte RegisterPowerSupplyVoltage = 0x29;
		private const byte RegisterRawOffset = 0x10;
		private const byte RegisterVoltageOffset = 0x20;
		private const byte RegisterValueOffset = 0x30;
		private const byte DeviceId = 0x0004;
		private I2cDevice Device= null;
		private bool Disposed = false;

		public enum AnalogPort
		{
			A0 = 0,
			A1 = 1,
			A2 = 2,
			A3 = 3,
			A4 = 4,
			A5 = 5,
			A6 = 6,
			A7 = 7,
			A8 = 8
		};

		public AnalogPorts()
		{
		}


		public AnalogPorts(I2cDevice device)
		{
			if (device== null)
			{
				throw new ArgumentNullException("device");
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.Disposed)
			{
				if (disposing)
				{
					if (Device != null)
					{
						Device.Dispose();
						Device = null;
					}
				}

				this.Disposed = true;
			}
		}

		~AnalogPorts()
		{
			Dispose(false);
		}

		public void Initialise()
		{
			if (Device == null)
			{
				string aqs = I2cDevice.GetDeviceSelector();

				DeviceInformationCollection I2CBusControllers = DeviceInformation.FindAllAsync(aqs).AsTask().Result;
				if (I2CBusControllers.Count != 1)
				{
					throw new IndexOutOfRangeException("I2CBusControllers");
				}

				I2cConnectionSettings settings = new I2cConnectionSettings(I2CAddress)
				{
					BusSpeed = I2cBusSpeed.StandardMode,
					SharingMode = I2cSharingMode.Shared,
				};

				Device = I2cDevice.FromIdAsync(I2CBusControllers[0].Id, settings).AsTask().Result;
			}

			byte[] writeBuffer = new byte[1] { RegisterDeviceId };
			byte[] readBuffer = new byte[1] { 0 };

			Device.WriteRead(writeBuffer, readBuffer);

			if (readBuffer[0] != DeviceId)
			{
				throw new Exception("GroveBaseHatRPI not found");
			}
		}

		public byte Version()
		{
			byte[] writeBuffer = new byte[1] { RegisterVersion };
			byte[] readBuffer = new byte[1] { 0 };
			Debug.Assert(Device != null, "Initialise method not called");

			Device.WriteRead(writeBuffer, readBuffer);
			byte version = readBuffer[0];

			Debug.WriteLine($"GroveBaseHatRPI version {version}");

			return version;
		}

		public double PowerSupplyVoltage()
		{
			byte[] writeBuffer = new byte[1] { RegisterPowerSupplyVoltage };
			byte[] readBuffer2 = new byte[2] { 0, 0 };
			Debug.Assert(Device != null, "Initialise method not called");

			Device.WriteRead(writeBuffer, readBuffer2);
			ushort value = BitConverter.ToUInt16(readBuffer2, 0);

			Debug.WriteLine($"GroveBaseHatRPI PowerSupplyVoltage {value}");

			return value / 1000.0 ;
		}

		public ushort ReadRaw(AnalogPort analogPort)
		{
			byte register = (byte)analogPort;
			register += RegisterRawOffset;
			byte[] writeBuffer = new byte[1] { register };
			byte[] readBuffer2 = new byte[2] { 0, 0 };
			Debug.Assert(Device != null, "Initialise method not called");

			Device.WriteRead(writeBuffer, readBuffer2);
			ushort value = BitConverter.ToUInt16(readBuffer2, 0);

			Debug.WriteLine($"GroveBaseHatRPI {analogPort} ReadRaw {value}");

			return value;
		}

		public double ReadVoltage(AnalogPort analogPort)
		{
			byte register = (byte)analogPort;
			register += RegisterVoltageOffset;
			byte[] writeBuffer = new byte[1] { register };
			byte[] readBuffer2 = new byte[2] { 0, 0 };
			Debug.Assert(Device != null, "Initialise method not called");

			Device.WriteRead(writeBuffer, readBuffer2);
			ushort value = BitConverter.ToUInt16(readBuffer2, 0);

			Debug.WriteLine($"GroveBaseHatRPI {analogPort} ReadVoltage {value}");

			return value / 1000.0 ;
		}

		public double Read(AnalogPort analogPort)
		{
			byte register = (byte)analogPort;
			register += RegisterValueOffset;
			byte[] writeBuffer = new byte[1] { register } ;
			byte[] readBuffer = new byte[2] { 0, 0 };
			Debug.Assert(Device != null, "Initialise method not called");

			Device.WriteRead(writeBuffer, readBuffer);
			ushort value = BitConverter.ToUInt16(readBuffer, 0);

			Debug.WriteLine($"GroveBaseHatRPI {analogPort} Read {value}");

			return (double)value / 10.0;
		}
	}
}
