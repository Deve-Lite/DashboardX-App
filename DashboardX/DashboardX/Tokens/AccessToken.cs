using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace DashboardX.Tokens;

public class AccessToken : Token
{
    private IEnumerable<Claim> claims;

    public IEnumerable<Claim> Claims => claims;
    

    public AccessToken() : base()
    {
        claims = new List<Claim>();
    }

    public AccessToken(string token, TimeSpan maxRequestTime) : base()
    {
        claims = new List<Claim>();
        this.value = token;

        try
        {
            this.maxRequestTime = maxRequestTime;
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            claims  = jwtToken.Claims;
            var expClaim = claims.FirstOrDefault(c => c.Type == "exp");

            if (expClaim != null)
            {
                var exp = long.Parse(expClaim.Value);
                expiration = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
            }
            else
                expiration = DateTime.MaxValue;
        }
        catch
        {
            expiration = DateTime.MinValue;
            isValid = false;
        }
    }
}
