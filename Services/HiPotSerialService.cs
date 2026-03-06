using System;
using System.IO.Ports;
using HiPot.AutoTester.Desktop.Helpers;
using HiPot.AutoTester.Desktop.Interfaces;

public class HiPotSerialService : IInstrumentCommunication, IDisposable
{
    private SerialPort _port;
    public bool IsConnected => _port != null && _port.IsOpen;

    private void TryOpenPort(string name, int baud)
    {
        SafeClosePort();

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

    public void Connect(string portName = null, int baudRate = 9600)
    {
        if (IsConnected) return;

        string[] availablePorts = SerialPort.GetPortNames();

        if (availablePorts.Length == 0)
            throw new Exception("No COM Port detected.");

        if (!string.IsNullOrEmpty(portName))
        {
            TryOpenPort(portName, baudRate);
            return;
        }

        foreach (string p in availablePorts)
        {
            try
            {
                TryOpenPort(p, baudRate);
                _port.WriteLine("*CLS");
                System.Threading.Thread.Sleep(50);
                _port.WriteLine("*IDN?");
                string idn = _port.ReadLine();

                if (!string.IsNullOrEmpty(idn) && idn.ToUpper().Contains("CHROMA"))
                {
                    _port.DiscardInBuffer();
                    return;
                }
            }
            catch (TimeoutException)
            {
                Logger.Debug($"Port {p} response timeout, skipping...");
                SafeClosePort();
            }
            catch (Exception ex)
            {
                Logger.Debug($"Port {p} failed: {ex.Message}");
                SafeClosePort();
            }
        }

        throw new Exception("Device not found.");
    }

    public void Disconnect()
    {
        SafeClosePort();
    }

    private void SafeClosePort()
    {
        try
        {
            if (_port != null)
            {
                if (_port.IsOpen)
                {
                    _port.DiscardInBuffer();
                    _port.DiscardOutBuffer();
                    _port.Close();
                }

                _port.Dispose();
                _port = null;
            }
        }
        catch { }
    }

    public void SendCommand(string command)
    {
        EnsureConnected();

        try
        {
            _port.WriteLine(command);
        }
        catch (Exception ex)
        {
            SafeClosePort();
            throw new Exception($"Command sent failed: {ex.Message}");
        }
    }

    public string Query(string command)
    {
        EnsureConnected();

        try
        {
            _port.DiscardInBuffer();
            _port.WriteLine(command);
            return _port.ReadLine();
        }
        catch (TimeoutException)
        {
            return "TIMEOUT";
        }
        catch (Exception ex)
        {
            SafeClosePort();
            throw new Exception($"Query failed: {ex.Message}");
        }
    }

    private void EnsureConnected()
    {
        if (_port == null || !_port.IsOpen)
            throw new InvalidOperationException("Serial port is not connected.");
    }

    public void Dispose()
    {
        SafeClosePort();
    }
}
