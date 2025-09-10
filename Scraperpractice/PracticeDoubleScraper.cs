using System;
using System.Threading.Tasks;

namespace scraper_practice
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting all scrapers...");

            var mgTask = MGScraper.RunAsync();
            var toyotaTask = ToyotaScraper.RunAsync();
            var citroenTask = CitroenScraper.RunAsync();

            await Task.WhenAll(mgTask, toyotaTask, citroenTask);

            Console.WriteLine("✅ All scrapers finished.");
        }
    }
}