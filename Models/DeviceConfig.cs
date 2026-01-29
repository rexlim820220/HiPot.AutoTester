namespace HiPot.AutoTester.Desktop.Models
{
    class DeviceConfig
    {
        public string Name { get; set; }
        public int PsuCount { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
