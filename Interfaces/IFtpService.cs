using System.IO;
using System.Text;
using Renci.SshNet;
using System.Threading.Tasks;

namespace HiPot.AutoTester.Desktop.Interfaces
{
    interface IFtpService
    {
        Task UploadLogAsync(string localPath, string remoteFileName);
    }

    public class SftpService : IFtpService
    {
        private SftpClient client;
        private readonly string _host = "10.197.189.143";
        private readonly string _username = "asrr";
        private readonly string _password = "Pega#1234";
        private readonly string _remoteDir = "asus_log";

        public async Task UploadLogAsync(string content, string fileName)
        {
            await Task.Run(() => {
                using (client = new SftpClient(_host, _username, _password))
                {
                    client.Connect();
                    client.ChangeDirectory(_remoteDir);

                    using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(content)))
                    {
                        client.UploadFile(ms, fileName);
                    }
                    client.Disconnect();
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
