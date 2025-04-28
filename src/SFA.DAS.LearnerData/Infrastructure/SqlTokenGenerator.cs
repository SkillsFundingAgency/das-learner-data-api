using Azure.Core;
using Azure.Identity;

namespace SFA.DAS.LearnerData.Infrastructure;

public static class SqlTokenGenerator
{
    private const string AzureResource = "https://database.windows.net/";

    private static readonly ChainedTokenCredential AzureServiceTokenProvider = new(
        new ManagedIdentityCredential(),
        new AzureCliCredential(),
        new VisualStudioCodeCredential(),
        new VisualStudioCredential());

    public static string Generate()
    {
        return AzureServiceTokenProvider.GetToken(new TokenRequestContext(scopes: [AzureResource])).Token;
    }
}