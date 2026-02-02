using System;
using System.Threading.Tasks;
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
        Task<SfisResult> LoginAsync(int status);
        Task<SfisResult> UploadResultAsync(string isn, string data);
        SfisResult UploadResult(string isn, string data);
        Task<SfisResult> CheckRouteAsync(string isn);
    }

    public class SfisService: ISfisService
    {
        // ------------- Web Service --------------------
        private bool _isLoggedIn = false;
        private readonly Sfis_Upload_Para _parameters;
        private readonly SFISTSPWebService _soapClient;

        public SfisService(Sfis_Upload_Para parameters = null)
        {
            _parameters = parameters ?? new Sfis_Upload_Para();
            //Web Services
            _soapClient = new SFISTSPWebService();
            _soapClient.Url = "http://pty-sfwspd-n1.sfis.pegatroncorp.com/sfiswebservice/sfistspwebservice.asmx";
            _soapClient.UseDefaultCredentials = true;
            _soapClient.Timeout = 30000;
        }

        #region ----- LOGIN 登入 -----
        public async Task<SfisResult> LoginAsync(int _status)
        {
            if (_isLoggedIn)
                return SfisResult.Success("Already logged in");
            try
            {
                string response = await Task.Run(() => _soapClient.WTSP_LOGINOUT(
                    programId: _parameters.ProgramId,
                    programPassword: _parameters.ProgramPassword,
                    op: "LA0800494",
                    password: "LA0800494",
                    device: _parameters.Device,
                    TSP: _parameters.TSP,
                    status: _status
                ));

                bool success = response.TrimStart().StartsWith("1");
                if (success) _isLoggedIn = true;

                return success
                    ? SfisResult.Success(response)
                    : SfisResult.Failure(response, "SFIS Login failed");
            }
            catch (Exception ex)
            {
                return SfisResult.Failure("", $"Login Exception: {ex.Message}");
            }
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
    }
}
