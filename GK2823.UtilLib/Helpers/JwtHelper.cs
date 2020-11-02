using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace GK2823.UtilLib.Helpers
{
    public class JwtHelper
    {
        public static string CreatJwtToken(string Id)
        {
            var claimsIdentity = new ClaimsIdentity();

            claimsIdentity.AddClaim(new Claim("gk2823", Id));
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Sid, "3"));
            //claimsIdentity.AddClaim(new Claim(ClaimTypes.Expired, "4"));
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("mysecret12345678");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddSeconds(60),
                IssuedAt = DateTime.Now,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string OpenJwtToken(string token)
        {
            string info = string.Empty;
            try
            {
                var k = new JwtSecurityToken(token);
                info = k.Claims.Where(p => p.Type == "gk2823").FirstOrDefault().Value;
            }
            catch (Exception ex)
            {

            }
            return info;
        }
    }
}
