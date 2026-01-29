// HiPotSerialService.cs
using System;
using System.IO.Ports;
using HiPot.AutoTester.Desktop.Interfaces;

namespace HiPot.AutoTester.Desktop.Services
{
    public class HiPotSerialService : IInstrumentCommunication
    {
        private SerialPort _port;

        public void Connect(string portName = "COM5", int baudRate = 9600)
        {
            try
            {
                if (_port != null && _port.IsOpen) return;
                _port = new SerialPort
                {
                    PortName = portName,
                    BaudRate = baudRate,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    NewLine = "\n"
                };
                _port.Open();
                _port.WriteLine("*CLS");
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}");
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
            if (_port == null || !_port.IsOpen)
            {
                throw new Exception("Serial port 未初始化或尚未開啟連線！");
            }
            try
            {
                _port.WriteLine(command);
            }
            catch (Exception ex)
            {
                throw new Exception($"發送指令失敗: {ex.Message}");
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