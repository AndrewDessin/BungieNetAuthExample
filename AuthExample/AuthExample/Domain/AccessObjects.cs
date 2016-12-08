namespace AuthExample.Domain
{
    public class AuthenticationResponse : ResponseBase
    {
        public ResponseObj Response { get; set; }
    }

    public class ResponseObj
    {
        public AccessToken AccessToken { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public int Scope { get; set; }
    }

    public class AccessToken
    {
        public string Value { get; set; }
        public int Readyin { get; set; }
        public int Expires { get; set; }
    }

    public class RefreshToken
    {
        public string Value { get; set; }
        public int Readyin { get; set; }
        public int Expires { get; set; }
    }

    public class AccessTokenRequest
    {
        public string Code { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class ResponseBase
    {
        public string Message { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorStatus { get; set; }

        public int ThrottleSeconds { get; set; }
    }
}