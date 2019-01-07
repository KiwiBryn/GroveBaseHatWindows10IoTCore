﻿/*
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
using Windows.ApplicationModel.Background;
using Windows.System.Threading;

using devMobile.Windows10IoTCore.GroveBaseHatRPI;


namespace devMobile.Windows10IoTCore.GroveBaseHatRPIClient
{
	public sealed class StartupTask : IBackgroundTask
	{
		private ThreadPoolTimer timer;
		private BackgroundTaskDeferral deferral;
		AnalogPorts analogPorts = new AnalogPorts();

		public void Run(IBackgroundTaskInstance taskInstance)
		{
			deferral = taskInstance.GetDeferral();

			analogPorts.Initialise();

			byte version = analogPorts.Version();
			Debug.WriteLine($"Version {version}");

			double powerSupplyVoltage = analogPorts.PowerSupplyVoltage();
			Debug.WriteLine($"Power supply voltage {powerSupplyVoltage}v");

			//timer = ThreadPoolTimer.CreatePeriodicTimer(AnalogPortsRaw, TimeSpan.FromSeconds(5));
			//timer = ThreadPoolTimer.CreatePeriodicTimer(AnalogPortsVoltage, TimeSpan.FromSeconds(5));
			//timer = ThreadPoolTimer.CreatePeriodicTimer(AnalogPortsRead, TimeSpan.FromSeconds(5));
		}

		void AnalogPortsRaw(ThreadPoolTimer timer)
		{
			try
			{
				ushort value;

				value = analogPorts.ReadRaw(AnalogPorts.AnalogPort.A0);
				Debug.WriteLine($"A0 Raw {value}");
				value = analogPorts.ReadRaw(AnalogPorts.AnalogPort.A1);
				Debug.WriteLine($"A1 Raw {value}");
				value = analogPorts.ReadRaw(AnalogPorts.AnalogPort.A2);
				Debug.WriteLine($"A2 Raw {value}");
				value = analogPorts.ReadRaw(AnalogPorts.AnalogPort.A3);
				Debug.WriteLine($"A3 Raw {value}");
				value = analogPorts.ReadRaw(AnalogPorts.AnalogPort.A4);
				Debug.WriteLine($"A4 Raw {value}");
				value = analogPorts.ReadRaw(AnalogPorts.AnalogPort.A5);
				Debug.WriteLine($"A5 Raw {value}");
				value = analogPorts.ReadRaw(AnalogPorts.AnalogPort.A6);
				Debug.WriteLine($"A6 Raw {value}");
				value = analogPorts.ReadRaw(AnalogPorts.AnalogPort.A7);
				Debug.WriteLine($"A7 Raw {value}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"AnalogPorts.ReadRaw failed {ex.Message}");
			}
		}

		void AnalogPortsVoltage(ThreadPoolTimer timer)
		{
			try
			{
				double value;

				value = analogPorts.ReadVoltage(AnalogPorts.AnalogPort.A0);
				Debug.WriteLine($"A0 {value}v");
				value = analogPorts.ReadVoltage(AnalogPorts.AnalogPort.A1);
				Debug.WriteLine($"A1 {value}v");
				value = analogPorts.ReadVoltage(AnalogPorts.AnalogPort.A2);
				Debug.WriteLine($"A2 {value}v");
				value = analogPorts.ReadVoltage(AnalogPorts.AnalogPort.A3);
				Debug.WriteLine($"A3 {value}v");
				value = analogPorts.ReadVoltage(AnalogPorts.AnalogPort.A4);
				Debug.WriteLine($"A4 {value}v");
				value = analogPorts.ReadVoltage(AnalogPorts.AnalogPort.A5);
				Debug.WriteLine($"A5 {value}v");
				value = analogPorts.ReadVoltage(AnalogPorts.AnalogPort.A6);
				Debug.WriteLine($"A6 {value}v");
				value = analogPorts.ReadVoltage(AnalogPorts.AnalogPort.A7);
				Debug.WriteLine($"A7 {value}v");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"AnalogPorts.ReadVoltage failed {ex.Message}");
			}
		}

		void AnalogPortsRead(ThreadPoolTimer timer)
		{
			try
			{
				double value;

				value = analogPorts.Read(AnalogPorts.AnalogPort.A0);
				Debug.WriteLine($"A0 {value}");
				value = analogPorts.Read(AnalogPorts.AnalogPort.A1);
				Debug.WriteLine($"A1 {value}");
				value = analogPorts.Read(AnalogPorts.AnalogPort.A2);
				Debug.WriteLine($"A2 {value}");
				value = analogPorts.Read(AnalogPorts.AnalogPort.A3);
				Debug.WriteLine($"A3 {value}");
				value = analogPorts.Read(AnalogPorts.AnalogPort.A4);
				Debug.WriteLine($"A4 {value}");
				value = analogPorts.Read(AnalogPorts.AnalogPort.A5);
				Debug.WriteLine($"A5 {value}");
				value = analogPorts.Read(AnalogPorts.AnalogPort.A6);
				Debug.WriteLine($"A6 {value}");
				value = analogPorts.Read(AnalogPorts.AnalogPort.A7);
				Debug.WriteLine($"A7 {value}");
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"AnalogPorts.Read failed {ex.Message}");
			}
		}
	}
}
