using Microsoft.IdentityModel.Tokens;
using Notification.Application.Service.Interface;
using Notification.Domain;
using Notification.Dto.Model;
using Notification.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Notification.Application.Service
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly TokenConfiguration _tokenConfiguration;
        private readonly SigninConfigurations _signinConfigurations;

        public AuthenticationService(TokenConfiguration tokenConfigurations, SigninConfigurations signinConfigurations)
        {
            _tokenConfiguration = tokenConfigurations;
            _signinConfigurations = signinConfigurations;
        }

        public TokenDto CreateToken(string login)
        {
            if(string.IsNullOrEmpty(login) || login.Trim().ToLower() != "admin")
                throw new Exception("invalid login");
            

            var jti = Guid.NewGuid().ToString("N");
            var _expirationDate = DomainUtils.GetLocalDate() + TimeSpan.FromSeconds(_tokenConfiguration.Seconds);

            _expirationDate.AddMinutes(5);

            var handler = new JwtSecurityTokenHandler();
            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(login, "Login"), new[] {
                        new Claim(JwtRegisteredClaimNames.Jti, jti),
                        new Claim(JwtRegisteredClaimNames.UniqueName,login),
                        new Claim(JwtRegisteredClaimNames.Sub,login),
                        new Claim("Login",login),


                });
            var _token = CreateToken(identity, _expirationDate, handler);

            return new TokenDto
            {
                TokenJwt = $"Bearer {_token}",
                TokenRefresh = Guid.NewGuid().ToString(),
                JwtId = jti,
                ExpiryDate = _expirationDate
            };
        }

        private string CreateToken(ClaimsIdentity identity, DateTime expirationDate, JwtSecurityTokenHandler handler)
        {

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = _tokenConfiguration.Issuer,
                Audience = _tokenConfiguration.Audience,
                SigningCredentials = _signinConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = DomainUtils.GetLocalDate(),
                Expires = expirationDate

            });

            return handler.WriteToken(securityToken);
        }
    }
}
