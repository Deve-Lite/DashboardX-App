using System.IdentityModel.Tokens.Jwt;

namespace DashboardX.Tokens;

public class Token
{
    protected TimeSpan maxRequestTime;
    protected string value;
    protected DateTime expiration;
    public string Value
    {
        get => value;
        set => this.value = value;
    }

    public Token() 
    {
        maxRequestTime = TimeSpan.FromSeconds(0);
        expiration = DateTime.UnixEpoch;
        value = string.Empty;
    }

    public Token(string token, TimeSpan maxRequestTime)
    {
        this.value = token;

        this.maxRequestTime = maxRequestTime;
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var claims = jwtToken.Claims;
        var expClaim = claims.FirstOrDefault(c => c.Type == "exp");

        if (expClaim != null)
        {
            var exp = long.Parse(expClaim.Value);
            expiration = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
        }
        else
            expiration = DateTime.MaxValue;
        
    }

    public DateTime Expiration
    {
        get => expiration;
        set => expiration = value;
    }

    public bool RequiresRefresh => Expiration + maxRequestTime + TimeSpan.FromSeconds(1) > DateTime.Now;
}
