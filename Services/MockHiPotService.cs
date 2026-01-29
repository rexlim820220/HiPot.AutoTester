using System;
using HiPot.AutoTester.Desktop.Interfaces;

namespace HiPot.AutoTester.Desktop.Services
{
    public class MockHiPotService : IInstrumentCommunication
    {
        public void Connect(string portName, int baudRate) { }
        public void Disconnect() { }
        public void SendCommand(string command) { }

        public string Query(string command)
        {
            if (command.Contains("SAFE:STAT?")) return "STOPPED";
            if (command.Contains("SAFE:RES:ALL?")) return "116";
            if (command.Contains("SAFE:SNUM?")) return "2";
            if (command.Contains("SAFE:FETH?"))
            {
                var rnd = new Random();
                return $"1,GB,+{rnd.Next(15, 20)}0000E-03,2,AC,+{rnd.Next(15, 20)}0000E-03";
            }
            return "MOCK_OK";
        }
    }
}
