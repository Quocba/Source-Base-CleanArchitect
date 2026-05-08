namespace Domain.Config;

public class SecuritySettings
{
    public int RateLimitCount { get; set; } = 1000;
    public int RateLimitWindowSeconds { get; set; } = 60;
    public int MaliciousIpPenaltyMinutes { get; set; } = 60;
    public List<string> WhitelistedIps { get; set; } = new();
}
