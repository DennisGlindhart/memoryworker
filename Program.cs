using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace memoryworker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }

    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        Dictionary<int,string> data = new Dictionary<int, string>();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                
                _logger.LogInformation("Worker running at: {time} " + i.ToString(), DateTimeOffset.Now);
                try {

                    data[i] = RandomString(102400);
                } catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }

                if (i % 5 == 0 && i > 0) {
                    //Just use some of the data so GC doesn't say "Hey - it will never be used, so lets flush it"- Don't make sense of it :)
                    foreach (var dat in data.TakeLast(1))
                    {
                        _logger.LogInformation(dat.Value[0] + "" + dat.Value[1]);
                    }
                }
                i++;
                await Task.Delay(1000, stoppingToken);
                
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}


