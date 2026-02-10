// HiPotSerialService.cs
using System;
using System.IO;
using System.IO.Ports;
using HiPot.AutoTester.Desktop.Interfaces;

namespace HiPot.AutoTester.Desktop.Services
{
    public class HiPotSerialService : IInstrumentCommunication
    {
        private SerialPort _port;

        private void TryOpenPort(string name, int baud)
        {
            try
            {
                if (_port != null)
                {
                    if (_port.IsOpen) _port.Close();
                    _port.Dispose();
                    _port = null;
                }

                _port = new SerialPort(name, baud, Parity.None, 8, StopBits.One)
                {
                    NewLine = "\n",
                    ReadTimeout = 2000,
                    WriteTimeout = 1000,
                    DtrEnable = true,
                    RtsEnable = true
                };

                _port.Open();

                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception($"Port {name} is already occupied by another program.");
            }
            catch (IOException ex)
            {
                throw new Exception($"Unable to open port {name}, please check hardware connections. Content: {ex.Message}");
            }
        }

        public void Connect(string portName = null, int baudRate = 9600)
        {
            string[] availablePorts = SerialPort.GetPortNames();

            if (availablePorts.Length == 0)
            {
                throw new Exception("No COM Port detected by the computer.");
            }

            if (!string.IsNullOrEmpty(portName))
            {
                TryOpenPort(portName, baudRate);
                return;
            }

            bool foundDevice = false;
            foreach (string p in availablePorts)
            {
                try
                {
                    TryOpenPort(p, baudRate);
                    _port.ReadTimeout = 1500;
                    _port.DiscardInBuffer();
                    _port.WriteLine("*IDN?");

                    string idn = _port.ReadLine();
                    if (idn.ToUpper().Contains("CHROMA"))
                    {
                        foundDevice = true;
                        _port.ReadTimeout = 5000;
                        break;
                    }
                }
                catch {}
                finally
                {
                    if (!foundDevice && _port != null && _port.IsOpen)
                        _port.Close();
                }
            }

            if (!foundDevice)
            {
                throw new Exception("Scan complete, device 19032-P not found (please ensure power is on).");
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