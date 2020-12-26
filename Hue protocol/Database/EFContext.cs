using HueProtocol.models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

namespace HueProtocol.Database
{
    public sealed class EFContext : DbContext
    {
        private readonly string _connectionString;

        public EFContext(Configuration configuration)
        {
            _connectionString = configuration.ConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(_connectionString);
            options.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #region Bot

        public DbSet<BotData> BotData { get; set; }
        public DbSet<Event> Events { get; set; }

        #endregion

        #region Guild

        public DbSet<ServerGuild> ServersSettings { get; set; }
        public DbSet<ReactionPost> ReactionPosts { get; set; }

        #endregion

        #region User

        public DbSet<UserData> UserData { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<Strike> Strikes { get; set; }
        public DbSet<Warn> Warns { get; set; }
        public DbSet<Birthday> Birthday { get; set; }
        public DbSet<UserClass> userclass { get; set; }
        public DbSet<Gear> gear { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemAttribute> ItemAttributes { get; set; }
        public DbSet<Equipment> Equipments { get; set; }

        #endregion
    }
}