using System;
using System.Threading.Tasks;
using GreenGeo;

public class Program
{
    public static async Task Main(string[] args)
    {
        var geoServer = new GeoServer(5001);
        await geoServer.StartAsync();
    }
}