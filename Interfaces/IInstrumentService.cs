namespace HiPot.AutoTester.Desktop.Interfaces
{
    public interface IInstrumentCommunication
    {
        bool IsConnected { get; }
        void Connect(string portName, int baudRate);
        void Disconnect();
        void SendCommand(string command);
        string Query(string command);
    }
}