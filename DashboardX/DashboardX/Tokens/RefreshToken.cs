namespace DashboardX.Tokens;

public class RefreshToken : Token
{
    public RefreshToken() : base()
    {
        
    }
    public RefreshToken(string token, TimeSpan maxRequestTime) : base(token, maxRequestTime)
    {
    }
}
