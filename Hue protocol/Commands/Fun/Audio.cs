using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HueProtocol.Services;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.Enums;

namespace HueProtocol.Commands.Fun
{
    [Name("Audio")]
    public class Audio : ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        private readonly AudioService _musicService;
        private static readonly IEnumerable<int> Range = Enumerable.Range(1900, 2000);

        public Audio(IServiceProvider services)
        {
            _lavaNode = services.GetRequiredService<LavaNode>();
            _musicService = services.GetRequiredService<AudioService>();
        }

        [Command("Join")]
        [Summary("Makes the bot join the voice chat")]
        public async Task JoinAsync()
        {
            //Check if bot is already connected any to voice chat
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                await ReplyAsync("I'm already connected to a voice channel!");
                return;
            }

            var voiceState = Context.User as IVoiceState;

            //Check if user is connected to voice chat
            if (voiceState?.VoiceChannel == null)
            {
                await ReplyAsync("You must be connected to a voice channel!");
                return;
            }

            //try to join
            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        //Consider limiting that command to people in the same voice room
        [Command("Leave")]
        [Summary("Makes the bot Leave the voice chat")]
        public async Task LeaveAsync()
        {
            //Check if bot is connected to voice chat
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to any voice channels!");
                return;
            }

            //Handle empty voice room
            var voiceChannel = (Context.User as IVoiceState).VoiceChannel ?? player.VoiceChannel;
            if (voiceChannel == null)
            {
                await ReplyAsync("Not sure which voice channel to disconnect from.");
                return;
            }

            //Try to leave the voice room
            try
            {
                await _lavaNode.LeaveAsync(voiceChannel);
                await ReplyAsync($"I've left {voiceChannel.Name}!");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("Play", RunMode = RunMode.Async)]
        [Alias("p")]
        [Summary("Plays the track\nUsage: $Play <link>")]
        public async Task PlayAsync([Remainder] string query)
        {
            //Check if query is empty 
            if (string.IsNullOrWhiteSpace(query))
            {
                await ReplyAsync("Please provide search terms.");
                return;
            }

            var voiceState = Context.User as IVoiceState;

            //Check if user is connected to voice room
            if (voiceState.VoiceChannel == null)
            {
                await ReplyAsync("You're not connected to the same voice channel as bot");
                return;
            }

            //Check if bot is already connected to voice room if not - connect 
            if(!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                try
                {
                    await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                    await ReplyAsync($"Joined {voiceState.VoiceChannel.Name}!");
                    player = _lavaNode.GetPlayer(Context.Guild);
                }
                catch
                {
                    await ReplyAsync("Make sure you're in the voice channel before you use that command");
                    return;
                }
            }

            //Check if the ID of user's room is the same as bot's
            if(voiceState.VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You have to be in the same voice channel as bot to play songs");
                return;
            }

            var searchResponse = await (Uri.IsWellFormedUriString(query, UriKind.Absolute) ?
                _lavaNode.SearchAsync(query) : _lavaNode.SearchYouTubeAsync(query));

            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await ReplyAsync($"I wasn't able to find anything for `{query}`.");
                return;
            }

