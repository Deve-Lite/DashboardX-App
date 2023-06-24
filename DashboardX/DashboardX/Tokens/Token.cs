using System.IdentityModel.Tokens.Jwt;

namespace DashboardX.Tokens;

public abstract class Token
{
    public static string AccessTokenName => "accessToken";
    public static string RefreshTokenName => "refreshToken";


    protected TimeSpan maxRequestTime;
    protected string value;
    protected DateTime expiration;
    protected bool isValid;

    public string Value
    {
        get => value;
        set => this.value = value;
    }

    public bool IsValid => isValid;

    public Token() 
    {
        maxRequestTime = TimeSpan.FromSeconds(0);
        expiration = DateTime.UnixEpoch;
        value = string.Empty;
        isValid = false;
    }

    public Token(string token, TimeSpan maxRequestTime)
    {
        this.value = token;

        try
        {
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
        catch
        {
            expiration = DateTime.MinValue;
            isValid = false;
        }
    }

    public DateTime Expiration
    {
        get => expiration;
        set => expiration = value;
    }

    public bool RequiresRefresh => Expiration + maxRequestTime + TimeSpan.FromSeconds(1) > DateTime.Now;
}
