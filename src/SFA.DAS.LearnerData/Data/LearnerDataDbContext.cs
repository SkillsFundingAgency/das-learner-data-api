using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Configuration;
using SFA.DAS.LearnerData.Data.Entities;

namespace SFA.DAS.LearnerData.Data;

public class LearnerDataDbContext : DbContext
{
    private readonly IDbConnection _connection;
    private readonly LearnerDataApi _configuration;
    private readonly AzureServiceTokenProvider _azureServiceTokenProvider;
    
    public DbSet<Learner> Learners { get; set; }

    public LearnerDataDbContext(DbContextOptions<LearnerDataDbContext> options) : base(options)
    {
    }

    public LearnerDataDbContext(IDbConnection connection, LearnerDataApi configuration, AzureServiceTokenProvider azureServiceTokenProvider, DbContextOptions options):base(options)
    {
        _connection = connection;
        _configuration = configuration;
        _azureServiceTokenProvider = azureServiceTokenProvider;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration is null || _azureServiceTokenProvider is null)
        {
            return;
        }

        optionsBuilder.UseSqlServer(_connection as SqlConnection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearnerDataDbContext).Assembly);
    }
}