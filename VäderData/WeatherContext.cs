using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataAccess
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherData> WeatherData { get; set; }

        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options) { }

        public void CreateDatabaseIfNotExists()
        {
            Database.EnsureCreated(); // Skapar databasen om den inte redan finns.
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=GroupNameWeatherData.db"); // Använd gruppens namn här.
            }
        }
    }
}
