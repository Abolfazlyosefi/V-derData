using System;
using System.Collections.Generic;
using System.IO;

namespace Core
{
    public class CvsLoader
    {
        public static List<WeatherData> LoadWeatherDataFromCsv(string filePath)
        {
            var weatherDataList = new List<WeatherData>();

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines.Skip(1))  // Första raden är rubrik
            {
                var columns = line.Split(',');

                var weatherData = new WeatherData
                {
                    Date = DateTime.Parse(columns[0]),
                    Location = columns[1],
                    Temperature = double.Parse(columns[2]),
                    Humidity = double.Parse(columns[3])
                };

                weatherDataList.Add(weatherData);
            }

            return weatherDataList;
        }
    }
}
