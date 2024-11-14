namespace GreenGeo;

using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class GeoServer
{
    private readonly int _port;

    public GeoServer(int port)
    {
        _port = port;
    }

    public async Task StartAsync()
    {
        var server = new TcpListener(IPAddress.Loopback, _port);
        server.Start();
        Console.WriteLine("Geo server is running...");

        while (true)
        {
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");
            _ = Task.Run(() => HandleClientAsync(client));
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        using (var networkStream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received: {message}");

            var parts = message.Split('|');
            if (parts.Length == 4)
            {
                string country1 = parts[0];
                string postalCode1 = parts[1];
                string country2 = parts[2];
                string postalCode2 = parts[3];

                var calculator = new LocationDistanceCalculator();
                try
                {
                    double distance = await calculator.GetDistanceBetweenPostalCodesAsync(country1, postalCode1, country2, postalCode2);
                    byte[] responseBytes = Encoding.UTF8.GetBytes(distance.ToString(CultureInfo.InvariantCulture));
                    await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine("Response sent.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
