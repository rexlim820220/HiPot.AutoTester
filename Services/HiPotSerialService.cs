// HiPotSerialService.cs
using System.IO.Ports;

namespace HiPot.AutoTester.Desktop.Services
{
    public class HiPotSerialService : IInstrumentCommunication
    {
        private SerialPort _port;

        public void Connect() { /* 初始化並 Open Port */ }
        public void Disconnect() { /* Close Port */ }
        public void SendCommand(string command) => _port.WriteLine(command);
        public string Query(string command)
        {
            _port.WriteLine(command);
            return _port.ReadLine();
        }
    }
}