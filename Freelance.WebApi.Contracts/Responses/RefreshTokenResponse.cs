﻿namespace Freelance.WebApi.Contracts.Responses
{
    public class RefreshTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
