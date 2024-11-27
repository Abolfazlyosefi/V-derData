using System;
using System.Collections.Generic;
using System.Linq;
using Core;



namespace Core
{
    public class WeatherService
    {
        private readonly WeatherContext _context;

        public WeatherService(WeatherContext context)
        {
            _context = context;
        }

        // Hjälpmetod för att gruppera och beräkna genomsnitt per dag
        private IEnumerable<(DateTime Date, double Value)> GroupAndCalculateAverage(string location, Func<WeatherData, double> selector)
        {
            return _context.WeatherData
                .Where(w => w.Location == location)
                .GroupBy(w => w.Date.Date)
                .Select(g => (Date: g.Key, Value: g.Average(selector)));
        }

        // Returnera sorterade dagar baserat på temperatur
        public IEnumerable<(DateTime Date, double AverageTemp)> GetDaysSortedByTemperature(string location)
        {
            if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Plats kan inte vara tom.");
            return GroupAndCalculateAverage(location, w => w.Temperature)
                .OrderByDescending(x => x.Value);
        }

        // Returnera sorterade dagar baserat på luftfuktighet
        public IEnumerable<(DateTime Date, double AverageHumidity)> GetDaysSortedByHumidity(string location)
        {
            if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Plats kan inte vara tom.");
            return GroupAndCalculateAverage(location, w => w.Humidity)
                .OrderBy(x => x.Value);
        }

        // Beräkna medeltemperatur för ett specifikt datum och plats
        public double GetAverageTemperatureForDate(DateTime date, string location)
        {
            if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Plats kan inte vara tom.");
            if (date == default) throw new ArgumentException("Datum är inte giltigt.");

            var data = _context.WeatherData
                .Where(w => w.Date.Date == date.Date && w.Location == location)
                .ToList();

            if (data.Any())
            {
                return data.Average(w => w.Temperature);
            }

            throw new InvalidOperationException("Ingen data tillgänglig för det angivna datumet och platsen.");
        }

        // Beräkna medeltemperatur för inomhusdata på ett specifikt datum
        public double GetAverageIndoorTemperatureForDate(DateTime date)
        {
            var data = _context.WeatherData
                .Where(w => w.Date.Date == date.Date && w.Location == "Inne")
                .ToList();

            if (data.Any())
            {
                return data.Average(w => w.Temperature);
            }

            throw new InvalidOperationException("Ingen inomhusdata tillgänglig för det angivna datumet.");
        }

        // Returnera sorterade dagar baserat på inomhustemperatur (varmaste till kallaste)
        public IEnumerable<(DateTime Date, double AvgTemperature)> GetSortedIndoorTemperatures()
        {
            return GroupAndCalculateAverage("Inne", w => w.Temperature)
                .OrderByDescending(x => x.Value);
        }

        // Returnera sorterade dagar baserat på inomhusluftfuktighet (torraste till fuktigaste)
        public IEnumerable<(DateTime Date, double AvgHumidity)> GetSortedIndoorHumidity()
        {
            return GroupAndCalculateAverage("Inne", w => w.Humidity)
                .OrderBy(x => x.Value);
        }

        // Beräkna mögelrisk baserat på temperatur och luftfuktighet
        public double CalculateMoldRisk(double temperature, double humidity)
        {
            if (humidity < 0 || humidity > 100) throw new ArgumentOutOfRangeException("Luftfuktighet måste vara mellan 0 och 100.");
            return humidity * (20 - temperature) / 100;
        }

        // Beräkna mögelrisk för inomhusdata baserat på temperatur och luftfuktighet
        public IEnumerable<(DateTime Date, double MoldRisk)> GetSortedMoldRisk()
        {
            return _context.WeatherData
                .Where(w => w.Location == "Inne")
                .GroupBy(w => w.Date.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    MoldRisk = g.Average(w => CalculateMoldRisk(w.Temperature, w.Humidity))
                })
                .OrderBy(x => x.MoldRisk)
                .Select(x => (x.Date, x.MoldRisk));
        }

        // Hjälpmetod för att beräkna meteorologiska periodstarter
        private DateTime? CalculateMeteorologicalStart(string location, Func<double, bool> condition, int consecutiveDays = 5)
        {
            if (string.IsNullOrWhiteSpace(location)) throw new ArgumentException("Plats kan inte vara tom.");

            var groupedTemps = GroupAndCalculateAverage(location, w => w.Temperature)
                .OrderBy(x => x.Date)
                .ToList();

            for (int i = 0; i <= groupedTemps.Count - consecutiveDays; i++)
            {
                if (groupedTemps.Skip(i).Take(consecutiveDays).All(g => condition(g.Value)))
                {
                    return groupedTemps[i].Date;
                }
            }
            return null;
        }

        // Beräkna meteorologisk höststart
        public DateTime? GetMeteorologicalAutumnStart(string location)
        {
            return CalculateMeteorologicalStart(location, temp => temp >= 0 && temp <= 10);
        }

        // Beräkna meteorologisk vinterstart
        public DateTime? GetMeteorologicalWinterStart(string location)
        {
            return CalculateMeteorologicalStart(location, temp => temp < 0);
        }
    }
}
