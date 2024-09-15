// Create Named Pipe Client
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace NamedPipeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var random = new Random(DateTime.Now.Millisecond);

            while (true)
            {
                var thread = new Thread(WorkerThread);
                thread.Start();

                Thread.Sleep(random.Next(1, 100));
            }
        }

        private static void WorkerThread(object? obj)
        {
            // Create named pipe client
            using var pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut);

            // Connect to the server
            pipeClient.Connect();

            var taskId = Guid.NewGuid();

            // Send request to the server
            StreamWriter writer = new StreamWriter(pipeClient);
            Console.WriteLine($"{taskId}: Send");
            writer.WriteLine($"{taskId}");
            writer.Flush();

            // Read response from the server
            StreamReader reader = new StreamReader(pipeClient);
            string response = reader.ReadLine()!;
            Console.WriteLine($"{taskId}: Response {response}");
        }
    }
}