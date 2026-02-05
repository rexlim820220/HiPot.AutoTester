using System;
using HiPot.AutoTester.Desktop.Interfaces;

namespace HiPot.AutoTester.Desktop.Services
{
    public class MockHiPotService : IInstrumentCommunication
    {
        private static readonly Random _random = new Random();
        public void Connect(string portName, int baudRate) { }
        public void Disconnect() { }
        public void SendCommand(string command) { }

        public string Query(string command)
        {
            if (command.Contains("SAFE:STAT?")) return "STOPPED";
            if (command.Contains("SAFE:RES:ALL?")) return "116,116"; //$"116,{(_random.Next(2) == 0 ? "116" : "52")}";
            if (command.Contains("SAFE:SNUM?")) return "+2";
            if (command.Contains("SAFE:RES:ALL:MODE?")) return "GB, DC";
            if (command.Contains("SAFE:STEP1:GB:LEV?")) return "+3.000000E+01";
            if (command.Contains("SAFE:STEP2:DC:LEV?")) return "+2.500000E+03";
            if (command.Contains("*IDN?")) return "MOCK_INSTRUMENT,MODEL_1234,SN0001,1.0";
            if (command.Contains("SAFE:RES:ALL:MMET?"))
            {
                var rnd = new Random();
                return $"+{rnd.Next(15, 20)}E-03,+{rnd.Next(1, 5)}0E-03";
            }
            return "MOCK_OK";
        }
    }
}
