using System.Net.Sockets;
using System.Text;
namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string serverIp = "127.0.0.1";
            const int port = 5000;
            TcpClient client = new TcpClient();
            client.Connect(serverIp, port);
            NetworkStream stream = client.GetStream();

            while (true)
            {
                string serverMsg = Receive(stream);
                Console.WriteLine(serverMsg);
                if (serverMsg.StartsWith("Game over!")) break;

                string input = Console.ReadLine();
                Send(stream, input);
            }

            client.Close();
        }

        static void Send(NetworkStream stream, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
        }

        static string Receive(NetworkStream stream)
        {
            byte[] buffer = new byte[256];
            int bytes = stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytes).Trim();
        }
    }
}
