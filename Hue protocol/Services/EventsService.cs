using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.models;
using HueProtocol.Entities;
using HueProtocol.Tools;
using HueProtocol.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Services
{
    internal class EventService : IInjectableService
    {
        private ILogger<EventService> _logger;
        private DiscordShardedClient _client;
        private EFContext _db;
        private MiscDatabaseMethodsService _misc;
        private DatabaseTimeEvents _events;
        private System.Threading.Timer dailyTimer;

        //* fix this services
        public EventService(IServiceProvider services, ILogger<EventService> logger, DiscordShardedClient client, EFContext db, MiscDatabaseMethodsService misc, DatabaseTimeEvents events)
        {
            _logger = logger;
            _client = client;
            _db = db;
            _misc = misc;
            _events = events;

            //Start the timer event
            _client.ShardReady += DailyTimer;

            _logger.LogInformation("DailyLooper has been loaded");
        }

        public async Task DailyTimer(DiscordSocketClient client)
        {
            var startTime = new TimeSpan(24, 0, 0) - DateTime.Now.TimeOfDay;
            var looper = TimeSpan.FromHours(24);

            _logger.LogInformation($"Daily coins reset timer started - Time to next reset: {startTime.Hours}:{startTime.Minutes}");
            _logger.LogInformation($"Daily coins event appears every 24 hours (00;00)");
            _logger.LogInformation("Clearing old posts started");
            _logger.LogInformation("Birthday timer started");

            //Check if these events happened today
            await _events.BirthdayEvent();
            await _events.CoinReset();
            await _events.ClearOldPosts();
            await _events.EggplantReset();

            dailyTimer = new System.Threading.Timer(async (e) =>
            {
                await _events.BirthdayEvent();
                await _events.CoinReset();
                await _events.ClearOldPosts();
                await _events.EggplantReset();
            }, null, startTime, looper);
        }
    }
}
