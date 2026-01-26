public interface IInstrumentCommunication
{
    void Connect();
    void Disconnect();
    void SendCommand(string command);
    string Query(string command);
}