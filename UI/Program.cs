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
            var options = new DbContextOptionsBuilder<WeatherContext>()
                .UseSqlite("Data Source=GroupNameWeatherData.db")
                .Options;

            using var context = new WeatherContext(options);
            context.CreateDatabaseIfNotExists();

            CsvLoader.LoadData("TempFuktData.csv", context);

            var weatherService = new WeatherService(context);
            var menu = new Menu(weatherService);

            menu.DisplayMenu();
        }
    }
}
