namespace DashboardX.Tokens;

public class RefreshToken : Token
{
    public RefreshToken(string token, TimeSpan maxRequestTime) : base(token, maxRequestTime)
    {
    }
}
