using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.LearnerData.Configuration;
using SFA.DAS.LearnerData.Data.Entities;

namespace SFA.DAS.LearnerData.Data;

public class LearnerDataDbContext(LearnerDataApi configuration, DbContextOptions options) : DbContext(options)
{
    public DbSet<Learner?> Learners { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (configuration is null)
        {
            return;
        }

        optionsBuilder.UseSqlServer(new SqlConnection
        {
            ConnectionString = configuration.DatabaseConnectionString,
        }, options => options
            .EnableRetryOnFailure(
                5,
                TimeSpan.FromSeconds(20),
                null
            ));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearnerDataDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        PopulateModificationHistoryValues();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void PopulateModificationHistoryValues()
    {
        var entries = ChangeTracker.Entries();

        var modificationHistoryList = entries.Where(entry => entry is { Entity: IModificationHistory, State: EntityState.Added or EntityState.Modified });

        foreach (var history in modificationHistoryList)
        {
            var modificationHistory = (IModificationHistory)history.Entity;

            modificationHistory.UpdatedDate = DateTime.UtcNow;

            if (history.State == EntityState.Added)
            {
                modificationHistory.CreatedDate = DateTime.UtcNow;
            }
        }
    }
}