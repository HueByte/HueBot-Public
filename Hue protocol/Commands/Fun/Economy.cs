using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.models;
using HueProtocol.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Commands.Fun {
    [Name("Economy")]
    public class Economy : ModuleBase<SocketCommandContext> {
        private readonly MiscDatabaseMethodsService _miscDb;
        private readonly EFContext _db;

        public Economy(EFContext db, MiscDatabaseMethodsService miscDb) {
            _db = db;
            _miscDb = miscDb;
        }

        [Command("Coins", RunMode = RunMode.Async)]
        [Summary("Shows your balance")]
        public async Task Coins() {
            var user = await _miscDb.GetOrCreateUser(Context.User.Id);

            var emb = new EmbedBuilder();
            emb.WithThumbnailUrl("https://www.iconpacks.net/icons/1/free-coin-icon-794-thumb.png");
            emb.WithCurrentTimestamp();
            emb.WithAuthor(Context.User);
            emb.WithColor(Color.Gold);
            emb.AddField("Coins: ", $"{user.Coins} :moneybag:");

            await ReplyAsync("", false, emb.Build());
        }

        [Command("GiveCoins")]
        [Summary("Give other user coins\nUsage: $GiveCoins @user")]
        public async Task GiveCoins(IUser usr, ulong amount) {
            var user = await _miscDb.GetOrCreateUser(Context.User.Id);

            var receiver = await _miscDb.GetOrCreateUser(usr.Id);

            if (usr.Id == Context.User.Id) {
                await ReplyAsync($"You've sent {amount} coins to yourself");
                return;
            }

            if (user.Coins < amount) {
                await ReplyAsync("You don't have that much coins, use $coins to check them!");
                return;
            }

            user.Coins -= amount;
            receiver.Coins += amount;

            _db.UserData.Update(receiver);
            _db.UserData.Update(user);
            await _db.SaveChangesAsync();
            await ReplyAsync($"You gave {usr.Username} {amount} coins!");
        }

        [Command("Daily", RunMode = RunMode.Async)]
        [Summary("Get your daily coins!\nDaily resets at 00:00")]
        public async Task DailyCoins() {
            var user = await _miscDb.GetOrCreateUser(Context.User.Id);

            if (user.UserDaily == true) {
                var nextDaily = new TimeSpan(24, 0, 0) - DateTime.Now.TimeOfDay;
                await ReplyAsync(
                    $"Sorry you got your daily coins, you can get another pack of them in {nextDaily.Hours} hours {nextDaily.Minutes} minutes");
                return;
            }

            user.Coins += 500;
            user.UserDaily = true;
            _db.UserData.Update(user);
            await _db.SaveChangesAsync();
            await ReplyAsync("You got 500 coins!");
        }
    }
}