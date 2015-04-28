using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;



namespace Riken
{

    class KinectServer
    {
        

        static void PostureServer(string[] args)
        {
          

            Microsoft.Kinect.Body body = null;
            long timestamp = 0;

            object mutex = new object();

            var kinect = new Thread(delegate()
            {
                var source = Kinect.BodyFrames();

                var tracked = from x in Kinect.Bodies(source)
                              where x != null
                              select new { bodies = x.bodies.Tracked(), timestamp = x.timestamp};

                var first = from x in tracked
                            where x.bodies.Length > 0
                            select new { body = x.bodies.First(), timestamp = x.timestamp };

                foreach (var it in first)
                {
                    lock (mutex)
                    {
                        body = it.body;
                        timestamp = it.timestamp;
                    }
                }


            });

            kinect.Start();

            try
            {

                var serializer = new Serializer();

				Console.WriteLine("public ip: {0}", Server.IpAddress());
                var server = new Server();

                int frame = 0;
                foreach (var client in server.Clients())
                {
                    
                    string json = null;

                    while (true)
                    {
                        lock (mutex)
                        {
                            if (body != null)
                            {
                                json = serializer.Serialize(body, timestamp);
                            }
                        }

                        try
                        {
                            // Console.WriteLine("sending");
                            if (json != null)
                            {
                                client.Send(json + '\n');
                                // Console.WriteLine("waiting ack");
                                var ack = client.Recv();
                                Console.WriteLine("frame {0}", frame++);
                            }
                        }
                        catch (IOException)
                        {
                            Console.WriteLine("write error");
                            break;
                        }
                    }
                }

            }

            finally
            {
                kinect.Abort();
                kinect.Join();
            }
        }

       
        static void Main(string[] args)
        {
            // KinectTest(args);
            // ServerTest(args);
            PostureServer(args);
            // Recorder(args);
        }

    }
}
