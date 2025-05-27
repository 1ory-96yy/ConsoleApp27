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

                if (serverMsg.Contains("Game ended") || serverMsg.StartsWith("Game over!") || serverMsg.Contains("You win!") || serverMsg.Contains("You lose!") || serverMsg.Contains("Draw!"))
                    break;

                if (serverMsg.Contains("Enter your move") || serverMsg.Contains("Opponent offers a draw"))
                {
                    string input = Console.ReadLine();
                    Send(stream, input);
                }
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
            StringBuilder sb = new StringBuilder();
            byte[] buffer = new byte[256];
            int bytes;
            do
            {
                bytes = stream.Read(buffer, 0, buffer.Length);
                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
            }
            while (stream.DataAvailable);

            return sb.ToString().Trim();
        }
    }
}
