using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Receiver.model;
using System.Collections.Generic;

namespace Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            PLayerStats _stats = new PLayerStats();
            
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "CVstats",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                        
                    Console.WriteLine(" [x] Received {0}", message);
                    _stats.points.Add(message);
                };
                channel.BasicConsume(queue: "CVstats",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
