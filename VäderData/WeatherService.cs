using System;
using System.Collections.Generic;
using System.Linq;
using DataAccess;

namespace Core
{
    public class WeatherService
    {
        private readonly WeatherContext _context;

        public WeatherService(WeatherContext context)
        {
            _context = context;
        }

        // Returnera sorterade dagar baserat på temperatur
        public IEnumerable<(DateTime Date, double AverageTemp)> GetDaysSortedByTemperature(string location)
        {
            return _context.WeatherData
                .Where(w => w.Location == location)  // Anpassat för "Location"
                .GroupBy(w => w.Date.Date)  // Anpassat för "Date"
                .Select(g => new { Date = g.Key, AverageTemp = g.Average(w => w.Temperature) })  // Anpassat för "Temperature"
                .OrderByDescending(x => x.AverageTemp)
                .Select(x => (x.Date, x.AverageTemp));
        }

        // Returnera sorterade dagar baserat på luftfuktighet
        public IEnumerable<(DateTime Date, double AverageHumidity)> GetDaysSortedByHumidity(string location)
        {
            return _context.WeatherData
                .Where(w => w.Location == location)  // Anpassat för "Location"
                .GroupBy(w => w.Date.Date)  // Anpassat för "Date"
                .Select(g => new { Date = g.Key, AverageHumidity = g.Average(w => w.Humidity) })  // Anpassat för "Humidity"
                .OrderBy(x => x.AverageHumidity)
                .Select(x => (x.Date, x.AverageHumidity));
        }

        // Beräkna meteorologisk höststart
        public DateTime? GetMeteorologicalAutumnStart(string location)
        {
            var groupedTemps = _context.WeatherData
                .Where(w => w.Location == location)  // Anpassat för "Location"
                .GroupBy(w => w.Date.Date)  // Anpassat för "Date"
                .Select(g => new { Date = g.Key, AvgTemp = g.Average(w => w.Temperature) })  // Anpassat för "Temperature"
                .OrderBy(x => x.Date)
                .ToList();

            for (int i = 0; i < groupedTemps.Count - 4; i++)
            {
                if (groupedTemps.Skip(i).Take(5).All(g => g.AvgTemp >= 0 && g.AvgTemp <= 10))
                    return groupedTemps[i].Date;
            }
            return null;
        }

        // Beräkna meteorologisk vinterstart
        public DateTime? GetMeteorologicalWinterStart(string location)
        {
            var groupedTemps = _context.WeatherData
                .Where(w => w.Location == location)  // Anpassat för "Location"
                .GroupBy(w => w.Date.Date)  // Anpassat för "Date"
                .Select(g => new { Date = g.Key, AvgTemp = g.Average(w => w.Temperature) })  // Anpassat för "Temperature"
                .OrderBy(x => x.Date)
                .ToList();

            for (int i = 0; i < groupedTemps.Count - 4; i++)
            {
                if (groupedTemps.Skip(i).Take(5).All(g => g.AvgTemp < 0))
                    return groupedTemps[i].Date;
            }
            return null;
        }

        // Beräkna medeltemperatur för ett specifikt datum och plats
        public double GetAverageTemperatureForDate(DateTime date, string location)
        {
            var data = _context.WeatherData
                .Where(w => w.Date.Date == date.Date && w.Location == location)  // Anpassat för "Date" och "Location"
                .ToList();

            if (data.Any()) return data.Average(w => w.Temperature);  // Anpassat för "Temperature"
            return 0;
        }

        // Beräkna mögelrisk baserat på temperatur och luftfuktighet
        public double CalculateMoldRisk(double temperature, double humidity)
        {
            // En enkel modell för mögelrisken: ju högre fuktighet och lägre temperatur, desto högre risk
            return humidity * (20 - temperature) / 100;
        }
    }
}
