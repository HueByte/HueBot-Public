using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Victoria;
using Victoria.EventArgs;

namespace HueProtocol.Services {
    public class AudioService : IInjectableService {
        private readonly LavaNode _lavaNode;
        private readonly DiscordShardedClient _client;
        private readonly ILogger<AudioService> _logger;
        public readonly HashSet<ulong> VoteQueue;
        private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;

        public AudioService(IServiceProvider services) {
            //Other
            _client = services.GetRequiredService<DiscordShardedClient>();
            _client.ShardReady += OnReady;
            _logger = services.GetRequiredService<ILogger<AudioService>>();

            //Lava
            _lavaNode = services.GetRequiredService<LavaNode>();
            _lavaNode.OnTrackEnded += OnTrackEnded;
            _lavaNode.OnTrackStarted += OntrackStarted;
            _lavaNode.OnTrackException += OnTrackException;
            _lavaNode.OnTrackStuck += OnTrackStuck;
            _lavaNode.OnWebSocketClosed += OnWebSocketClosed;
            _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();
            VoteQueue = new HashSet<ulong>();

            _logger.LogInformation("AudioService has been loaded");
        }

        private async Task OntrackStarted(TrackStartEventArgs args) {
            //If player hasn't got cancellation token return
            if (!_disconnectTokens.TryGetValue(args.Player.VoiceChannel.Id, out var value))
                return;

            //If Cancellation token isn't active return
            if (value.IsCancellationRequested)
                return;

            //deactive the token
            value.Cancel(true);
            _logger.LogInformation(
                $"The auto-disconnected cancelled from {args.Player.VoiceChannel.Name} in {args.Player.VoiceChannel.Guild.Name}");
        }

        private async Task OnTrackEnded(TrackEndedEventArgs args) {
            if (!args.Reason.ShouldPlayNext())
                return;

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable)) {
                //If there's nothing to play, initialize the auto disconnect
                _ = InitDisconnect(args.Player, TimeSpan.FromMinutes(10));
                return;
            }

            if (!(queueable is LavaTrack track)) {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            await args.Player.PlayAsync(track);
            await player.PlayAsync(track);
            var emb = new EmbedBuilder();
            emb.WithColor(Color.DarkPurple);
            emb.WithTitle("Now playing");
            emb.WithDescription(track.Title);
            emb.AddField("Duration", track.Duration.ToString(@"hh\:mm\:ss"));
            emb.AddField("Author", track.Author);
            emb.WithThumbnailUrl(track.FetchArtworkAsync().GetAwaiter().GetResult());
            await player.TextChannel.SendMessageAsync("", false, emb.Build());
        }

        private async Task InitDisconnect(LavaPlayer player, TimeSpan timeSpan) {
            //If player hasn't got cancellation token, create one
            if (!_disconnectTokens.TryGetValue(player.VoiceChannel.Id, out var value)) {
                value = new CancellationTokenSource();
                _disconnectTokens.TryAdd(player.VoiceChannel.Id, value);
            }

            //if token already existed and is active
            else if (value.IsCancellationRequested) {
                _disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
                value = _disconnectTokens[player.VoiceChannel.Id];
            }

            await player.TextChannel.SendMessageAsync(
                $"No more tracks to play, I will auto-disconnect in {timeSpan.Minutes} minues if you won't play anything!");

            //wait for the timeSpan for Cancellationtoken to be in cancellation state
            var isCancelled = SpinWait.SpinUntil(() => value.IsCancellationRequested, timeSpan);
 
            if (isCancelled)
                return;

            value.Dispose();
            _disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
            //disconnect
            await _lavaNode.LeaveAsync(player.VoiceChannel);
            await player.TextChannel.SendMessageAsync(
                $"I wasn't playing any track for {timeSpan.Minutes} minutes. **Disconnecting**");
        }

        private Task OnTrackException(TrackExceptionEventArgs arg) {
            _logger.LogCritical($"Track exception received for {arg.Track.Title}.");
            return Task.CompletedTask;
        }

        private Task OnTrackStuck(TrackStuckEventArgs arg) {
            _logger.LogError($"Track stuck received for {arg.Track.Title}.");
            return Task.CompletedTask;
        }

        private async Task OnWebSocketClosed(WebSocketClosedEventArgs arg) {
            var player = _lavaNode.GetPlayer(_client.GetGuild(arg.GuildId));
            var voiceChannel = player.VoiceChannel;
            await voiceChannel.DisconnectAsync();
            _logger.LogCritical($"Discord WebSocket connection closed with following reason: {arg.Reason}");
        }

        private async Task OnReady(DiscordSocketClient client) {
            try {
                await _lavaNode.ConnectAsync();
                _logger.LogInformation("Lava should be connected");
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}