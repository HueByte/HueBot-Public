using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.models;
using HueProtocol.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace HueProtocol.Commands
{
    [Name("Admin")]
    public class Admin : ModuleBase<SocketCommandContext>
    {
        private MiscDatabaseMethodsService _miscDb;
        private EFContext _dataBase;
        private DiscordShardedClient _client;

        public Admin(MiscDatabaseMethodsService miscDb, EFContext dbContext, DiscordShardedClient client)
        {
            _miscDb = miscDb;
            _dataBase = dbContext;
            _client = client;
        }

        #region Bot Configuration

        [Command("ConfigurePrefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Uploads server specified prefix to database - worthless for now\nUsage: $ConfigurePrefix <Character>")]
        public async Task ConfigurePrefix(string prefix)
        {
            var server = await _miscDb.GetOrCreateServer(Context.Guild.Id);
            server.prefix = Char.Parse(prefix);
            _dataBase.ServersSettings.Update(server);
            await _dataBase.SaveChangesAsync();
            await ReplyAsync("Server prefix to H.u.e database");

        }

        [Command("ConfigureGreeting", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Configures the greeting of the server\nUsage: $ConfigureGreeting [Channel ID] \"The greeting message where #user will be replaced with new user\"")]
        public async Task ConfigureGreet(ulong GreetingRoom, string greeting)
        {
            var server = await _miscDb.GetOrCreateServer(Context.Guild.Id);
            if (server == null)
                await ReplyAsync("Use \"$ConfigureBot\" command before that one!");
            else
            {
                server.GreetingRoom = GreetingRoom;
                //replace #user with new user
                server.GreetingMsg = greeting;
                _dataBase.ServersSettings.Update(server);
                await _dataBase.SaveChangesAsync();
                await ReplyAsync("Server greeting settings to H.u.e database");
            }
        }

        [Command("ToggleWelcoming")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Activates or disables the greeting option")]
        public async Task GreetBool()
        {
            var server = await _miscDb.GetOrCreateServer(Context.Guild.Id);
            if (server == null)
            {
                await ReplyAsync("Use $ConfigureBot before that command!");
                return;
            }

            var DoGreet = server.DoGreet == false ? true : false;

            server.DoGreet = DoGreet;
            _dataBase.ServersSettings.Update(server);
            await _dataBase.SaveChangesAsync();
            await ReplyAsync($"Greeting has been " + (DoGreet ? "enabled" : "Disabled"));
        }

        //TODO
        [Command("CreateRolePost", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Create post for assigning roles\nUsage: <title> <description> <thumbnailUrl>")]
        public async Task CreateRolePost(string title = "title", string description = "description", string ThumbnailUrl = "")
        {
            var emb = new EmbedBuilder();
            emb.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
            emb.Title = title;
            emb.Description = description;
            emb.ThumbnailUrl = ThumbnailUrl;
            emb.WithColor(Color.DarkPurple);
            emb.WithFooter(@"Post made at ");
            emb.WithCurrentTimestamp();

            await Context.Message.DeleteAsync();
            var msg = await ReplyAsync("", false, emb.Build());
            await msg.AddReactionAsync((new Emoji("🤍")));

            var server = await _miscDb.GetOrCreateServer(Context.Guild.Id);
            server.RolePostId = msg.Id;
            _dataBase.ServersSettings.Update(server);
            await _dataBase.SaveChangesAsync();
        }

        [Command("ToggleBirthdays")]
        [Summary("Switch on/off displaying birthdays at your server")]
        public async Task ToggleBirthdays()
        {
            var server = await _miscDb.GetOrCreateServer(Context.Guild.Id);
            server.DoBrithday = !server.DoBrithday;
            if (server.DoBrithday)
                await ReplyAsync("I'll be displaying birthdays since now");
            else
                await ReplyAsync("I'll stop displaying birthdays now");
            _dataBase.ServersSettings.Update(server);
            await _dataBase.SaveChangesAsync();
        }

        [Command("BirthdayRoom")]
        [Summary("Set birthday room for birthdays")]
        public async Task SetBirthdayRoom(ulong Id)
        {
            var server = await _miscDb.GetOrCreateServer(Context.Guild.Id);
            server.BirthdayRoom = Id;
            _dataBase.ServersSettings.Update(server);
            await _dataBase.SaveChangesAsync();
            var channel = _client.GetChannel(Id);
            await ReplyAsync($"Changed birthday room to <#{channel.Id}>");
        }
        #endregion

        [Command("purge", RunMode = RunMode.Async)]
        [Alias("pur")]
        [RequireUserPermission(Discord.ChannelPermission.ManageMessages)]
        [Summary("Purges x amount of messages\nUsage: $purge <number>")]
        public async Task PurgeChat(uint amount)
        {
            var messages = await Context.Channel.GetMessagesAsync((int)amount + 1).FlattenAsync();

            var filteredMessages = messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            var count = filteredMessages.Count();

            if (count == 0)
                await ReplyAsync("Nothing to delete");
            else
            {
                await (Context.Channel as ITextChannel).DeleteMessagesAsync(filteredMessages);
            }
        }

        [Command("GetChannel")]
        [Alias("chanID")]
        [Summary("Gives channel ID")]
        public async Task GetChannelID()
        {
            await ReplyAsync(Context.Channel.Id.ToString());
        }

        [Command("ServerInfo")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Summary("Gives you information about the server")]
        public async Task ShowInfo()
        {
            var server = await _miscDb.GetOrCreateServer(Context.Guild.Id);
            var embed = new EmbedBuilder
            {
                Title = "Server information",
                Description = Context.Guild.Description,
                ThumbnailUrl = Context.Guild.IconUrl
            };
            embed.WithFooter($"Post made by {Context.User.Username} request");
            embed.WithColor(Color.DarkPurple);
            embed.AddField("Hue Prefix: ", $"{server.prefix}");
            embed.AddField("Server name: ", $"{server.ServerName}");
            embed.AddField("Server ID: ", $"{server.ServerId}");
            embed.AddField("Owner: ", $"{server.OwnerName}");
            embed.AddField("Members:", $"{Context.Guild.MemberCount}");
            embed.AddField("Created at: ", $"{Context.Guild.CreatedAt}");

            await ReplyAsync("", false, embed.Build());

        }

        #region strikes 

        [Command("strike", RunMode = RunMode.Async)]
        [Summary("Gives user a strike\nUsage: $strike @user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Strike(IUser user, string reason = null)
        {
            var userInfo = await _miscDb.GetOrCreateUser(user.Id);

            var creator = await _miscDb.GetOrCreateUser(Context.User.Id);

            var strike = new Strike()
            {
                User = userInfo,
                GuildId = Context.Guild.Id,
                Reason = reason,
                Date = DateTime.Now,
                Creator = creator
            };

            _dataBase.Strikes.Add(strike);
            await _dataBase.SaveChangesAsync();
            var strikes = Queryable.Where(_dataBase.Strikes, s => s.User.UserId == user.Id && s.GuildId == Context.Guild.Id)
                                             .ToList();
            await ReplyAsync($"{user.Mention} got a new strike! Overall: {strikes.Count} strikes");

        }

        [Command("RemoveStrike", RunMode = RunMode.Async)]
        [Alias("RmStrike")]
        [Summary("Removes oldest strike from user\nUsage: $RemoveStrike @user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task RemoveStrike(IUser user)
        {
            var userInfo = await _miscDb.GetOrCreateUser(user.Id);

            //If user has 0 strikes skip and respond
            var strikes = Queryable.Where(_dataBase.Strikes, s => s.User.UserId == user.Id && s.GuildId == Context.Guild.Id).ToList();
            if (strikes.Count == 0)
            {
                await ReplyAsync("This user hasn't got any strikes");
                return;
            }

            _dataBase.Strikes.Remove(strikes[strikes.Count - 1]);
            await _dataBase.SaveChangesAsync();
            await ReplyAsync($"Strike removed from {user.Mention}! Overall: {strikes.Count - 1} strikes");

        }

        [Command("ClearStrikes", RunMode = RunMode.Async)]
        [Alias("ClearS")]
        [Summary("Clears strikes of mentioned user\nUsage: $ClearStrikes @user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task ClearStrikes(IUser user)
        {
            _dataBase.Strikes.RemoveRange(Queryable.Where(_dataBase.Strikes, s => s.User.UserId == user.Id && s.GuildId == Context.Guild.Id));
            await _dataBase.SaveChangesAsync();
            await ReplyAsync("Strikes removed");
        }

        [Command("ShowStrikes", RunMode = RunMode.Async)]
        [Alias("ShowS")]
        [Summary("Shows strikes that user has\nUsage: $ShowStrikes @user")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task ShowStrike(IUser user)
        {
            var strikes = Queryable.Where(_dataBase.Strikes, s => s.User.UserId == user.Id && s.GuildId == Context.Guild.Id)
                                             .ToList();
            if (strikes.Count == 0)
            {
                await ReplyAsync("This user hasn't got any strikes");
                return;
            }
            var sb = new StringBuilder();
            foreach (var strike in strikes)
            {
                sb.AppendLine("```fix");
                sb.AppendLine($"Strike ID: {strike.StrikeId}");

                IUser creator = strike.Creator != null
                    ? _client.GetUser(strike.Creator.UserId)
                    : null;

                if (creator != null)
                    sb.AppendLine($"Creator: {creator.Username}");
                else
                    sb.AppendLine("Creator: Unknown");

                sb.AppendLine($"Date: {strike.Date}");
                var reason = strike.Reason ?? "🤷‍♂️";
                sb.AppendLine($"Reason: {reason}");
                sb.AppendLine("```");
            }

            var emb = new EmbedBuilder();
            emb.WithTitle($"{user.Username} strikes");
            emb.WithDescription($"\n{sb}\n");
            emb.WithColor(Color.Magenta);
            emb.WithCurrentTimestamp();
            emb.WithThumbnailUrl(user.GetAvatarUrl());

            await ReplyAsync("", false, emb.Build());

        }

        [Command("RemoveStrikeByID", RunMode = RunMode.Async)]
        [Alias("RSID")]
        [Summary("Removes the warn by it's ID - You can check the ID's by $ShowWarns @user command")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task RemoveStrikeByID(long id)
        {
            var strike = await _dataBase.Strikes.FirstOrDefaultAsync(x => x.StrikeId == id && x.GuildId == Context.Guild.Id);
            if (strike == null)
            {
                await ReplyAsync("There's no strike with that ID");
                return;
            }


            //Build content
            var embed = new EmbedBuilder();
            embed.WithTitle($"Removed strike with {id} ID");
            embed.WithColor(Color.Gold);
            embed.WithCurrentTimestamp();
            embed.AddField("Strike ID:", strike.StrikeId, true);
            embed.AddField("Creator:", $"<@!{strike.Creator.UserId}>", true);
            var reason = strike.Reason ?? "🤷‍♂️";
            embed.AddField("Reason:", reason);
            embed.AddField("Date:", strike.Date, true);

            //Remove from database and save + send feedback
            _dataBase.Strikes.Remove(strike);
            await _dataBase.SaveChangesAsync();
            await ReplyAsync("", false, embed.Build());
        }

        #endregion

        [Command("ServerSettings")]
        [Summary("Show your server settings")]
        public async Task ServerSettingsShow()
        {
            var server = await _miscDb.GetOrCreateServer(Context.Guild.Id);
            EmbedBuilder emb = new EmbedBuilder()
            {
                Color = Discord.Color.DarkTeal,
                Title = $"{Context.Guild.Name} settings",
                Description = $"Check your settings here. If you find room ID as 0 it means it's server default",
            };

            try
            {
                emb.WithThumbnailUrl(Context.Guild.IconUrl);
                emb.WithCurrentTimestamp();
                emb.AddField("Prefix", server.prefix);
                if (server.GreetingRoom != 0)
                    emb.AddField("Greeting room", $"<#{server.GreetingRoom}>");
                else
                    emb.AddField("Greeting room", "Default");
                if (server.GreetingMsg != null)
                    emb.AddField("Greeting message", server.GreetingMsg);
                else
                    emb.AddField("Greeting message", "null");
                emb.AddField("Greeting status", server.DoGreet.ToString());
                if (server.BirthdayRoom != 0)
                    emb.AddField("Birthday announcement room", $"<#{server.BirthdayRoom}>");
                else
                    emb.AddField("Birthday announcement room", "Default");
                emb.AddField("Birthday Status", server.DoBrithday.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await ReplyAsync("", false, emb.Build());
        }
    }
}
