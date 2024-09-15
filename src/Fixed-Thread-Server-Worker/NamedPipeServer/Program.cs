// Create named pipe server
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace NamedPipeServer
{
    class Program
    {
        static int workers = 10;
        static bool run = true;

        static void Main(string[] args)
        {
            var threads = Enumerable
                            .Range(0, workers)
                            .Select(i =>
                            {
                                var thread = new Thread(ServerThread);
                                thread.Start(i);

                                Console.WriteLine($"Thread {i} started.");

                                return thread;
                            })
                            .ToList();

            while (true)
            {
                if (!threads.Any(x => x.IsAlive))
                    break;

                Thread.Sleep(1000);
            }

            Console.WriteLine("All threads are done.");
        }

        private static void ServerThread(object? obj)
        {
            var workerId = (int)obj!;
            var random = new Random(DateTime.Now.Millisecond);

            while (run)
            {
                var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut, workers);
                pipeServer.WaitForConnection();

                try
                {

                    using StreamReader reader = new StreamReader(pipeServer);
                    string request = reader.ReadLine()!;
                    Console.WriteLine($"Worker {workerId}: Get {request}");

                    // Process
                    Thread.Sleep(random.Next(200, 3000));

                    // Send response to the client
                    using StreamWriter writer = new StreamWriter(pipeServer);
                    writer.WriteLine(request);
                    writer.Flush();

                    Console.WriteLine($"Worker {workerId}: Responsed");

                }
                catch (IOException ex) when (ex.Message.Contains("Broken pipe")
                                             || ex.Message.Contains("Pipe is broken"))
                {
                    Console.WriteLine($"Worker {workerId}: {ex.Message}");
                }
                finally
                {
                    if (pipeServer.IsConnected)
                    {
                        pipeServer.Disconnect();
                        pipeServer.Close();
                    }
                }
            }
        }

    }
}

