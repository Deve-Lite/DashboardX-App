using System.IdentityModel.Tokens.Jwt;

namespace DashboardX.Tokens;

public class AccessToken : Token
{
    public AccessToken(string token, TimeSpan maxRequestTime) : base()
    {
        this.value = token;

        this.maxRequestTime = maxRequestTime;
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var claims = jwtToken.Claims;
        var expClaim = claims.FirstOrDefault(c => c.Type == "exp");

        //TODO: Read other claims here

        if (expClaim != null)
        {
            var exp = long.Parse(expClaim.Value);
            expiration = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
        }
        else
            expiration = DateTime.MaxValue;
    }
}
