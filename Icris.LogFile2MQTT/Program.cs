using Icris.FormatDetectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Icris.LogFile2MQTT
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: LogFile2MQTT [brokeraddress] [topic] [interval] [logfile]");
                Console.WriteLine("Brokeraddress: ip or hostname of the MQTT broker");
                Console.WriteLine("Topic: Topic of the messages to be sent");
                Console.WriteLine("Interval: delay in milliseconds between sending the messages");
                Console.WriteLine("Logfile: the CSV file containing the messages (one per line, including header for field names)");
                return;
            }
            var brokeraddress = args[0];
            var topic = args[1];
            var interval = args[2];
            var logfile = args[3];

            MqttClient client = new MqttClient(brokeraddress);

            string clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);

            var detector = new CSVDetector(logfile);
            Console.WriteLine("Found headers:");
            detector.Headers.ForEach(x => Console.WriteLine(x));
            Console.WriteLine("Starting message sending in 3 seconds...");
            Thread.Sleep(3000);
            Console.Clear();
            Console.WriteLine("Messages sent:");
            var record = detector.Rows.FirstOrDefault();
            var counter = 0;
            while (record != null)
            {
                //Console.WriteLine(record.ToString());
                Thread.Sleep(int.Parse(interval));
                record = detector.Rows.Skip(counter).Take(1).FirstOrDefault();
                client.Publish(topic, System.Text.UTF8Encoding.UTF8.GetBytes(record.ToString()));
                counter++;
                Console.CursorLeft = 1;
                Console.CursorTop = 1;
                Console.Write(counter);
            }
            Console.ReadLine();
        }
    }
}
