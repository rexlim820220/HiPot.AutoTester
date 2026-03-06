using System;
using System.Threading;
using System.Threading.Tasks;
using HiPot.AutoTester.Desktop.Helpers;
using HiPot.AutoTester.Desktop.sfistspwebservice;

namespace HiPot.AutoTester.Desktop.Services
{
    public class SfisResult
    {
        public bool IsSuccess { get; }
        public string Response { get; }
        public string ErrorMessage { get; }

        public SfisResult(bool _isSuccess, string _response, string _errorMessage = null)
        {
            IsSuccess = _isSuccess;
            Response = _response;
            ErrorMessage = _errorMessage;
        }

        public static SfisResult Success(string response) => new SfisResult(true, response);
        public static SfisResult Failure(string response, string errorMessage) => new SfisResult(false, response, errorMessage);
    }

    public interface ISfisService
    {
        bool IsLoggedIn { get; }
        bool IsConnecting { get; }
        Task<SfisResult> LoginAsync(int status);
        Task<SfisResult> UploadResultAsync(string isn, string data);
        SfisResult UploadResult(string isn, string data);
        Task<SfisResult> CheckRouteAsync(string isn);
    }

    public class SfisService: ISfisService
    {
        // ------------- Web Service --------------------
#if DEBUG
        private bool _isLoggedIn = true;
#else
        private bool _isLoggedIn = false;
#endif
        private bool _isConnecting = false;
        private readonly Sfis_Upload_Para _parameters;
        private readonly SFISTSPWebService _soapClient;
        private readonly SemaphoreSlim _loginLock = new SemaphoreSlim(1, 1);

        public SfisService(Sfis_Upload_Para parameters = null)
        {
            _parameters = parameters ?? new Sfis_Upload_Para();
            //Web Services
            _soapClient = new SFISTSPWebService();
            _soapClient.Url = "http://pty-sfwspd-n1.sfis.pegatroncorp.com/sfiswebservice/sfistspwebservice.asmx";
            _soapClient.UseDefaultCredentials = true;
            _soapClient.Timeout = 1000;
        }

        #region ----- LOGIN 登入 -----
        public async Task<SfisResult> LoginAsync(int _status)
        {
#if !DEBUG
            await _loginLock.WaitAsync();
            try
            {
                if (_isLoggedIn)
                    return SfisResult.Success("Already logged in");

                _isConnecting = true;
                await _loginLock.WaitAsync();

                _soapClient.Timeout = 5000;
                string response = await Task.Run(() =>
                {
                    try
                    {
                        return _soapClient.WTSP_LOGINOUT(
                            programId: _parameters.ProgramId,
                            programPassword: _parameters.ProgramPassword,
                            op: _parameters.UserID,
                            password: _parameters.UserPassword,
                            device: _parameters.Device,
                            TSP: _parameters.TSP,
                            status: _status
                        );
                    }
                    catch (Exception innerEx)
                    {
                        throw new InvalidOperationException(
                            $"SFIS SOAP 呼叫失敗 (device: {_parameters.Device})", innerEx);
                    }
                });

                Logger.Debug($"SFIS Login回應: [{response}]");

                bool success = response?.TrimStart().StartsWith("1") == true;

                if (success)
                {
                    _isLoggedIn = true;
                    return SfisResult.Success(response.Trim());
                }
                else
                {
                    string errorDetail = response?.Trim() ?? "(無回應)";
                    return SfisResult.Failure(response, $"SFIS 登入失敗 - 伺服器回應: {errorDetail}");
                }
            }
            catch (System.Net.WebException ex)
            {
                Logger.LogError($"SFIS 網路例外", ex);
                return SfisResult.Failure("", $"網路連線失敗: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"SFIS 其他例外", ex);
                return SfisResult.Failure("", $"登入發生例外: {ex.Message}");
            }
            finally
            {
                _isConnecting = false;
                _loginLock.Release();
            }
#else
            _isLoggedIn = true;
            return SfisResult.Success("Debug mode: simulated login");
#endif
        }
        #endregion

        #region ----- CHKROUTE 檢查路由 -----
        public async Task<SfisResult> CheckRouteAsync(string isn)
        {
            if (!_isLoggedIn)
            {
                var loginResult = await LoginAsync(1);
                if (!loginResult.IsSuccess)
                    return SfisResult.Failure(loginResult.Response, "Error: Cannot call CHKROUTE when logged out");
            }
            try
            {
                string response = await Task.Run(() => _soapClient.WTSP_CHKROUTE(
                    programId: _parameters.ProgramId,
                    programPassword: _parameters.ProgramPassword,
                    ISN: isn,
                    device: _parameters.Device,
                    checkFlag: _parameters.CPKFlag, // IMEI;MAC1;MAC2
                    checkData: "12345;A00001;A00002",
                    type: 1
                )).ConfigureAwait(false);

                Logger.Debug($"SFIS CheckRoute回應: [{response}]");

                bool isSuccess = response.StartsWith("1");
                return isSuccess
                    ? SfisResult.Success(response)
                    : SfisResult.Failure(response, "Route validation error");
            }
            catch (Exception ex)
            {
                return SfisResult.Failure("", $"CHKROUTE Exception: {ex.Message}");
            }
        }
        #endregion

        #region ----- 上傳主方法 -----
        public async Task<SfisResult> UploadResultAsync(string isn, string data)
        {
            try
            {
                string response = await Task.Run(() => _soapClient.WTSP_RESULT(
                    programId: _parameters.ProgramId,
                    programPassword: _parameters.ProgramPassword,
                    ISN: isn,
                    error: _parameters.Error,
                    device: _parameters.Device,
                    TSP: _parameters.TSP,
                    data: data,
                    status: _parameters.Status,
                    CPKFlag: _parameters.CPKFlag
                )).ConfigureAwait(false);

                Logger.Debug($"SFIS Upload Result回應: [{response}]");

                bool isSuccess = !string.IsNullOrEmpty(response) && response.StartsWith("1");
                return isSuccess
                    ? SfisResult.Success(response)
                    : SfisResult.Failure(response, "Response does not contain '1' and 'SUCCESSFUL'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[WebService] Upload failed: {ex.Message}");
                return SfisResult.Failure("", $"Web and TCP upload failed: {ex.Message}");
            }
        }

        public SfisResult UploadResult(string isn, string data)
            => UploadResultAsync(isn, data).GetAwaiter().GetResult();
        #endregion

        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
        }
        public bool IsConnecting
        {
            get { return _isConnecting; }
        }
    }
}
