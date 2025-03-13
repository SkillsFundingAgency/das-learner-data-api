namespace SFA.DAS.LearnerData.Infrastructure.ConnectionFactory;

public class LocalDbTokenProvider : IManagedIdentityTokenProvider
{
    public Task<string> GetSqlAccessTokenAsync()
    {
        return Task.FromResult(default(string));
    }
}