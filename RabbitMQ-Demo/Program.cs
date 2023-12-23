using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Program
{
    static void Main()
    {
        
        ProduceMessage();
        ConsumeMessages();
    }

    static void ProduceMessage()
    {
        var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 }; 

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

            string message = "Hello, RabbitMQ";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body);
            Console.WriteLine($" [x] Sent '{message}'");
        }
    }

    static void ConsumeMessages()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };

        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {

            channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

      
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");

           
            var consumer = new EventingBasicConsumer(channel);

          
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received '{message}'");
            };

         
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
