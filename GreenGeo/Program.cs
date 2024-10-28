using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var server = new TcpListener(IPAddress.Loopback, 5001);
        server.Start();
        Console.WriteLine("Geo server is running...");

        while (true)
        {
            var client = await server.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");

            _ = Task.Run(() => HandleClient(client));
        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        using (client)
        using (var networkStream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received: {message}");

            //format: "Country1|PostalCode1|Country2|PostalCode2")
            var parts = message.Split('|');
            if (parts.Length == 4)
            {
                string country1 = parts[0];
                string postalCode1 = parts[1];
                string country2 = parts[2];
                string postalCode2 = parts[3];
                
                double distance = CalculateDistance(country1, postalCode1, country2, postalCode2);
                byte[] responseBytes = Encoding.UTF8.GetBytes(distance.ToString(CultureInfo.InvariantCulture));
                await networkStream.WriteAsync(responseBytes, 0, responseBytes.Length);

                Console.WriteLine("Response sent.");
            }
        }
    }

    private static double CalculateDistance(string country1, string postalCode1, string country2, string postalCode2)
    {
        // Placeholder for google maps service
        return 123.45;
    }
}
