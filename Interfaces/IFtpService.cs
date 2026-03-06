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
        Task<bool> UploadLogAsync(string localPath, string remoteFileName, string remoteDir=null);
    }

    public class SftpService : IFtpService
    {
        private SftpClient client;
        private readonly string _host = "10.197.189.138";
        private readonly string _username = "root";
        private readonly string _password = "Abba@24Jun20";
        private readonly string _remoteDir = "ASUS/RS700-HIPOT";

        public async Task<bool> UploadLogAsync(string content, string fileName, string remoteDir=null)
        {
            return await Task.Run(() => {
                try
                {
                    using (client = new SftpClient(_host, _username, _password))
                    {
                        client.Connect();
                        string targetDir = remoteDir??_remoteDir;
                        client.ChangeDirectory(targetDir);

                        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                        {
                            client.UploadFile(ms, fileName);
                            Logger.Debug($"FTP upload details: {content} to {targetDir}");
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

                        Logger.Debug($"Local backup created at: {localPath}");
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
