using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CsvHelper;

namespace scraper_practice
{
    public class MGScraper
    {
        public static async Task RunAsync()
        {
            var apiUrl = "https://www.mg.co.uk/api/all-dealers";

            var dealers = await ScrapeApiAsync<List<MGDealer>>(apiUrl); // top-level array

            if (dealers is { Count: > 0 })
            {
                var rows = new List<MGDealerCsvRow>();

                foreach (var dealer in dealers)
                {
                    var (lat, lon) = ParseLatLon(dealer.LatitudeLongitude);

                    var row = new MGDealerCsvRow
                    {
                        Name = dealer.Name,
                        AddressLine = dealer.Address1 ?? "",
                        City = dealer.Address2 ?? "",
                        County = dealer.Town ?? "",
                        Postcode = dealer.Postcode ?? "",
                        Phone = dealer.PhoneNumber ?? "",

                        // NEW:
                        Latitude = lat?.ToString(CultureInfo.InvariantCulture) ?? "",
                        Longitude = lon?.ToString(CultureInfo.InvariantCulture) ?? "",
                        DealerUrl = dealer.Website ?? "",
                        DealerId = dealer.MgId ?? ""
                    };

                    rows.Add(row);

                    Console.WriteLine(
                        $"MG: {row.Name} | {row.AddressLine} | {row.City} | {row.County} | {row.Postcode} | {row.Phone} | " +
                        $"Lat:{row.Latitude} Lon:{row.Longitude} | URL:{row.DealerUrl} | ID:{row.DealerId}"
                    );
                }

                ExportToCsv(rows, "MG_Dealerships.csv");
            }
            else
            {
                Console.WriteLine("❌ No MG dealers found or response invalid.");
            }
        }

        private static async Task<T?> ScrapeApiAsync<T>(string url)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36"
            );
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Referrer = new Uri("https://www.mg.co.uk/");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-GB,en-US;q=0.9,en;q=0.8");
            client.DefaultRequestHeaders.Connection.ParseAdd("keep-alive");

            try
            {
                var response = await client.GetAsync(url);
                var responseText = await response.Content.ReadAsStringAsync();
                File.WriteAllText("mg_dealers_raw.json", responseText);

                if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseText))
                    return default;

                return JsonSerializer.Deserialize<T>(responseText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error scraping MG dealers: {ex.Message}");
                return default;
            }
        }

        // Robustly parse "lat lon" or "lat,lon"
        private static (double? lat, double? lon) ParseLatLon(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return (null, null);

            var cleaned = value.Replace(",", " ").Trim();
            var parts = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return (null, null);

            if (double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
            {
                return (lat, lon);
            }

            return (null, null);
        }

        private static void ExportToCsv(List<MGDealerCsvRow> rows, string fileName)
        {
            using var writer = new StreamWriter(fileName);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<MGDealerCsvRow>();
            csv.NextRecord();
            csv.WriteRecords(rows);
        }
    }

    public class MGDealer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [JsonPropertyName("address1")]
        public string Address1 { get; set; } = string.Empty;

        [JsonPropertyName("address2")]
        public string Address2 { get; set; } = string.Empty;

        [JsonPropertyName("town")]
        public string Town { get; set; } = string.Empty;

        [JsonPropertyName("postcode")]
        public string Postcode { get; set; } = string.Empty;

        // NEW FIELDS FROM THE API
        [JsonPropertyName("latitude_longitude")]
        public string LatitudeLongitude { get; set; } = string.Empty; // e.g. "55.942651 -3.408369"

        [JsonPropertyName("website")]
        public string Website { get; set; } = string.Empty; // e.g. "https://dealer.mg.co.uk/wellington-mg"

        [JsonPropertyName("mg_id")]
        public string MgId { get; set; } = string.Empty; // e.g. "MG165"
    }

    public class MGDealerCsvRow
    {
        // Display names here are just for clarity; CsvHelper uses property names by default for headers.
        public string Name { get; set; } = string.Empty;                 // Dealer Name
        public string AddressLine { get; set; } = string.Empty;          // Street Address
        public string City { get; set; } = string.Empty;                 // City
        public string County { get; set; } = string.Empty;               // County
        public string Postcode { get; set; } = string.Empty;             // Postal Code
        public string Phone { get; set; } = string.Empty;                // Phone Number

        // NEW OUTPUT COLUMNS
        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string DealerUrl { get; set; } = string.Empty;            // specific dealer page
        public string DealerId { get; set; } = string.Empty;             // mg_id
    }
}
