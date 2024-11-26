using System;
using Core;
using CsvHelper;
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

            // Exempel på medeltemperatur för utomhus på ett valt datum
            Console.Write("Ange datum (yyyy-MM-dd): ");
            DateTime date = DateTime.Parse(Console.ReadLine());
            Console.WriteLine($"Medeltemperatur utomhus: {weatherService.GetAverageTemperatureForDate(date, "Ute")} °C");

            // Sortera dagar efter medeltemperatur (utomhus)
            var sortedDays = weatherService.GetDaysSortedByTemperature("Ute");
            foreach (var day in sortedDays)
                Console.WriteLine($"{day.Date.ToShortDateString()}: {day.AverageTemp} °C");

            // Visa meteorologisk höststart
            var autumnStart = weatherService.GetMeteorologicalAutumnStart("Ute");
            Console.WriteLine(autumnStart.HasValue ? $"Meteorologisk höst börjar: {autumnStart.Value.ToShortDateString()}" : "Höst ej funnen.");

            // Medeltemperatur inomhus för ett valt datum
            Console.Write("Ange datum (yyyy-MM-dd): ");
            date = DateTime.Parse(Console.ReadLine());
            Console.WriteLine($"Medeltemperatur inomhus: {weatherService.GetAverageIndoorTemperatureForDate(date)} °C");

            // Sortera inomhusdata
            Console.WriteLine("Sortering av varmaste till kallaste dag (inomhus):");
            var sortedTemperatures = weatherService.GetSortedIndoorTemperatures();
            foreach (var item in sortedTemperatures)
            {
                Console.WriteLine($"{item.Date.ToString("yyyy-MM-dd")} - {item.AvgTemperature} °C");
            }

            Console.WriteLine("Sortering av torraste till fuktigaste dag (inomhus):");
            var sortedHumidity = weatherService.GetSortedIndoorHumidity();
            foreach (var item in sortedHumidity)
            {
                Console.WriteLine($"{item.Date.ToString("yyyy-MM-dd")} - {item.AvgHumidity}%");
            }

            Console.WriteLine("Sortering av minst till störst risk för mögel (inomhus):");
            var sortedMoldRisk = weatherService.GetSortedMoldRisk();
            foreach (var item in sortedMoldRisk)
            {
                Console.WriteLine($"{item.Date.ToString("yyyy-MM-dd")} - Risk: {item.MoldRisk:F2}");
            }
        }
    }
}
