using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core
{
    public class CsvLoader
    {
        public static List<WeatherData> LoadWeatherDataFromCsv(string filePath, char delimiter = ',')
        {
            var weatherDataList = new List<WeatherData>();

            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("CSV-filen kunde inte hittas.", filePath);
                }

                var lines = File.ReadAllLines(filePath);

                if (lines.Length < 2)
                {
                    throw new InvalidDataException("CSV-filen innehåller inga data eller saknar rubrikrad.");
                }

                foreach (var line in lines.Skip(1))  // Hoppa över rubrikraden
                {
                    var columns = line.Split(delimiter);

                    if (columns.Length != 4)
                    {
                        Console.WriteLine($"Ogiltig rad ignoreras: {line}");
                        continue; // Hoppa över rader med felaktigt antal kolumner
                    }

                    if (!DateTime.TryParse(columns[0], out var date) ||
                        !double.TryParse(columns[2], out var temperature) ||
                        !double.TryParse(columns[3], out var humidity))
                    {
                        Console.WriteLine($"Ogiltig data på rad ignoreras: {line}");
                        continue; // Hoppa över rader med ogiltigt format
                    }

                    var weatherData = new WeatherData
                    {
                        Date = date,
                        Location = columns[1],
                        Temperature = temperature,
                        Humidity = humidity
                    };

                    weatherDataList.Add(weatherData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ett fel inträffade vid inläsning av CSV-filen: {ex.Message}");
            }

            return weatherDataList;
        }
    }
}
