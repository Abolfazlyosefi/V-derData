using System;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class WeatherData
    {
        [Key]
        public int Id { get; set; }  // Primärnyckel för WeatherData

        public DateTime Date { get; set; }

        public string Location { get; set; }  // "Ute" eller "Inne"

        public double Temperature { get; set; }

        public double Humidity { get; set; }
    }
}
