using System;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace Riken
{
    class Server
    {
        public string ip = IPAddress.Any.ToString();
        ushort port = 9001;

        TcpListener listener;

		// in case you need the public IP address
        static public string IpAddress()
        {
            var entry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            return entry.AddressList.Where(
                (ip) => ip.AddressFamily == AddressFamily.InterNetwork).First().ToString();
        }

        public class Client : IDisposable
        {
            public TcpClient tcp;
            NetworkStream stream;

            static ushort buffer_length = 4096;
            byte[] buffer = new byte[buffer_length];

            Encoding encoder = new UTF8Encoding();

            public Client(TcpClient tcp)
            {
                this.tcp = tcp;
                this.stream = tcp.GetStream();
            }

            public virtual void Dispose() { tcp.Close(); }



            public string Recv()
            {
                // if (poll && !stream.DataAvailable) return null;
                int read = stream.Read(buffer, 0, buffer.Length);
                return encoder.GetString(buffer, 0, read);
            }

            public void Send(string msg)
            {
                byte[] buf = encoder.GetBytes(msg);
                stream.Write(buf, 0, buf.Length);
                stream.Flush();
            }

           
        };

        // enumerates sequential clients
        public IEnumerable<Client> Clients()
        {
            Start();
            try
            {
                while (true)
                {
                    Console.WriteLine("waiting for client...");
                    using (Client client = new Client(listener.AcceptTcpClient()))
                    {
                        Console.WriteLine("client connected");
                        yield return client;
                    }
                    Console.WriteLine("client disconnected");
                }

            }
            finally
            {
                Stop();
            }

        }





        void Start()
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            listener.Start();

            Console.WriteLine("server started (ip: {0}, port: {1})", ip, port);
        }




        void Stop()
        {
            listener.Stop();
            Console.WriteLine("server stopped");
        }



    }
}