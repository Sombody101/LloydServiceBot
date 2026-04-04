using LloydBot.Models.Main;
using Microsoft.EntityFrameworkCore;

namespace LloydBot.Context;

public class LloydBotContext : DbContext
{
    public LloydBotContext(DbContextOptions<LloydBotContext> options)
        : base(options)
    { }

    public DbSet<BlacklistedDbEntity> Blacklist { get; set; }

    public DbSet<UserDbEntity> Users { get; set; }
    public DbSet<GuildDbEntity> Guilds { get; set; }
    public DbSet<GuildConfigDbEntity> Configs { get; set; }
    public DbSet<IncidentDbEntity> Incidents { get; set; }
    public DbSet<StarboardMessageDbEntity> Starboard { get; set; }
    public DbSet<QuoteDbEntity> Quotes { get; set; }
    public DbSet<ReminderDbEntity> Reminders { get; set; }
    public DbSet<VoiceAlert> VoiceAlerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(LloydBotContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.UseSqlite(DbConstants.DB_CONNECTION_STRING);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
    }
}