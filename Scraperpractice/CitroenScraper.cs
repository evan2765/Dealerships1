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
    public class CitroenScraper
    {
        public static async Task RunAsync()
        {
            var apiUrl = "https://www.citroen.co.uk/apps/atomic/DealersServlet?distance=300&latitude=52.77206&longitude=-0.29095&maxResults=100&orderResults=false&path=L2NvbnRlbnQvY2l0cm9lbi93b3JsZHdpZGUvdWsvZW4%3D&searchType=latlong";

            var apiResponse = await ScrapeApiAsync<CitroenApiResponse>(apiUrl);

            if (apiResponse?.Payload?.Dealers is { Count: > 0 })
            {
                var rows = new List<CitroenDealerCsvRow>();

                foreach (var dealer in apiResponse.Payload.Dealers)
                {
                    var row = new CitroenDealerCsvRow
                    {
                        Name = dealer.DealerName,
                        AddressLine1 = dealer.Address?.AddressLine1 ?? "",
                        City = dealer.Address?.CityName ?? "",
                        County = dealer.Address?.County ?? "",
                        Postcode = dealer.Address?.PostalCode ?? "",
                        Phone = dealer.GeneralContact?.Phone1 ?? "",
                        Latitude = dealer.Geolocation?.Latitude ?? "",
                        Longitude = dealer.Geolocation?.Longitude ?? "",
                        WebsiteUrl = !string.IsNullOrWhiteSpace(dealer.DealerUri)
                            ? dealer.DealerUri!
                            : (dealer.DealerUrl ?? "")
                    };

                    rows.Add(row);

                    Console.WriteLine(
                        $"Citroen: {row.Name} | {row.AddressLine1} | {row.City} | {row.County} | {row.Postcode} | {row.Phone} | {row.Latitude},{row.Longitude} | {row.WebsiteUrl}"
                    );
                }

                ExportToCsv(rows, "Citroen_Dealerships.csv");
            }
            else
            {
                Console.WriteLine("❌ No Citroen dealers found or response was invalid.");
            }
        }

        private static async Task<T?> ScrapeApiAsync<T>(string url)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/124.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            try
            {
                var response = await client.GetAsync(url);
                var responseText = await response.Content.ReadAsStringAsync();
                File.WriteAllText("citroen_dealers_raw.json", responseText);

                if (!response.IsSuccessStatusCode || string.IsNullOrWhiteSpace(responseText))
                    return default;

                return JsonSerializer.Deserialize<T>(responseText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error scraping Citroen dealers: {ex.Message}");
                return default;
            }
        }

        private static void ExportToCsv(List<CitroenDealerCsvRow> rows, string fileName)
        {
            using var writer = new StreamWriter(fileName);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<CitroenDealerCsvRow>();
            csv.NextRecord();
            csv.WriteRecords(rows);
        }
    }

    // --------- Models for Citroen ------------

    public class CitroenApiResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("payload")]
        public CitroenPayload Payload { get; set; } = new();
    }

    public class CitroenPayload
    {
        [JsonPropertyName("dealers")]
        public List<CitroenDealer> Dealers { get; set; } = new();
    }

    public class CitroenDealer
    {
        [JsonPropertyName("dealerName")]
        public string DealerName { get; set; } = string.Empty;

        // Website URL (seen as dealerUri in payload; sometimes dealerUrl)
        [JsonPropertyName("dealerUri")]
        public string? DealerUri { get; set; }

        [JsonPropertyName("dealerUrl")]
        public string? DealerUrl { get; set; }

        [JsonPropertyName("geolocation")]
        public DealerGeolocation? Geolocation { get; set; }

        [JsonPropertyName("generalContact")]
        public GeneralContact? GeneralContact { get; set; }

        [JsonPropertyName("address")]
        public DealerAddress? Address { get; set; }
    }

    public class DealerGeolocation
    {
        [JsonPropertyName("latitude")]
        public string Latitude { get; set; } = string.Empty;

        [JsonPropertyName("longitude")]
        public string Longitude { get; set; } = string.Empty;
    }

    public class GeneralContact
    {
        [JsonPropertyName("phone1")]
        public string Phone1 { get; set; } = string.Empty;
    }

    public class DealerAddress
    {
        [JsonPropertyName("addressLine1")]
        public string AddressLine1 { get; set; } = string.Empty;

        [JsonPropertyName("cityName")]
        public string CityName { get; set; } = string.Empty;

        [JsonPropertyName("county")]
        public string County { get; set; } = string.Empty;

        [JsonPropertyName("postalCode")]
        public string PostalCode { get; set; } = string.Empty;
    }

    public class CitroenDealerCsvRow
    {
        // CSV columns
        public string Name { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string Latitude { get; set; } = string.Empty;
        public string Longitude { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
    }
}
