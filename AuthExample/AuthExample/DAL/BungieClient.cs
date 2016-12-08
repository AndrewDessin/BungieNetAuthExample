using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;
using AuthExample.Annotations;
using AuthExample.Domain;

namespace AuthExample.DAL
{
    public class BungieClient : INotifyPropertyChanged
    {
        public const string BungieBaseUri = "https://www.bungie.net/";
        public const string AccessTokenRequest = "Platform/App/GetAccessTokensFromCode/";
        public const string RefreshTokenRequest = "Platform/App/GetAccessTokensFromRefreshToken/";
        public const string AuthenticationCodeRequest = "https://www.bungie.net/en/Application/Authorize/......."; //todo: Replace with your own Auth url.
        private const int Success = 1;
        private const string ApiKey = "3c9......"; //todo: Replace with your own ApiKey.
        public static BungieClient Instance = new BungieClient();

        private string _authCode;

        public BungieClient()
        {
            PrepareBungieRequests();
        }

        public AccessToken AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }

        public string AuthCode
        {
            get { return _authCode; }
            set
            {
                if (value == _authCode) return;
                _authCode = value;
                OnPropertyChanged();
            }
        }

        public HttpClient Client { get; set; }
        public HttpBaseProtocolFilter BaseFilter { get; set; } = new HttpBaseProtocolFilter();

        public event PropertyChangedEventHandler PropertyChanged;

        public void PrepareBungieRequests()
        {
            Client = new HttpClient(BaseFilter);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            Client.DefaultRequestHeaders.Add("X-API-Key", ApiKey);
            BaseFilter.AutomaticDecompression = true;
        }

        /// <summary>
        ///     Makes an HTTP GET request to the specified endpoint
        /// </summary>
        /// <typeparam name="T">Type of object expected</typeparam>
        /// <param name="endpoint">Endpoint which is being accessed</param>
        public async Task<T> RunGetAsync<T>(string endpoint)
        {
            return await RunRequestAsync<T>(endpoint, RequestTypes.Get);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">Type of object expected</typeparam>
        /// <param name="requestObject">Object to be serialized and sent in the POST request</param>
        /// <param name="endpoint">Endpoint which is being accessed</param>
        public async Task<T> RunPostAsync<T>(object requestObject, string endpoint)
        {
            return await RunRequestAsync<T>(endpoint, RequestTypes.Post, requestObject);
        }

        /// <summary>
        ///     Makes an HTTP Web Request to the specified Bungie.net endpoint.
        /// </summary>
        /// <returns>
        ///     Returns deserialized object of specified type T.
        /// </returns>
        private async Task<T> RunRequestAsync<T>(string endpoint, RequestTypes requestType, object postObject = null)
        {
            try
            {
                HttpResponseMessage response = null;
                Client.DefaultRequestHeaders.Authorization = AccessToken?.Value != null ? new HttpCredentialsHeaderValue("Bearer", AccessToken.Value) : null;
                //post
                if (requestType == RequestTypes.Post)
                {
                    var json = postObject.ToJson();
                    var requestStringContent = new HttpStringContent(json);
                    response = await Client.PostAsync(new Uri(BungieBaseUri + endpoint), requestStringContent);
                    if (response.StatusCode == HttpStatusCode.TemporaryRedirect)
                    {
                        var url = response.Headers.Location;
                        var retryContent = new HttpStringContent(json);
                        response = await Client.PostAsync(url, retryContent);
                    }
                }
                //get
                else if (requestType == RequestTypes.Get)
                    response = await Client.GetAsync(new Uri(BungieBaseUri + endpoint));

                if (response != null && response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    return message.FromJson<T>();
                }
            }
            catch (Exception ex)
            {
                //log
            }
            return default(T);
        }

        public async Task<ResponseBase> ObtainAccessToken()
        {
            var response = await RunPostAsync<AuthenticationResponse>(new AccessTokenRequest {Code = AuthCode}, AccessTokenRequest);
            if (response?.ErrorCode == Success)
            {
                AccessToken = response.Response.AccessToken;
                RefreshToken = response.Response.RefreshToken;
            }
            return response;
        }

        public async Task<ResponseBase> RefreshAccessToken()
        {
            var response = await RunPostAsync<AuthenticationResponse>(new RefreshTokenRequest {RefreshToken = RefreshToken.Value}, RefreshTokenRequest);
            if (response?.ErrorCode == Success)
            {
                AccessToken = response.Response.AccessToken;
                RefreshToken = response.Response.RefreshToken;
            }
            return response;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}