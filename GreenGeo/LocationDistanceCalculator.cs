namespace GreenGeo;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class LocationDistanceCalculator
{
    private const string GoogleGeocodingApiKey = "AIzaSyB1F32YxHGafTJzkI3vLHd6VR-IzV6FcCs";

    public async Task<(double latitude, double longitude)> GetCoordinatesAsync(string country, string postalCode)
    {
        string url = $"https://maps.googleapis.com/maps/api/geocode/json?components=country:{country}|postal_code:{postalCode}&key={GoogleGeocodingApiKey}";

        using HttpClient client = new HttpClient();
        string response = await client.GetStringAsync(url);
        var json = JObject.Parse(response);

        if (json["status"].ToString() == "OK")
        {
            var location = json["results"][0]["geometry"]["location"];
            double latitude = (double)location["lat"];
            double longitude = (double)location["lng"];
            return (latitude, longitude);
        }

        throw new Exception("Unable to get coordinates for the specified location.");
    }

    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;

        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private double DegreesToRadians(double degrees) => degrees * Math.PI / 180;

    public async Task<double> GetDistanceBetweenPostalCodesAsync(string country1, string postalCode1, string country2, string postalCode2)
    {
        var coords1 = await GetCoordinatesAsync(country1, postalCode1);
        var coords2 = await GetCoordinatesAsync(country2, postalCode2);

        return CalculateDistance(coords1.latitude, coords1.longitude, coords2.latitude, coords2.longitude);
    }
}