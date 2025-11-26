using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using FellerBackend.Services.Interfaces;

namespace FellerBackend.Services;

public class JwtTokenHelper : IJwtTokenHelper
{
    private readonly IConfiguration _configuration;

    public JwtTokenHelper(IConfiguration configuration)
    {
  _configuration = configuration;
    }

    public string GenerateToken(string email, string nombre, string rol, int userId)
    {
    var jwtSettings = _configuration.GetSection("JwtSettings");
 var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, nombre),
            new Claim(ClaimTypes.Role, rol),
        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
    };

        var token = new JwtSecurityToken(
         issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
  claims: claims,
      expires: DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["ExpirationHours"])),
 signingCredentials: credentials
  );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public int? ValidateToken(string token)
    {
 try
 {
         var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            
            var tokenHandler = new JwtSecurityTokenHandler();
 var validationParameters = new TokenValidationParameters
            {
      ValidateIssuer = true,
     ValidateAudience = true,
     ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = key
            };

  var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
   var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
       
 return userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        }
        catch
        {
            return null;
        }
    }
}
