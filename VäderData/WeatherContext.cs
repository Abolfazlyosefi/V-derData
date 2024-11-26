using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Core
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherData> WeatherData { get; set; }

        // Konfiguration för SQLite-databasen (eller annan databas)
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=VaderData.db");
        }
    }
}
