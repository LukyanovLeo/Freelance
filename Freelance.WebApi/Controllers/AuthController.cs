using Freelance.WebApi.Contracts.Common;
using Freelance.WebApi.Contracts.Requests;
using Freelance.WebApi.Contracts.Responses;
using Freelance.WebApi.QueryHandlers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Freelance.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthQueryHandler _queryHandler;

        private Guid UserId => Guid.Parse(User.Claims.Single(x => x.Type == JwtClaimTypes.UserId).Value);
        private List<Claim> UserClaims => User.Claims.ToList();
        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        public AuthController(IAuthQueryHandler queryHandler)
        {
            _queryHandler = queryHandler;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public Task<LoginResponse> Login(LoginRequest request) => _queryHandler.Login(request.Email, ipAddress());

        [AllowAnonymous]
        [HttpGet("getSessionSalt/{email}")]
        public Task<GetSessionSaltResponse> GetSessionSalt(string email) => _queryHandler.GetSessionSalt(email);

        [AllowAnonymous]
        [HttpGet("getPublicKey")]
        public Task<GetPublicKeyResponse> GetPublicKey() => Task.FromResult(new GetPublicKeyResponse { PublicKey = _queryHandler.GetPublicKey() });

        [AllowAnonymous]
        [HttpPost("registration")]
        public Task<RegistrationResponse> Registration(RegistrationRequest request) => _queryHandler.Registration(request.Email, request.Password, ipAddress());

        [AllowAnonymous]
        [HttpPost("refreshToken")]
        public Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request) => _queryHandler.RefreshToken(UserId, request.RefreshToken, UserClaims, ipAddress());

        [AllowAnonymous]
        [HttpPost("revokeToken")]
        public Task<bool> RevokeToken(RevokeTokenRequest request) => _queryHandler.RevokeToken(UserId, ipAddress());
    }
}
