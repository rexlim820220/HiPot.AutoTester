using System;
using System.IO;
using System.Text;
using Renci.SshNet;
using System.Threading.Tasks;
using HiPot.AutoTester.Desktop.Helpers;

namespace HiPot.AutoTester.Desktop.Interfaces
{
    interface IFtpService
    {
        Task<bool> UploadLogAsync(string localPath, string remoteFileName);
    }

    public class SftpService : IFtpService
    {
        private SftpClient client;
        private readonly string _host = "10.197.189.143";
        private readonly string _username = "asrr";
        private readonly string _password = "Pega#1234";
        private readonly string _remoteDir = "asus_log/RS700";

        public async Task<bool> UploadLogAsync(string content, string fileName)
        {
            return await Task.Run(() => {
                try
                {
                    using (client = new SftpClient(_host, _username, _password))
                    {
                        client.Connect();
                        client.ChangeDirectory(_remoteDir);

                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                        {
                            client.UploadFile(ms, fileName);
                        }
                        client.Disconnect();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError("FTP upload fail, starting local backup.", ex);

                    try
                    {
                        string pureFileName = Path.GetFileNameWithoutExtension(fileName);
                        string backupDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{pureFileName}_FTP_Backups");
                        if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);

                        string localPath = Path.Combine(backupDir, fileName);
                        File.WriteAllText(localPath, content);

                        Logger.Log($"Local backup created at: {localPath}");
                    }
                    catch (Exception backupEx)
                    {
                        Logger.LogError("Critical: Local backup also failed.", backupEx);
                    }

                    return false;
                }
            });
        }

        public void Dispose()
        {
            if (client != null)
            {
                if (client.IsConnected) client.Disconnect();
                client.Dispose();
            }
        }
    }
}
