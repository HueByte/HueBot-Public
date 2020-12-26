using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.models;
using HueProtocol.Services;
using HueProtocol.Entities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HueProtocol.Tools
{
    public class MiscDatabaseMethodsService : IInjectableService
    {
        private readonly EFContext _db;
        private readonly CommandHandler _commandHandler;
        private DiscordShardedClient _client;

        public MiscDatabaseMethodsService(EFContext db, CommandHandler handler, DiscordShardedClient client)
        {
            _db = db;
            _commandHandler = handler;
            _client = client;
        }

        public async Task<UserData> GetOrCreateUser(ulong userId)
        {
            // var user = await _db.UserData.FirstOrDefaultAsync(user => user.UserId == userId);
            var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_db.UserData, user => user.UserId == userId);
            if (user != null)
                return user;

            var userInfo = new UserData
            {
                UserId = userId,
                UserName = _client.GetUser(userId).Username,
                Coins = 0,
                CurrentQuest = null,
                Level = 1,
                UserDaily = false,
                EggplantDaily = false
            };

            await _db.UserData.AddAsync(userInfo);
            await _db.SaveChangesAsync();
            return userInfo;
        }

        public async Task<ServerGuild> GetOrCreateServer(ulong serverId)
        {
            var server = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_db.ServersSettings, server => server.ServerId == serverId);
            if (server != null)
                return server;

            var guild = _client.GetGuild(serverId);

            server = new ServerGuild
            {
                DoGreet = false,
                GreetingMsg = null,
                GreetingRoom = guild.DefaultChannel.Id,
                OwnerId = guild.OwnerId,
                OwnerName = guild.Owner.Nickname,
                prefix = '$',
                RolePostId = 0,
                ServerId = guild.Id,
                ServerName = guild.Name,
                BirthdayRoom = guild.DefaultChannel.Id,
                DoBrithday = false
            };

            await _db.ServersSettings.AddAsync(server);
            await _db.SaveChangesAsync();
            return server;
        }

        public async Task CreatePost(string title, string time, string details, string type,
                                     SocketCommandContext context, int playerLimit = 23,
                                     string thumbnail =
                                         "https://i.pinimg.com/originals/ef/0b/5c/ef0b5cd2663c4412a71e13642fb1f2da.jpg")
        {
            //check the player limit
            if (playerLimit > 23)
            {
                var wrong = await context.Channel.SendMessageAsync(
                    "The value of **maximum** players has to be lower than 23 ");
                await Task.Delay(5000);
                await wrong.DeleteAsync();
                return;
            }

            //eventually mention if there's a LFP role
            var mention = "";

            //build embed
            var emb = new EmbedBuilder();
            emb.WithAuthor(context.User.Username, context.User.GetAvatarUrl());
            emb.Title = title;

            //check the type
            if (type == "Daily")
            {
                //get the role
                var role = context.Guild.Roles.FirstOrDefault(x => x.Name == "LFP");
                //if role wasn't found ignore it 
                mention = role == null ? "" : $"{role.Mention}";
                emb.WithDescription("React with 🤍 to sign yourself into the party!");
            }

            //if it's raid
            else
                emb.WithDescription("React with 🤍 to sign yourself into the raid!");

            emb.WithThumbnailUrl(thumbnail);
            emb.WithColor(Color.DarkPurple);
            emb.WithCurrentTimestamp();
            emb.WithFooter(@"Post made with love 🤍");
            emb.AddField("Time: ", $"```diff\n+ {time}```");
            emb.AddField("Details: ", $"```fix\n{details}```");

            var emoji = new Emoji("🤍");
            await context.Message.DeleteAsync();
            var msg = await context.Channel.SendMessageAsync(mention, false, emb.Build());
            await msg.AddReactionAsync(emoji);

            await _db.ReactionPosts.AddAsync(new ReactionPost()
            {
                Id = msg.Id,
                ChannelId = context.Channel.Id,
                ServerId = context.Guild.Id,
                Date = DateTime.Now,
                Type = type,
                UserMax = playerLimit,
                Details = details
            });

            await _db.SaveChangesAsync();
            await _commandHandler.updatePosts();
        }
    }
}