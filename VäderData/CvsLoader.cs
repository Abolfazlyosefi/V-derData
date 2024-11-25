using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DataAccess;

namespace Core
{
    public class CsvLoader
    {
        public static void LoadData(string filePath, WeatherContext context)
        {
            if (context.WeatherData.Any()) return; // Om data redan finns, läs inte in igen.

            var lines = File.ReadAllLines(filePath);
            var weatherDataList = new List<WeatherData>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                var datum = DateTime.Parse(parts[0]);
                var plats = parts[1];
                var temperatur = double.Parse(parts[2], CultureInfo.InvariantCulture);
                var luftfuktighet = double.Parse(parts[3], CultureInfo.InvariantCulture);

                weatherDataList.Add(new WeatherData
                {
                    Datum = datum,
                    Plats = plats,
                    Temperatur = temperatur,
                    Luftfuktighet = luftfuktighet
                });
            }

            context.WeatherData.AddRange(weatherDataList);
            context.SaveChanges();
        }
    }
}
