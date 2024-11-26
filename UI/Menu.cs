using System;
using Core;

namespace UI
{
    public class Menu
    {
        private readonly WeatherService _weatherService;

        public Menu(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public void DisplayMenu()
        {
            while (true)
            {
                Console.Clear(); // För att rensa skärmen varje gång
                Console.WriteLine("Välj ett alternativ:");
                Console.WriteLine("1. Visa medeltemperatur för ett datum");
                Console.WriteLine("2. Sortera dagar efter medeltemperatur");
                Console.WriteLine("3. Sortera dagar efter luftfuktighet");
                Console.WriteLine("4. Visa meteorologisk höst och vinter");
                Console.WriteLine("5. Avsluta");

                var choice = Console.ReadLine();
                HandleUserChoice(choice);
            }
        }

        private void HandleUserChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    Console.WriteLine("Ange datum (yyyy-MM-dd):");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime date))
                    {
                        var avgTemp = _weatherService.CalculateAverageTemperature(date, "Ute");
                        Console.WriteLine($"Medeltemperatur för {date.ToShortDateString()}: {avgTemp:F1}°C");
                    }
                    else
                    {
                        Console.WriteLine("Felaktigt datumformat. Försök igen.");
                    }
                    break;

                case "2":
                    var sortedTemp = _weatherService.SortByTemperature();
                    Console.WriteLine("Sorterade dagar efter medeltemperatur:");
                    foreach (var data in sortedTemp)
                    {
                        Console.WriteLine($"{data.Datum.ToShortDateString()}: {data.Temperatur:F1}°C");
                    }
                    break;

                case "3":
                    var sortedHumidity = _weatherService.SortByHumidity();
                    Console.WriteLine("Sorterade dagar efter luftfuktighet:");
                    foreach (var data in sortedHumidity)
                    {
                        Console.WriteLine($"{data.Datum.ToShortDateString()}: {data.Luftfuktighet:F1}%");
                    }
                    break;

                case "4":
                    var autumn = _weatherService.CalculateMeteorologicalAutumnStart();
                    var winter = _weatherService.CalculateMeteorologicalWinterStart();
                    Console.WriteLine($"Meteorologisk höst startade: {autumn?.ToShortDateString() ?? "Ej funnen"}");
                    Console.WriteLine($"Meteorologisk vinter startade: {winter?.ToShortDateString() ?? "Ej funnen"}");
                    break;

                case "5":
                    Console.WriteLine("Avslutar programmet...");
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    break;
            }
            Console.WriteLine("Tryck på en tangent för att fortsätta...");
            Console.ReadKey(); // Vänta på användaren innan menyn visas igen
        }
    }
}
