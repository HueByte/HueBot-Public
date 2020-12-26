using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.models;
using HueProtocol.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Services
{
    public class UserInteractions : IInjectableService {
        private EFContext _context;
        private ILogger<UserInteractions> _logger;
        private MiscDatabaseMethodsService _misc;

        public UserInteractions(EFContext context, DiscordShardedClient client, ILogger<UserInteractions> logger, MiscDatabaseMethodsService misc)
        {
            _context = context;
            _logger = logger;
            client.UserJoined += Greeting;
            _misc = misc;
            _logger.LogInformation("UserInteractionsService has been loaded");
        }

        public async Task Greeting(SocketGuildUser user)
        {
            var server = await _misc.GetOrCreateServer(user.Guild.Id);
            if (server.DoGreet)
            {
                ISocketMessageChannel msgChannel = null;
                if (server.GreetingRoom != 0)
                    msgChannel = user.Guild.GetChannel(server.GreetingRoom) as ISocketMessageChannel;

                else
                    msgChannel = user.Guild.DefaultChannel as ISocketMessageChannel;

                var embed = new EmbedBuilder();
                embed.Title = $"{user.Username} has joined {user.Guild.Name}!";
                embed.Description = server.GreetingMsg.Replace("#user", $"<@{user.Id}>");
                embed.ThumbnailUrl = user.GetAvatarUrl();
                embed.WithColor(Color.Green);

                await msgChannel.SendMessageAsync("", false, embed.Build());
                
                _logger.LogInformation($"New user {user.Id} in {server.ServerId}");
            }
            else
                return;
        }
    }
}
