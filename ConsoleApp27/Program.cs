using System.Net;
using System.Net.Sockets;
using System.Text;
namespace ConsoleApp27
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int port = 5000;
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Waiting for two players...");

            TcpClient player1 = listener.AcceptTcpClient();
            Console.WriteLine("Player 1 connected.");
            TcpClient player2 = listener.AcceptTcpClient();
            Console.WriteLine("Player 2 connected.");

            NetworkStream stream1 = player1.GetStream();
            NetworkStream stream2 = player2.GetStream();

            int score1 = 0, score2 = 0;
            for (int round = 1; round <= 5; round++)
            {
                Send(stream1, $"Round {round}: Enter your move (rock/paper/scissors):");
                Send(stream2, $"Round {round}: Enter your move (rock/paper/scissors):");

                string move1 = Receive(stream1);
                string move2 = Receive(stream2);

                string result = GetRoundResult(move1, move2, out int winner);
                if (winner == 1) score1++;
                else if (winner == 2) score2++;

                Send(stream1, $"You: {move1}, Opponent: {move2}. {result} | Score: {score1}-{score2}");
                Send(stream2, $"You: {move2}, Opponent: {move1}. {result} | Score: {score2}-{score1}");
            }

            string finalResult = score1 == score2 ? "Draw!" : (score1 > score2 ? "You win!" : "You lose!");
            Send(stream1, $"Game over! Final score: {score1}-{score2}. {finalResult}");
            Send(stream2, $"Game over! Final score: {score2}-{score1}. {(score2 > score1 ? "You win!" : (score2 == score1 ? "Draw!" : "You lose!"))}");

            player1.Close();
            player2.Close();
            listener.Stop();
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
            return Encoding.UTF8.GetString(buffer, 0, bytes).Trim().ToLower();
        }

        static string GetRoundResult(string move1, string move2, out int winner)
        {
            if (move1 == move2)
            {
                winner = 0;
                return "Draw!";
            }
            if ((move1 == "rock" && move2 == "scissors") ||
                (move1 == "scissors" && move2 == "paper") ||
                (move1 == "paper" && move2 == "rock"))
            {
                winner = 1;
                return "You win this round!";
            }
            winner = 2;
            return "You lose this round!";
        }
    }
}
