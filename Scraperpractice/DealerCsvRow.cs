using CsvHelper.Configuration.Attributes;

namespace scraper_practice
{
    public class DealerCsvRow
    {
        [Name("Dealer Name")]
        public string Name { get; set; } = string.Empty;

        [Name("Address Line 1")]
        public string AddressLine1 { get; set; } = string.Empty;

        [Name("Town/City")]
        public string TownCity { get; set; } = string.Empty;

        [Name("County")]
        public string County { get; set; } = string.Empty;

        [Name("Postal Code")]
        public string Postcode { get; set; } = string.Empty;

        [Name("Phone Number")]
        public string Phone { get; set; } = string.Empty;
    }
}
