namespace SFA.DAS.LearnerData.Configuration;

public record LearnerDataApi
{
    public string DatabaseConnectionString { get; set; }
    public string NServiceBusConnectionString { get; set; }
    public string NServiceBusLicense { get; set; }
    public string DataProtectionKeysDatabase { get; set; }
    public string RedisConnectionString { get; set; }
}