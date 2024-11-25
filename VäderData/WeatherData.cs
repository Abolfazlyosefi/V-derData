using System;

namespace DataAccess
{
    public class WeatherData
    {
        public int Id { get; set; }
        public DateTime Datum { get; set; }
        public string Plats { get; set; } // "Ute" eller "Inne"
        public double Temperatur { get; set; }
        public double Luftfuktighet { get; set; }
    }
}
