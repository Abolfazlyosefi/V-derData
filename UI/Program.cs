using System;
using Core;
using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace UI
{
    class Program
    {
        static void Main(string[] args)
        {
            // Skapa DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<WeatherContext>();
            optionsBuilder.UseSqlite("Data Source=weatherdata.db");

            // Skapa en instans av WeatherContext
            using var context = new WeatherContext(optionsBuilder.Options);

            // Initiera och fyll på databasen om det behövs
            context.Database.EnsureCreated(); // Skapar databasen om den inte finns
            CsvLoader.LoadData("TempFuktData.csv", context); // Ladda data från CSV till databasen

            // Skapa instans av WeatherService och Menu
            var weatherService = new WeatherService(context);
            var menu = new Menu(weatherService);

            // Visa menyn för användaren
            menu.DisplayMenu();

            // Här kan vi lägga till exempelanrop för nya funktioner:
            // Exempel på att hämta medeltemperatur för inomhusdata för ett specifikt datum
            Console.Write("Ange ett datum för inomhus medeltemperatur (yyyy-MM-dd): ");
            DateTime date = DateTime.Parse(Console.ReadLine());
            try
            {
                var indoorAvgTemp = weatherService.GetAverageIndoorTemperatureForDate(date);
                Console.WriteLine($"Medeltemperatur inomhus den {date.ToShortDateString()}: {indoorAvgTemp:F1}°C");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fel: {ex.Message}");
            }

            // Exempel på att hämta och skriva ut sorterade inomhustemperaturer
            Console.WriteLine("\nSorterade inomhustemperaturer från varmast till kallast:");
            var sortedIndoorTemps = weatherService.GetSortedIndoorTemperatures();
            foreach (var item in sortedIndoorTemps)
            {
                Console.WriteLine($"{item.Date.ToShortDateString()} - {item.AvgTemperature:F1}°C");
            }

            // Exempel på att hämta och skriva ut mögelrisk för inomhusmiljöer
            Console.WriteLine("\nSorterade dagar baserat på mögelrisk (från lägst till högst risk):");
            var sortedMoldRisk = weatherService.GetSortedMoldRisk();
            foreach (var item in sortedMoldRisk)
            {
                Console.WriteLine($"{item.Date.ToShortDateString()} - Risk: {item.MoldRisk:F2}");
            }

            // Menyn är redan hanterad genom att anropa menu.DisplayMenu(), så vi gör inget mer här i main.
        }
    }
}
