using Microsoft.IdentityModel.Tokens;
using Notification.Dto.Model;
using System;

using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace Notification.Application.Utils
{
    public static class Utils
    {        

        public static string DescriptionEnum<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return source.ToString();
            }
        }

        public static string VerificationToken(string token)
        {

            if (token.ToLower().StartsWith("bearer"))
                token = token.Replace("bearer", "").Replace("Bearer", "").Trim();

            return token;

        }

        public static DateTime GetLocalDate()
        {
            try
            {
                DateTime timeUtc = DateTime.UtcNow;
                var brasilia = TimeZoneInfo.FindSystemTimeZoneById("Brazil/East");
                return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, brasilia);

            }
            catch (System.Exception)
            {

                return DateTime.Now;
            }


        }

        private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       System.StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
