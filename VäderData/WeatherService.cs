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

        public double CalculateAverageTemperature(DateTime date, string location)
        {
            return _context.WeatherData
                .Where(w => w.Datum.Date == date.Date && w.Plats == location)
                .Average(w => w.Temperatur);
        }

        public List<WeatherData> SortByTemperature()
        {
            return _context.WeatherData
                .GroupBy(w => w.Datum.Date)
                .Select(g => new WeatherData
                {
                    Datum = g.Key,
                    Temperatur = g.Average(w => w.Temperatur)
                })
                .OrderByDescending(w => w.Temperatur)
                .ToList();
        }

        public List<WeatherData> SortByHumidity()
        {
            return _context.WeatherData
                .GroupBy(w => w.Datum.Date)
                .Select(g => new WeatherData
                {
                    Datum = g.Key,
                    Luftfuktighet = g.Average(w => w.Luftfuktighet)
                })
                .OrderBy(w => w.Luftfuktighet)
                .ToList();
        }

        public DateTime CalculateMeteorologicalAutumnStart()
        {
            return _context.WeatherData
                .Where(w => w.Temperatur < 10 && w.Plats == "Ute")
                .OrderBy(w => w.Datum)
                .Select(w => w.Datum)
                .FirstOrDefault();
        }

        public DateTime CalculateMeteorologicalWinterStart()
        {
            return _context.WeatherData
                .Where(w => w.Temperatur < 0 && w.Plats == "Ute")
                .OrderBy(w => w.Datum)
                .Select(w => w.Datum)
                .FirstOrDefault();
        }
    }
}
