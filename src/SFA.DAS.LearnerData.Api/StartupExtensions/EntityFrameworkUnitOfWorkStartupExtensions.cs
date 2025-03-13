using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using NServiceBus.Persistence;
using SFA.DAS.LearnerData.Configuration;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.NServiceBus.SqlServer.Data;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.LearnerData.Api.StartupExtensions;

public static class EntityFrameworkUnitOfWorkStartupExtensions
{
    public static IServiceCollection AddEntityFrameworkForLearnerData(this IServiceCollection services, LearnerDataApi config)
    {
        return services.AddScoped(provider =>
        {
            var unitOfWorkContext = provider.GetService<IUnitOfWorkContext>();
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            LearnerDataDbContext dbContext;
            
            try
            {
                var synchronizedStorageSession = unitOfWorkContext.Get<SynchronizedStorageSession>();
                var sqlStorageSession = synchronizedStorageSession.GetSqlStorageSession();
                var optionsBuilder = new DbContextOptionsBuilder<LearnerDataDbContext>().UseSqlServer(sqlStorageSession.Connection);
                dbContext = new LearnerDataDbContext(sqlStorageSession.Connection, config, azureServiceTokenProvider, optionsBuilder.Options);
                dbContext.Database.UseTransaction(sqlStorageSession.Transaction);
            }
            catch (KeyNotFoundException)
            {
                var optionsBuilder = new DbContextOptionsBuilder<LearnerDataDbContext>().UseSqlServer(config.DatabaseConnectionString);
                optionsBuilder.UseLoggerFactory(DebugLoggingFactory);
                dbContext = new LearnerDataDbContext(optionsBuilder.Options);
            }

            return dbContext;
        });
    }

    private static readonly LoggerFactory DebugLoggingFactory = new([new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()]);
}