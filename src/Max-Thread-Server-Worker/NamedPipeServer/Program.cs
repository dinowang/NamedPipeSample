// Create named pipe server
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;

namespace NamedPipeServer
{
    class Program
    {
        static bool run = true;

        static int counter = 0;

        static void Main(string[] args)
        {
            var random = new Random(DateTime.Now.Millisecond);

            while (run)
            {
                var pipeServer = new NamedPipeServerStream("testpipe", PipeDirection.InOut, 254);
                pipeServer.WaitForConnection();

                var thread = new Thread(() =>
                {
                    var workerId = Interlocked.Increment(ref counter);
                    var threadPipeServer = pipeServer;

                    try
                    {
                        using StreamReader reader = new StreamReader(threadPipeServer);
                        string request = reader.ReadLine()!;
                        Console.WriteLine($"Worker {workerId}: Get {request}");

                        // Process
                        Thread.Sleep(random.Next(200, 3000));

                        // Send response to the client
                        using StreamWriter writer = new StreamWriter(threadPipeServer);
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
                        if (threadPipeServer.IsConnected)
                        {
                            threadPipeServer.Disconnect();
                            threadPipeServer.Close();
                            threadPipeServer = null;
                        }
                    }
                });

                thread.Start();
            }
        }
    }
}

