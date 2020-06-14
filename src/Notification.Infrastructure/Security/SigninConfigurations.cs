using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Notification.Infrastructure.Security
{
    public class SigninConfigurations
    {
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigninConfigurations(IConfiguration _configuration)
        {
            Key = new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes(_configuration["TokenConfigurations:SecurityKey"]));

            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

        }
    }
}
