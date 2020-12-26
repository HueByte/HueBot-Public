using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HueProtocol;
using HueProtocol.Entities;
using HueProtocol.Database;
using HueProtocol.models;
using HueProtocol.Lib;
using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace HueProtocol.Events
{
    public class DatabaseTimeEvents : IInjectableService
    {
        private ILogger<DatabaseTimeEvents> _logger;
        private EFContext _db;
        private DiscordShardedClient _client;
        public DatabaseTimeEvents(ILogger<DatabaseTimeEvents> logger, EFContext db, DiscordShardedClient client)
        {
            _logger = logger;
            _db = db;
            _client = client;
        }

        public async Task ClearOldPosts()
        {
            //Get posts older than 2 days and other type than raids
            var Posts = await Queryable.Where(_db.ReactionPosts, item => item.Date < DateTime.Now.AddDays(-2) && item.Type == "Daily").ToListAsync();

            if (Posts == null)
                return;

            //List for servers
            var servers = new List<IGuild>();
            //Get the unique server ids
            var uniqueIds = Posts.GroupBy(item => item.ServerId).Select(winner => winner.First()).ToList();

            //Get the servers via unique ids
            foreach (var server in uniqueIds)
            {
                servers.Add(_client.GetGuild(server.ServerId));
            }

            foreach (var server in servers)
            {
                foreach (var message in Posts)
                {
                    if (server.Id == message.ServerId)
                    {
                        try
                        {
                            //Get the message channel from the server
                            var channel = await server.GetTextChannelAsync(message.ChannelId);
                            //remove the message
                            await channel.DeleteMessageAsync(message.Id);
                            _logger.LogCritical($"[{message.Id}] removed from discord");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.ToString());
                        }
                    }
                }
            }
            _db.RemoveRange(Posts);
            await _db.SaveChangesAsync();
            await AddEvent(EventType.ClearingOldPosts);
            _logger.LogInformation("Finished clearing the database from old posts");
        }

        public async Task CoinReset()
        {
            //Check if event happened today
            var didHappen = await EntityFrameworkQueryableExtensions.AnyAsync(_db.Events, ev => ev.Date.Day == DateTime.Today.Day
                && ev.Date.Month == DateTime.Today.Month
                && ev.Date.Year == DateTime.Today.Year
                && ev.Type == EventType.Coins);
            if (didHappen)
                return;

            //get users
            var users = await EntityFrameworkQueryableExtensions.ToListAsync(_db.UserData);

            var newUsers = users.Select(c => { c.UserDaily = false; return c; }).ToList();
            _db.UserData.UpdateRange(newUsers);
            await _db.SaveChangesAsync();
            //Add event
            await AddEvent(EventType.Coins);
            _logger.LogInformation("Coin reset");
        }

        public async Task EggplantReset()
        {
            var didHappen = await EntityFrameworkQueryableExtensions.AnyAsync(_db.Events, ev => ev.Date.Day == DateTime.Today.Day
                && ev.Date.Month == DateTime.Today.Month
                && ev.Date.Year == DateTime.Today.Year
                && ev.Type == EventType.EggplantReset);
            if (didHappen)
                return;

            var users = await EntityFrameworkQueryableExtensions.ToListAsync(_db.UserData);
            var newUsers = users.Select(c => { c.EggplantDaily = false; return c; }).ToList();
            _db.UserData.UpdateRange(newUsers);
            await _db.SaveChangesAsync();

            await AddEvent(EventType.EggplantReset);
            _logger.LogInformation("Eggplant reset");
        }

        public async Task BirthdayEvent()
        {
            //check if event happened today already
            var didHappen = await EntityFrameworkQueryableExtensions.AnyAsync(_db.Events, ev => ev.Date.Day == DateTime.Today.Day
                && ev.Date.Month == DateTime.Today.Month
                && ev.Date.Year == DateTime.Today.Year
                && ev.Type == EventType.BirthdayEvent);
            if (didHappen)
                return;

            List<UserData> users = await System.Linq.AsyncEnumerable
                .Where<UserData>(_db.UserData.Include(entity => entity.BirthdayDate)
                .ToAsyncEnumerable(), user => user.BirthdayDate != null
                    && user.BirthdayDate.DoDisplay == true
                    && user.BirthdayDate.Date.Day == DateTime.Today.Day
                    && user.BirthdayDate.Date.Month == DateTime.Today.Month)
                .ToListAsync();

            if (users == null)
                return;

            foreach (var user in users)
            {
                //check if user wants to display his birthday
                if (!user.BirthdayDate.DoDisplay)
                {
                    return;
                }

                SocketUser discordUser = _client.GetUser(user.UserId);

                foreach (var guild in discordUser.MutualGuilds)
                {
                    var guildSetting = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_db.ServersSettings, x => x.ServerId == guild.Id);
                    //check if server allows birthdays
                    if (!guildSetting.DoBrithday)
                    {
                        continue;
                    }

                    if (guildSetting.BirthdayRoom == 0)
                        await guild.DefaultChannel.SendMessageAsync($"<@{user.UserId}> has birthday today! You can wish him his happy cake day!");
                    else
                    {
                        try
                        {
                            await guild.GetTextChannel(guildSetting.BirthdayRoom).SendMessageAsync($"<@{user.UserId}> has birthday today! You can wish him his happy cake day!");
                        }
                        catch (Exception e)
                        {
                            await guild.DefaultChannel.SendMessageAsync("Something went wrong with birthday room, could you check if the ID of it is correct?\nYou can use $GetChannelID command to see if its correct and $ServerSettings to see settings");
                        }
                    }
                }
            }

            //Add event
            await AddEvent(EventType.BirthdayEvent);
            _logger.LogInformation("Birthday event");
        }

        private async Task AddEvent(string eventType)
        {
            Event evn = null;

            switch (eventType)
            {
                case EventType.Coins:
                    evn = new Event()
                    {
                        Description = EventMessage.CoinReset,
                        Type = EventType.Coins,
                        Date = DateTime.Today
                    };
                    break;
                case EventType.EggplantReset:
                    evn = new Event()
                    {
                        Description = EventMessage.ClearOldPosts,
                        Type = EventType.EggplantReset,
                        Date = DateTime.Now
                    };
                    break;
                case EventType.BirthdayEvent:
                    evn = new Event()
                    {
                        Description = EventMessage.BirthdayToday,
                        Type = EventType.BirthdayEvent,
                        Date = DateTime.Now,
                    };
                    break;
                case EventType.ClearingOldPosts:
                    evn = new Event()
                    {
                        Description = EventMessage.ClearOldPosts,
                        Type = EventType.ClearingOldPosts,
                        Date = DateTime.Now
                    };
                    break;
                default:
                    evn = null;
                    _logger.LogInformation("Something went wrong with creating event");
                    break;
            }

            await _db.Events.AddAsync(evn);
            await _db.SaveChangesAsync();
        }
    }
}