            //var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    foreach (var track in searchResponse.Tracks)
                    {
                        player.Queue.Enqueue(track);
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    var track = searchResponse.Tracks[0];
                    player.Queue.Enqueue(track);
                    await ReplyAsync($"Enqueued: {track.Title}");
                }
            }
            else
            {
                var track = searchResponse.Tracks[0];

                if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
                {
                    for (var i = 0; i < searchResponse.Tracks.Count; i++)
                    {
                        if (i == 0)
                        {
                            await player.PlayAsync(track);
                            var emb = new EmbedBuilder();
                            emb.WithColor(Color.DarkPurple);
                            emb.WithTitle("Now playing");
                            emb.WithDescription(track.Title);
                            emb.AddField("Duration", track.Duration.ToString(@"hh\:mm\:ss"));
                            emb.AddField("Author", track.Author);
                            emb.WithThumbnailUrl(track.FetchArtworkAsync().GetAwaiter().GetResult());
                            await ReplyAsync("", false, emb.Build());
                        }
                        else
                        {
                            player.Queue.Enqueue(searchResponse.Tracks[i]);
                        }
                    }

                    await ReplyAsync($"Enqueued {searchResponse.Tracks.Count} tracks.");
                }
                else
                {
                    await player.PlayAsync(track);
                    var emb = new EmbedBuilder();
                    emb.WithColor(Color.DarkPurple);
                    emb.WithTitle("Now playing");
                    emb.WithDescription(track.Title);
                    emb.AddField("Duration", track.Duration.ToString(@"hh\:mm\:ss"));
                    emb.AddField("Author", track.Author);
                    emb.WithThumbnailUrl(track.FetchArtworkAsync().GetAwaiter().GetResult());
                    await ReplyAsync("", false, emb.Build());
                }
            }
        }

        [Command("Resume")]
        [Summary("Resumes the track")]
        public async Task ResumeAsync()
        {
            //Check if bot is already connected to voice chat
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            //Check if the user's room is the same as bot's
            if ((Context.User as IVoiceState).VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You have to be in the same room as bot to resume the music");
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                await ReplyAsync("But I am playing!");
                return;
            }
            try
            {
                await player.ResumeAsync();
                await ReplyAsync($"Resumed: {player.Track.Title}");
            }
            catch
            {
                await ReplyAsync("Something went wrong");
            }
        }

        [Command("Pause")]
        [Summary("Pauses the track")]
        public async Task PauseAsync()
        {
            //Check if the bot is connected to voice chat
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            //Check if the user's room is the same as bot's
            if ((Context.User as IVoiceState).VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You have to be in the same room as bot to pause the music");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("I cannot pause when I'm not playing anything!");
                return;
            }

            try
            {
                await player.PauseAsync();
                await ReplyAsync($"Paused: {player.Track.Title}");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("Stop")]
        [Summary("Stops the music")]
        public async Task StopAsync()
        {
            //Check if the bot is connected to voice room
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            //Check if the user's room is the same as bot's
            if ((Context.User as IVoiceState).VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You have to be in the same room as bot to stop the music");
                return;
            }

            if (player.PlayerState == PlayerState.Stopped)
            {
                await ReplyAsync("Woaaah there, I can't stop the stopped.");
                return;
            }

            try
            {
                await player.StopAsync();
                player.Queue.Clear();
                await ReplyAsync("No longer playing anything.");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("Skip")]
        [Alias("s")]
        [Summary("Skips the track or entered amount of tracks\nUsage: $Skip 10")]
        public async Task SkipAsync(int amount = 0)
        {
            //Check if the user is connected to voice room
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            //Check if the user's room is the same as bot's
            if ((Context.User as IVoiceState).VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You have to be in the same room as bot to skip the music");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I can't skip when nothing is playing.");
                return;
            }

            if (player.Queue.Count < amount)
            {
                await ReplyAsync("Provide value which is lower than overall count of tracks");
                return;
            }

            /*var voiceChannelUsers = (player.VoiceChannel as SocketVoiceChannel).Users.Where(x => !x.IsBot).ToArray();
            if (MusicService.VoteQueue.Contains(Context.User.Id))
            {
                await ReplyAsync("You can't vote again.");
                return;
            }

            MusicService.VoteQueue.Add(Context.User.Id);
            var percentage = MusicService.VoteQueue.Count / voiceChannelUsers.Length * 100;
            if (percentage < 30)
            {
                await ReplyAsync("You need more than 30% votes to skip this song.");
                return;
            }
            */

            try
            {
                var oldTrack = player.Track;

                if (amount > 0)
                    player.Queue.RemoveRange(0, amount);

                var currenTrack = await player.SkipAsync();

                var emb = new EmbedBuilder();
                emb.WithColor(Color.DarkPurple);
                emb.WithTitle("Now playing");
                emb.WithDescription(currenTrack.Title);
                emb.AddField("Duration", currenTrack.Duration.ToString(@"hh\:mm\:ss"));
                emb.AddField("Author", currenTrack.Author);
                emb.WithThumbnailUrl(currenTrack.FetchArtworkAsync().GetAwaiter().GetResult());

                await ReplyAsync($"Skipped: {oldTrack.Title}", false, emb.Build());
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        //To Update
        [Command("Seek")]
        [Summary("Check thes status")]
        public async Task SeekAsync(TimeSpan timeSpan)
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I can't seek when nothing is playing.");
                return;
            }

            try
            {
                await player.SeekAsync(timeSpan);
                await ReplyAsync($"I've seeked `{player.Track.Title}` to {timeSpan}.");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("Volume")]
        [Summary("Change the volume with max of 100\nUsage: $Volume <number>")]
        public async Task VolumeAsync(ushort volume)
        {
            //Check if the USER IS OWNER OF THE BOT TO NOT LET THEM EARRAPE 
            if(volume > 100)
            {
                if (Context.User.Id != 215556401467097088)
                {
                    await ReplyAsync("You can't make it higher than 100");
                    return;
                }
            }

            //Check if the bot is connected to voice chat
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            //Check if the user's room is the same as bot's
            if ((Context.User as IVoiceState).VoiceChannel != player.VoiceChannel)
            {
                await ReplyAsync("You have to be in the same room as bot to change the volume");
                return;
            }

            try
            {
                await player.UpdateVolumeAsync(volume);
                await ReplyAsync($"I've changed the player volume to {volume}.");
            }
            catch (Exception exception)
            {
                await ReplyAsync(exception.Message);
            }
        }

        [Command("NowPlaying"), Alias("Np")]
        [Summary("Get current track")]
        public async Task NowPlayingAsync()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I'm not playing any tracks.");
                return;
            }

            var track = player.Track;
            var artwork = await track.FetchArtworkAsync();

            var embed = new EmbedBuilder
            {
                Title = $"{track.Author} - {track.Title}",
                ThumbnailUrl = artwork,
                Url = track.Url
            }
                .AddField("Id", track.Id)
                .AddField("Duration", track.Duration)
                .AddField("Position", track.Position);

            await ReplyAsync(embed: embed.Build());
        }

        [Command("Genius", RunMode = RunMode.Async)]
        [Summary("Get Genius lyrics")]
        public async Task ShowGeniusLyrics()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I'm not playing any tracks.");
                return;
            }

            var lyrics = await player.Track.FetchLyricsFromGeniusAsync();
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await ReplyAsync($"No lyrics found for {player.Track.Title}");
                return;
            }

            var splitLyrics = lyrics.Split('\n');
            var stringBuilder = new StringBuilder();
            foreach (var line in splitLyrics)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    await ReplyAsync($"```{stringBuilder}```");
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            await ReplyAsync($"```{stringBuilder}```");
        }

        [Command("OVH", RunMode = RunMode.Async)]
        [Summary("Get OVH lyrics")]
        public async Task ShowOvhLyrics()
        {
            if (!_lavaNode.TryGetPlayer(Context.Guild, out var player))
            {
                await ReplyAsync("I'm not connected to a voice channel.");
                return;
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                await ReplyAsync("Woaaah there, I'm not playing any tracks.");
                return;
            }

            var lyrics = await player.Track.FetchLyricsFromOVHAsync();
            if (string.IsNullOrWhiteSpace(lyrics))
            {
                await ReplyAsync($"No lyrics found for {player.Track.Title}");
                return;
            }

            var splitLyrics = lyrics.Split('\n');
            var stringBuilder = new StringBuilder();
            foreach (var line in splitLyrics)
            {
                if (Range.Contains(stringBuilder.Length))
                {
                    await ReplyAsync($"```{stringBuilder}```");
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.AppendLine(line);
                }
            }

            await ReplyAsync($"```{stringBuilder}```");
        }

    }
}

