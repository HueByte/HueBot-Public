using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Services
{
    //Discord client events:
    //- class Command Handler
    //> MessageReceived (Command handler)
    //> ReactionAdded
    //> ReactionRemoved
    //> JoinedGuild
    //- class UserInteractions
    //> UserJoined
    //- class Audio
    //> OnReady
    //- class RandomReactions
    //> MessagReceived 
    //- class LoggingService
    //> Log
    public class CommandHandler : IInjectableService
    {
        private readonly CommandService _commands;
        private readonly DiscordShardedClient _client;
        private readonly ILogger<CommandHandler> _logger;
        private readonly IServiceProvider _services;
        private readonly EFContext _db;
        private readonly List<ServerGuild> _servers;
        private List<ReactionPost> ReactionPosts;
        private readonly Configuration _configuration;

        public CommandHandler(CommandService commands, DiscordShardedClient client, ILogger<CommandHandler> logger,
                              IServiceProvider services, EFContext db, Configuration configuration)
        {
            _commands = commands;
            _client = client;
            _logger = logger;
            _services = services;
            _db = db;
            _configuration = configuration;
            //client events
            client.MessageReceived += HandleCommand;
            client.ReactionAdded += ListenForAddReaction;
            client.ReactionRemoved += ListenForReactionRemove;
            client.JoinedGuild += OnJoined;

            ReactionPosts = db.ReactionPosts.ToList();
            _servers = db.ServersSettings.ToList();
            _logger.LogInformation("CommandHandler has been loaded");
        }

        public async Task OnJoined(SocketGuild guild)
        {
            var exists = await _db.ServersSettings.AnyAsync(serv => serv.ServerId == guild.Id);

            var newServer = new ServerGuild()
            {
                OwnerId = guild.OwnerId,
                OwnerName = guild.Owner.Username,
                DoGreet = false,
                GreetingMsg = null,
                GreetingRoom = 0,
                RolePostId = 0,
                prefix = '$',
                ServerId = guild.Id,
                ServerName = guild.Name
            };

            //if doesn't exist
            if (exists)
                _db.ServersSettings.Add(newServer);

            //if exist in database but bot wasn't there
            else
                _db.ServersSettings.Update(newServer);

            await _db.SaveChangesAsync();
        }

        public async Task HandleCommand(SocketMessage paramMessage)
        {
            //Check if system message
            var message = paramMessage as SocketUserMessage;
            if (message == null)
                return;

            //check if user is the source of msg
            if (message.Source != MessageSource.User)
                return;

            //starting index of command
            var argumentPos = 0;

            //command context
            var context = new ShardedCommandContext(_client, message);

            if (!(message.HasMentionPrefix(_client.CurrentUser, ref argumentPos) ||
                  message.HasCharPrefix(_configuration.Prefix, ref argumentPos)))
                return;

            //TODO
            //var serverPrefix = GetPrefix((long)context.Guild.Id);

            //Execute 
            var result = await _commands.ExecuteAsync(context, argumentPos, _services);

            await LogCommandUsage(context, result);

            if (!result.IsSuccess)
                if (result.ErrorReason != "Unknown command")
                {
                    await message.Channel.SendMessageAsync($"Error {result.ErrorReason}");
                }
        }

        public async Task updatePosts()
        {
            ReactionPosts = await _db.ReactionPosts.ToListAsync();
            _logger.LogInformation("Listening List Updated");
        }

        private PrefixList GetPrefix(long serverId)
        {
            return null;
            //Left for database usage 
        }

        private async Task LogCommandUsage(SocketCommandContext context, IResult result)
        {
            await Task.Run(() =>
            {
                if (context.Channel is IGuildChannel)
                {
                    var log =
                        $"User: [{context.User.Username}]<->[{context.User.Id}] Discord Server: [{context.Guild.Name}] -> [{context.Message.Content}]";
                    _logger.LogInformation(log);
                }
                else
                {
                    var log = $"User: [{context.User.Username}]<->[{context.User.Id}] -> [{context.Message.Content}]";
                    _logger.LogInformation(log);
                }
            });
        }

        private async Task ListenForAddReaction(Cacheable<IUserMessage, ulong> cachedMessage,
                                                ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            if (reaction.Emote.ToString() == "🤍")
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                //Search for post
                var Post = ReactionPosts.FirstOrDefault(post => post.Id == reaction.MessageId);

                if (Post != null)
                {
                    //return if reaction was made by user
                    if (reaction.User.Value?.IsBot == true)
                        return;

                    //Get the cached message or download it
                    var message = await cachedMessage.DownloadAsync();

                    //Create EmbedBuilder object
                    var embed = message.Embeds.First().ToEmbedBuilder();

                    //make offset for fields
                    var postOffset = 2;

                    //return if post achieved the max user count
                    if ((embed.Fields.Count - 2) >= Post.UserMax)
                        return;

                    //remove field with UserId to make sure it isn't duplicate
                    embed.Fields.RemoveAll(user => user.Value.ToString() == $"<@{reaction.UserId}>");

                    //Add user to list
                    embed.AddField($"{embed.Fields.Count - postOffset + 1}", $"<@{reaction.UserId}>", true);

                    //Modify the embed
                    await message.ModifyAsync(msg => msg.Embed = embed.Build());
                    watch.Stop();
                    _logger.LogInformation($"UserReactionAdded with type [{Post.Type}] by [{reaction.User}] - {reaction.Channel} in {watch.Elapsed}");
                }

                else if (_servers.FirstOrDefault(item => item.RolePostId == reaction.MessageId) != null)
                {
                    var guild = _client.GetGuild(_servers
                        .FirstOrDefault(item => item.RolePostId == reaction.MessageId).ServerId);
                    var role = guild.Roles.FirstOrDefault(role => role.Name == "LFP");
                    var socketUser = guild.GetUser(reaction.UserId);
                    if (socketUser?.IsBot == true)
                        return;
                    await socketUser.AddRoleAsync(role);
                    
                    watch.Stop();
                    _logger.LogInformation($"Added role to {socketUser.Username} in {watch.Elapsed}");
                }
                else
                {
                    watch.Stop();
                    return;
                }
            }
            else
                return;
        }

        private async Task ListenForReactionRemove(Cacheable<IUserMessage, ulong> cachedMessage,
                                                   ISocketMessageChannel originChannel, SocketReaction reaction)
        {
            if (reaction.Emote.ToString() == "🤍")
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                //Search for post
                var Post = ReactionPosts.FirstOrDefault(post => post.Id == reaction.MessageId);

                if (Post != null)
                {
                    //return if reaction was made by user
                    if (reaction.User.Value?.IsBot == true)
                        return;

                    //Get the cached message or download it
                    var message = await cachedMessage.DownloadAsync();

                    //Create EmbedBuilder object
                    var embed = message.Embeds.First().ToEmbedBuilder();

                    //To fix
                    //Avoid updating if list is full already 
                    //if (!embed.Fields.Any(x => x.Value == $"<@{reaction.UserId}>"))
                    //    return;

                    //make offset for fields
                    var postOffset = 2;

                    //remove field with UserId
                    embed.Fields.RemoveAll(user => user.Value.ToString() == $"<@{reaction.UserId}>");

                    //rebuild
                    if (embed.Fields.Count != postOffset)
                    {
                        for (var x = postOffset; x < embed.Fields.Count; x++)
                        {
                            embed.Fields[x].Name = (x - postOffset + 1).ToString();
                        }
                    }

                    await message.ModifyAsync(msg => msg.Embed = embed.Build());
                    watch.Stop();
                    _logger.LogInformation($"UserReactionDeleted with type [{Post.Type}] by [{reaction.User}] - {reaction.Channel} in {watch.Elapsed}");
                }

                else if (_servers.FirstOrDefault(item => item.RolePostId == reaction.MessageId) != null)
                {
                    var guild = _client.GetGuild(_servers
                        .FirstOrDefault(item => item.RolePostId == reaction.MessageId).ServerId);
                    var role = guild.Roles.FirstOrDefault(role => role.Name.ToString() == "LFP");
                    var socketUser = guild.GetUser(reaction.UserId);
                    if (socketUser?.IsBot == true)
                        return;
                    await socketUser.RemoveRoleAsync(role);
                    watch.Stop();
                    _logger.LogInformation($"Removed role from {socketUser.Username} in {watch.Elapsed}");
                }
                else
                {
                    watch.Stop();
                    return;
                }
            }
            else
                return;
        }
    }
}