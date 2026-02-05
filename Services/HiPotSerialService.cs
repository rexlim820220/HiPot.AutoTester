// HiPotSerialService.cs
using System;
using System.IO.Ports;
using HiPot.AutoTester.Desktop.Interfaces;

namespace HiPot.AutoTester.Desktop.Services
{
    public class HiPotSerialService : IInstrumentCommunication
    {
        private SerialPort _port;

        private void TryOpenPort(string name, int baud)
        {
            if (_port != null && _port.IsOpen) _port.Close();
            _port = new SerialPort(name, baud, Parity.None, 8, StopBits.One)
            {
                NewLine = "\n",
                ReadTimeout = 2000,
                WriteTimeout = 1000
            };
            _port.Open();
        }

        public void Connect(string portName = null, int baudRate = 9600)
        {
            if (!string.IsNullOrEmpty(portName))
            {
                TryOpenPort(portName, baudRate);
                return;
            }

            string[] availablePorts = SerialPort.GetPortNames();
            bool foundDevice = false;

            foreach(string p in availablePorts)
            {
                try
                {
                    TryOpenPort(p, baudRate);
                    _port.ReadTimeout = 1000;
                    _port.WriteLine("*IDN?");
                    string idn = _port.ReadLine();

                    if (idn.Contains("Chroma"))
                    {
                        foundDevice = true;
                        _port.ReadTimeout = 5000;
                        break;
                    }
                    else
                    {
                        _port.Close();
                    }
                }
                catch
                {
                    if (_port != null && _port.IsOpen) _port.Close();
                }
            }

            if (!foundDevice)
            {
                throw new Exception("19032-P was not found in any of the COM ports");
            }
        }
        public void Disconnect()
        {
            if (_port != null && _port.IsOpen)
            {
                _port.WriteLine("SYST:LOC");
                _port.Close();
                _port.Dispose();
                _port = null;
            }
        }

        public void SendCommand(string command)
        {
            if (_port == null) throw new Exception("Serial port object is null. Did you call Connect()?");
            if (!_port.IsOpen) throw new Exception("Serial port is closed.");
            try
            {
                _port.WriteLine(command);
            }
            catch (Exception ex)
            {
                throw new Exception($"Command sent failed: {ex.Message}");
            }
        }

        public string Query(string command)
        {
            if (_port == null && _port.IsOpen)
            {
                throw new InvalidOperationException("Serial port is not connected!");
            }

            _port.DiscardInBuffer();
            _port.WriteLine(command);

            try
            {
                return _port.ReadLine();
            }
            catch (TimeoutException)
            {
                return "TIMEOUT";
            }
        }
    }
}