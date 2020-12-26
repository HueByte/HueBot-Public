using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Services
{
    internal class RandomReactions : IInjectableService
    {
        private readonly CommandService _commands;
        private readonly DiscordShardedClient _client;
        private readonly ILogger<RandomReactions> _logger;
        private readonly IServiceProvider _services;

        public RandomReactions(IServiceProvider services, CommandService commands, DiscordShardedClient client,
                               ILogger<RandomReactions> logger)
        {
            _services = services;
            _commands = commands;
            _client = client;
            _logger = logger;
            _client.MessageReceived += FunnyReaction;

            _logger.LogInformation("RandomReactionsService has been loaded");
        }

        public async Task FunnyReaction(SocketMessage msg)
        {
            //Check if system message
            if (!(msg is SocketUserMessage message))
                return;

            //check if user is the source of msg
            if (message.Source != Discord.MessageSource.User)
                return;

            //Get emotes 
            var emote = await GetEmote("49");

            if (msg.Channel.Id == 347154413124583424)
                if (msg.Content.Length == 0)
                {
                    await msg.AddReactionAsync(emote);
                    Console.WriteLine("Pic Detected");
                }

            #region Fun responses

            if (msg.Content.ToLower() == "reich")
                await msg.Channel.SendMessageAsync("Oh not again...");

            else if (msg.Content.ToLower().Contains("omg"))
                await msg.Channel.SendMessageAsync("No way D:");

            string[] lewdWords = new string[]
            {
                "lewd", "panties", "yaoi", "porn", "hentai"
            };

            if (lewdWords.Any(x => x.Equals(msg.Content, StringComparison.OrdinalIgnoreCase)))
                await msg.Channel.SendMessageAsync("( ͡° ͜ʖ ͡°)");

            #endregion
        }

        public async Task<GuildEmote> GetEmote(string name)
        {
            var emote = _client.Guilds
                .SelectMany(x => x.Emotes)
                .FirstOrDefault(x => x.Name.IndexOf(
                    name, StringComparison.OrdinalIgnoreCase) != -1);
            return emote;
        }
    }
}
