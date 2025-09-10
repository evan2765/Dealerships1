using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration.Attributes;

namespace scraper_practice
{
    public class ToyotaScraper
    {
        public static async Task RunAsync()
        {
            var apiUrl = "https://kong-proxy-intranet.toyota-europe.com/dxp/dealers/api/toyota/gb/en/drive/-0.118092/51.509865?extraCountries=je|im|gg&count=500";

            var apiResponse = await ScrapeApiAsync<ToyotaDealerApiResponse>(apiUrl);

            if (apiResponse?.Dealers is { Count: > 0 })
            {
                var rows = new List<ToyotaDealerCsvRow>();

                foreach (var dealer in apiResponse.Dealers)
                {
                    // Prefer "geo", but fall back to other common keys if the API changes.
                    var coords = dealer.Geo ?? dealer.Coordinates ?? dealer.Location;

                    // Build a full, absolute URL for the dealer page.
                    var dealerUrl = string.IsNullOrWhiteSpace(dealer.T1Url)
                        ? ""
                        : new Uri(new Uri("https://www.toyota.co.uk"),
                                  dealer.T1Url.StartsWith("/") ? dealer.T1Url : "/" + dealer.T1Url)
                          .ToString();

                    var row = new ToyotaDealerCsvRow
                    {
                        Name        = dealer.Name,
                        AddressLine = dealer.Address?.AddressLine1 ?? "",
                        City        = dealer.Address?.City ?? "",
                        Postcode    = dealer.Address?.Postcode ?? "",
                        Region      = dealer.Address?.Region ?? "",
                        Phone       = dealer.Phone ?? "",

                        DealerId    = dealer.LocalDealerId ?? "",
                        DealerUrl   = dealerUrl,
                        Latitude    = coords?.Lat,
                        Longitude   = coords?.Lon
                    };

                    rows.Add(row);

                    Console.WriteLine(
                        $"Toyota: {row.Name} | {row.AddressLine} | {row.City} | {row.Postcode} | {row.Region} | {row.Phone} | ID={row.DealerId} | URL={row.DealerUrl} | {row.Latitude},{row.Longitude}");
                }

                ExportToCsv(rows, "Toyota_Dealerships.csv");
            }
            else
            {
                Console.WriteLine("No Toyota dealers found.");
            }
        }

        private static async Task<T?> ScrapeApiAsync<T>(string url)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            try
            {
                var response = await client.GetAsync(url);
                var responseText = await response.Content.ReadAsStringAsync();
                File.WriteAllText("toyota_dealers_raw.json", responseText);

                if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseText))
                    return default;

                return JsonSerializer.Deserialize<T>(responseText);
            }
            catch
            {
                return default;
            }
        }

        private static void ExportToCsv(List<ToyotaDealerCsvRow> rows, string fileName)
        {
            using var writer = new StreamWriter(fileName);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<ToyotaDealerCsvRow>();
            csv.NextRecord();
            csv.WriteRecords(rows);
        }
    }

    public class ToyotaDealerApiResponse
    {
        [JsonPropertyName("dealers")]
        public List<ToyotaDealer> Dealers { get; set; } = new();
    }

    public class ToyotaDealer
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public ToyotaAddress Address { get; set; } = new();

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = string.Empty;

        // New bits ↓↓↓
        [JsonPropertyName("localDealerID")]
        public string LocalDealerId { get; set; } = string.Empty;

        [JsonPropertyName("t1Url")]
        public string T1Url { get; set; } = string.Empty;

        // Different APIs sometimes use different keys for the same lat/lon object.
        [JsonPropertyName("geo")]
        public ToyotaGeo? Geo { get; set; }

        [JsonPropertyName("coordinates")]
        public ToyotaGeo? Coordinates { get; set; }

        [JsonPropertyName("location")]
        public ToyotaGeo? Location { get; set; }
    }

    public class ToyotaGeo
    {
        [JsonPropertyName("lat")]
        public double? Lat { get; set; }

        [JsonPropertyName("lon")]
        public double? Lon { get; set; }
    }

    public class ToyotaAddress
    {
        [JsonPropertyName("address1")]
        public string AddressLine1 { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("zip")]
        public string Postcode { get; set; } = string.Empty;

        [JsonPropertyName("region")]
        public string Region { get; set; } = string.Empty;
    }

    public class ToyotaDealerCsvRow
    {
        [Name("Dealer Name")]
        public string Name { get; set; } = string.Empty;

        [Name("Street Address")]
        public string AddressLine { get; set; } = string.Empty;

        [Name("City")]
        public string City { get; set; } = string.Empty;

        [Name("Postal Code")]
        public string Postcode { get; set; } = string.Empty;

        [Name("Region")]
        public string Region { get; set; } = string.Empty;

        [Name("Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Name("Dealer ID")]
        public string DealerId { get; set; } = string.Empty;

        [Name("Dealer URL")]
        public string DealerUrl { get; set; } = string.Empty;

        [Name("Latitude")]
        public double? Latitude { get; set; }

        [Name("Longitude")]
        public double? Longitude { get; set; }
    }
}
