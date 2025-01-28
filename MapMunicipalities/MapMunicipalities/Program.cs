using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using MapMunicipalities.Models;

class Program
{
    static async Task Main(string[] args)
    {
        var inputFilePath = "addresses.csv";
        var outputFilePath = "addresses_with_municipality.csv";

        var addresses = ReadAddressesFromCsv(inputFilePath);
        var httpClient = new HttpClient();

        List<AddressWithMunicipalityRecord> fullRecords = new List<AddressWithMunicipalityRecord>();
        foreach (var addressRecord in addresses)
        {
            var countySubdivision = await GetCountySubdivisionAsync(httpClient, addressRecord.Address);
            AddressWithMunicipalityRecord fullRecord = new AddressWithMunicipalityRecord(addressRecord.Address, countySubdivision);
            fullRecords.Add(fullRecord);
        }

        WriteAddressesToCsv(outputFilePath, fullRecords);
    }

    static List<AddressRecord> ReadAddressesFromCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader);
        return csv.GetRecords<AddressRecord>().ToList();
    }

    static async Task<string> GetCountySubdivisionAsync(HttpClient httpClient, string address)
    {
        var requestUri = $"https://geocoding.geo.census.gov/geocoder/geographies/onelineaddress?address={Uri.EscapeDataString(address)}&benchmark=Public_AR_Current&vintage=Current_Current&format=json";
        var response = await httpClient.GetStringAsync(requestUri);
        var jsonDoc = JsonDocument.Parse(response);

        var countySubdivision = jsonDoc.RootElement
            .GetProperty("result")
            .GetProperty("addressMatches")[0]
            .GetProperty("geographies")
            .GetProperty("County Subdivisions")[0]
            .GetProperty("NAME")
            .GetString();

        return countySubdivision;
    }

    static void WriteAddressesToCsv(string filePath, List<AddressWithMunicipalityRecord> addresses)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer);
        csv.WriteRecords(addresses);
    }
}
