using System;
using System.Text;
using Confluent.Kafka;
using Kafka.Public;
using Kafka.Public.Loggers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KafkaIntegration
{
    public class Name
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("hello world");
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, collection) =>
        {
            collection.AddHostedService<KafkaConsumer>();
            collection.AddHostedService<kafkaProducer>();
        });



    }

    public class kafkaProducer : IHostedService
    {

        private readonly ILogger<kafkaProducer> _logger;
        private readonly IProducer<Null, string> _producer;
        public kafkaProducer(ILogger<kafkaProducer> logger)
        {
            this._logger = logger;
            var config = new ProducerConfig()
            {
                BootstrapServers = "localhost:9092"
            };
            _producer = new ProducerBuilder<Null, string>(config).Build();

        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            for (var i = 0; i < 100; i++)
            {
                var value = $"hello number {i}";
                _logger.LogInformation(value);

                await _producer.ProduceAsync("demo", new Message<Null, string>()
                {
                    Value = value

                }, cancellationToken);
            }
            _producer.Flush(TimeSpan.FromSeconds(10));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _producer?.Dispose();
            return Task.CompletedTask;
        }
    }


    public class KafkaConsumer : IHostedService
    {
        private readonly ILogger<KafkaConsumer> _logger;
        private ClusterClient _cluster;

        public KafkaConsumer(ILogger<KafkaConsumer> logger)
        {
            this._logger = logger;
            _cluster = new ClusterClient(new Configuration
            {
                Seeds = "localhost:9092"
            }, new ConsoleLogger());
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cluster.ConsumeFromLatest("demo");
            _cluster.MessageReceived += record =>
            {
                _logger.LogInformation($"got it {Encoding.UTF8.GetString(record?.Value as byte[])}");
            };

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cluster?.Dispose();
            return Task.CompletedTask;
        }
    }


}