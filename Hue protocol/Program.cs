using System;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.Logging;
using HueProtocol.Services;
using HueProtocol.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Victoria;

namespace HueProtocol
{
    internal sealed class Program
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Configuration _configuration;

        public Program()
        {

            _configuration = Configuration.IsCreated
                ? Configuration.Load()
                : Configuration.Create();

            var services = new ServiceCollection()
                .AddSingleton(_configuration)
                .AddSingleton(new DiscordShardedClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Verbose,
                    CaseSensitiveCommands = false,
                    ThrowOnError = false
                }))
                .AddLogging(x =>
                {
                    x.ClearProviders();
                    x.AddProvider(new LoggerProvider());
                })
                .AddServicesOfT<IInjectableService>()
                .AddSingleton(new HttpClient())
                .AddDbContext<EFContext>()
                .AddLavaNode();

            _serviceProvider = services.BuildServiceProvider();
        }

        private static async Task Main()
        {
            try
            {
                Extensions.PrintHeader();
                await new Program().StartAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async Task StartAsync()
        {
            //load command service
            var commandService = _serviceProvider.GetRequiredService<CommandService>();
            await commandService.AddModulesAsync(typeof(Program).Assembly, _serviceProvider);

            //connect to API
            var shardedClient = _serviceProvider.GetRequiredService<DiscordShardedClient>();

            //Load features
            //Note : AudioService is called by Audio (ModuleBase)
            _serviceProvider.GetRequiredService<UserInteractions>();
            _serviceProvider.GetRequiredService<EventService>();
            _serviceProvider.GetRequiredService<RandomReactions>();
            _serviceProvider.GetRequiredService<SeedDatabase>();
            
            await shardedClient.LoginAsync(TokenType.Bot, _configuration.Token);
            await shardedClient.StartAsync();
            await Task.Delay(-1);
        }
    }
}