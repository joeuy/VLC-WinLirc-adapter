using LircVlc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;

namespace LircVlc
{
    class Program
    {
        const int COMMAND_INTERVAL = 300 * 10000;

        static void Main(string[] args)
        {
            //read configuration
            var config = ReadConfig();
            var commands = ReadCommands();

            //act
            while (true)
            {
                try
                {
                    RelayCommands(config, commands);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("# Restarting...");
                    Thread.Sleep(5 * 1000);
                }
            }
        }

        private static void RelayCommands(Config config, IEnumerable<Command> commands)
        {
            using (var client = new TcpClient())
            {
                client.Connect(config.Lirc);
                Console.WriteLine("# Connected to winlirc");

                using (var socketStream = client.GetStream())
                using (var stream = new StreamReader(socketStream))
                using (var http = new HttpClient())
                {
                    while (client.Connected && socketStream.CanRead && !stream.EndOfStream)
                    {
                        var data = stream.ReadLine().Split(' ');
                        var cmd = commands.FirstOrDefault(c => c.Button == data[2] && c.Remote == data[3]);

                        if (cmd != null && DateTime.Now.Ticks - cmd.LastSeen > COMMAND_INTERVAL)
                        {
                            Console.WriteLine("Sending VLCCommand: {0}, button: {1}, remote: {2}", cmd.VLCCommand, cmd.Button, cmd.Remote);
                            cmd.LastSeen = DateTime.Now.Ticks;
                            var target = string.Format(config.Vlc, cmd.VLCCommand);
                            http.GetAsync(target);
                        }
                    }
                }
            }
        }

        private static IEnumerable<Command> ReadCommands()
        {
            IEnumerable<Command> result = null;
            try
            {
                result = File.ReadAllLines("commands.txt")
                    .Where(line => !line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                    .Select(line => line.Split(' '))
                    .Select(tokens => new Command() { Remote = tokens[0], Button = tokens[1], VLCCommand = tokens[2] })
                    .ToList();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("EXCEPTION: commands.txt file not found make sure its on the same dir as the LircVlc.exe");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("EXCEPTION: commands.txt wrong format. make sure its something like remote_name button vlccommand");
            }

            return result;
        }

        private static Config ReadConfig()
        {
            Config result = null;
            try
            {
                var lines = File.ReadAllLines("config.txt")
                    .Select(line => line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                    .ToDictionary(tokens => tokens[0], tokens => tokens[1]);

                var lirc = lines["lirc"].Split(':');

                result = new Config()
                {
                    Lirc = new IPEndPoint(IPAddress.Parse(lirc[0]), int.Parse(lirc[1])),
                    Vlc = lines["vlc"]
                };
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("EXCEPTION: config.txt file not found make sure its on the same dir as the LircVlc.exe");
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("EXCEPTION: config.txt must contain a line for vlc (i.e. vlc http://127.0.0.1:8080/requests/status.xml?command={0}) and a line for lirc (i.e. lirc 127.0.0.1:8765)");
            }
            catch (FormatException)
            {
                Console.WriteLine("EXCEPTION: invalid ip address or port number");
            }

            return result;
        }
    }
}
