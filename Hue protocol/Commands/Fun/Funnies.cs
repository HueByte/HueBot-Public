using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HueProtocol.models;
using HueProtocol.Services.APIs;
using HueProtocol.Tools;
using HueProtocol.Database;
using Newtonsoft.Json;

namespace HueProtocol.Commands.Fun
{
    [Name("Fun")]
    public class Funnies : ModuleBase<SocketCommandContext>
    {
        private readonly UrbanService _urbanService;
        private readonly CoronaService _coronaApi;
        private readonly MiscDatabaseMethodsService _miscDb;
        private readonly EFContext _db;

        public Funnies(UrbanService urbanService, CoronaService coronaService, MiscDatabaseMethodsService miscDb, EFContext db)
        {
            _coronaApi = coronaService;
            _urbanService = urbanService;
            _miscDb = miscDb;
            _db = db;
        }

        [Command("random")]
        [Alias("rnd")]
        [Summary(
            "rolls the random number from 1 to 100. If you want custom numbers to roll from look at usage.\nUsage: $Random minValue maxValue")]
        public async Task Roll(int min = 1, int max = 100)
        {
            var rnd = new Random();
            await ReplyAsync((rnd.Next(min, max)).ToString());
        }

        [Command("HowYouFeel")]
        [Summary("Ask how the bot feels like")]
        public async Task Feelings()
        {
            var emote = await GetEmote("x5");
            await ReplyAsync($"<:{emote.Name}:{emote.Id}>");
        }

        [Command("Kys")]
        [Summary("Wish death to bot")]
        public async Task Kys()
        {
            var emote = await GetEmote("x5");
            await ReplyAsync($"No u <:{emote.Name}:{emote.Id}>");
        }

        [Command("GetAvatarURL")]
        [Alias("gau")]
        [Summary("Get the url of the avatar\nUsage: $GetAvatarUrl @user")]
        public async Task GetUrlAvatar(IUser user)
        {
            var embed = new EmbedBuilder();
            embed.WithThumbnailUrl(user.GetAvatarUrl());
            embed.WithTitle($"Avatar link of {user.Username}");
            embed.WithDescription(user.GetAvatarUrl());
            await ReplyAsync("", false, embed.Build());
        }

        [Command("PatHue")]
        [Summary("Pat Hue :)")]
        public async Task PatMe()
        {
            var embed = new EmbedBuilder
            {
                ImageUrl =
                    "https://media.discordapp.net/attachments/519223818309468180/619784806741311518/1434476384_file.gif"
            };
            embed.WithColor(Color.DarkMagenta);

            await ReplyAsync("", false, embed.Build());
        }

        [Command("Fight", RunMode = RunMode.Async)]
        [Summary("Fight with somebody!\nUsage: $Fight @user @user")]
        public async Task Fight(IUser user, IUser user2)
        {
            var emot = await GetEmote("PeFightLeft");
            var emot2 = await GetEmote("PeFightRight");

            var rnd = new Random();
            IUser[] array = { user, user2 };
            var winner = array[rnd.Next(0, 2)];

            try
            {
                await ReplyAsync($"Fight! <:{emot.Name}:{emot.Id}> <:{emot2.Name}:{emot2.Id}>");
            }
            catch
            {
                await ReplyAsync("Fight!");
            }

            string[] text = { "What's this!?", "You lost this fight!", "*Cloud of dust falls over...*" };
            foreach (var item in text)
            {
                await ReplyAsync(item);
                await Task.Delay(400);
            }
            await ReplyAsync($"{winner.Mention} won!");
        }

        [Command("Urban", RunMode = RunMode.Async)]
        [Alias("ub")]
        [Summary("Defines a given word\nUsage: $UrbanDefine <word>")]
        public async Task DefineWordUrban([Remainder] string word)
        {
            var urbanDefinition = await _urbanService.GetAsync(word);
            var embed = new EmbedBuilder();

            try
            {
                embed.WithAuthor(Context.Client.CurrentUser.Username, Context.Client.CurrentUser.GetAvatarUrl());
                embed.Title = "Urban definition";
                embed.Description = $"Word: {word}";

                embed.WithColor(Color.DarkMagenta);
                embed.WithFooter("Post made at ");
                embed.WithCurrentTimestamp();
                embed.WithThumbnailUrl(
                    "https://apprecs.org/gp/images/app-icons/300/28/xyz.sayangoswami.urbandictionary.jpg");

                embed.AddField($"Author", urbanDefinition.Author);
                string definition = urbanDefinition.Definition.Replace("[", "**").Replace("]", "**");
                embed.AddField("Definition", definition);
                string example = urbanDefinition.Example.Replace("[", "*").Replace("]", "*");
                embed.AddField("Example", example);

                await ReplyAsync(Context.User.Mention, false, embed.Build());
            }
            catch
            {
                await ReplyAsync("Wat");
            }
        }

        [Command("Corona", RunMode = RunMode.Async)]
        [Summary("Get the latest data about country")]
        public async Task CoronaGet(string country)
        {
            try
            {
                var result = await _coronaApi.GetAsync(country);

                var embed = new EmbedBuilder();

                var nfi = new CultureInfo("ru-RU", false).NumberFormat;
                nfi.NumberDecimalDigits = 0;

                embed.WithAuthor(Context.Client.CurrentUser.Username, Context.Client.CurrentUser.GetAvatarUrl());
                embed.Title = "Covid-19 data";
                embed.Description = $"Country: {country}";

                embed.WithColor(Color.DarkRed);
                embed.WithFooter("Post made at ");
                embed.WithCurrentTimestamp();
                embed.WithThumbnailUrl("https://rcre.opolskie.pl/wp-content/uploads/sites/3/2020/03/koronawirus.png");

                embed.AddField("code: ", result.Code.ToString(nfi));
                embed.AddField("Confirmed: ", result.Confirmed.ToString("N", nfi));
                embed.AddField("Recovered: ", result.Recovered.ToString("N", nfi));
                embed.AddField("Critical: ", result.Critical.ToString("N", nfi));
                embed.AddField("Deaths: ", result.Deaths.ToString("N", nfi));
                embed.AddField("Active: ",
                    (result.Confirmed - result.Deaths - result.Recovered).ToString("N", nfi));
                embed.AddField("Last update: ", result.LastUpdate);

                await ReplyAsync(Context.User.Mention, false, embed.Build());
            }
            catch
            {
                await ReplyAsync("Something went wrong?");
                return;
            }
        }

        [Command("UwU", RunMode = RunMode.Async)]
        [Summary("UwU the text\nUsage: $UwU text")]
        public async Task UwU([Remainder] string uwuText)
        {
            uwuText = uwuText.Replace('R', 'W');
            uwuText = uwuText.Replace('r', 'w');
            uwuText = uwuText.Replace("ove", "uv");
            uwuText = uwuText.Replace("Ove", "Uv");
            uwuText = uwuText.Replace("l", "w");
            uwuText = uwuText.Replace("m", "mw");
            uwuText += " OwO";

            await Context.Message.DeleteAsync();
            await ReplyAsync($"{Context.User.Username}: {uwuText}");
        }

        [Command("Love")]
        [Summary("Check how much you're lovely match with somebody\nUsage: $love @user @user")]
        public async Task Love(IUser user1, IUser user2)
        {
            var rnd = new Random();
            if (user1.Id == 215556401467097088 || user2.Id == 215556401467097088)
            {
                await ReplyAsync($"{user1.Mention} + {user2.Mention} = {rnd.Next(1, 2)}% luv");
            }
            else
            {
                await ReplyAsync($"{user1.Mention} + {user2.Mention} = {rnd.Next(1, 100)}% luv");
            }
        }

        [Command("Time")]
        [Summary("Check the CEST/CET time")]
        public async Task CestZone()
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Current CET time");
            embed.WithDescription($"**{DateTime.Now:HH:mm}**");
            embed.WithColor(Color.DarkMagenta);
            await ReplyAsync("", false, embed.Build());
        }

        /*[Command("gif", RunMode = RunMode.Async)]
        [Summary("Work in progress")]
        public async Task Teste([Remainder] string tagsInput)
        {
            var tags = tagsInput.ToLower().Replace(" ", "+");
            var requestUrl =
                $"http://api.giphy.com/v1/gifs/random?tag={tags}&api_key=B1QVxhJOi6f9W1HgvfAwr1j7aVWn6y6f&limit=1&rating=r";
            var request = await new HttpClient().GetStringAsync(requestUrl);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Request Giphy: {requestUrl}");
            Console.ResetColor();

            var result = JsonConvert.DeserializeObject<GiphyResult>(request);
            var embed = new EmbedBuilder();
            embed.WithColor(Color.DarkMagenta);
            embed.WithImageUrl(result.Data.Images.Original.Url.ToString());
            await ReplyAsync("", false, embed.Build());
        }*/

        [Command("Download", RunMode = RunMode.Async)]
        public async Task DownloadSite(string link)
        {
            var path = Directory.GetCurrentDirectory() + "/temp/";
            var client = new HttpClient();
            var result = await client.GetStringAsync(link);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText($"{path}/site.html", result);
            await Context.Channel.SendFileAsync($"{path}/site.html");
            File.Delete($"{path}/site.html");
        }

        [Command("EggPlant", RunMode = RunMode.Async)]
        [Summary("Show how big your eggplant is!")]
        public async Task EggPlant()
        {
            var user = await _miscDb.GetOrCreateUser(Context.User.Id);
            if (user.EggPlant == 0)
            {
                Random rnd = new Random();

                int luck = rnd.Next(1, 100);
                int mainSize = 0;
                //smoll
                if (luck < 15)
                {
                    mainSize = rnd.Next(3, 10);
                }
                //avarage 
                else if (luck >= 15 && luck <= 75)
                {
                    mainSize = rnd.Next(10, 16);
                }
                //big
                else if (luck > 75 && luck < 95)
                {
                    mainSize = rnd.Next(16, 25);
                }
                //huge
                else if (luck >= 95)
                {
                    mainSize = rnd.Next(25, 40);
                }

                user.EggPlant = float.Parse($"{mainSize}.{rnd.Next(1, 9)}", CultureInfo.InvariantCulture.NumberFormat);
                await ReplyAsync($"{Context.User.Mention} eggplant has {user.EggPlant} cm! :eggplant:");
                _db.UserData.Update(user);
                await _db.SaveChangesAsync();
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention} eggplant has {user.EggPlant} cm! :eggplant:");
            }
        }

        [Command("Grow")]
        [Summary("Grow your eggplant")]
        public async Task GrowEggplant()
        {
            var user = await _miscDb.GetOrCreateUser(Context.User.Id);
            if (user.EggplantDaily == true)
            {
                var nextGrow = new TimeSpan(24, 0, 0) - DateTime.Now.TimeOfDay;
                await ReplyAsync(
                    $"You already did grow your eggplant today. you can grow it again in {nextGrow.Hours} hours {nextGrow.Minutes} minutes"
                );
                return;
            }

            if (user.EggPlant == 0)
            {
                Random rnd = new Random();

                int luck = rnd.Next(1, 100);
                int mainSize = 0;
                //smoll
                if (luck < 15)
                {
                    mainSize = rnd.Next(3, 10);
                }
                //avarage 
                else if (luck >= 15 && luck <= 75)
                {
                    mainSize = rnd.Next(10, 16);
                }
                //big
                else if (luck > 75 && luck < 95)
                {
                    mainSize = rnd.Next(16, 25);
                }
                //huge
                else if (luck >= 95)
                {
                    mainSize = rnd.Next(25, 40);
                }

                user.EggPlant = float.Parse($"{mainSize}.{rnd.Next(1, 9)}", CultureInfo.InvariantCulture.NumberFormat);

            }

            user.EggPlant += 0.1f;
            user.EggplantDaily = true;
            _db.UserData.Update(user);
            await _db.SaveChangesAsync();
            await ReplyAsync($"You grew your eggplant by 0.1cm! It has {user.EggPlant} cm now! :eggplant:");
        }

        public Task<GuildEmote> GetEmote(string name)
        {
            var emote = Context.Client.Guilds
                .SelectMany(x => x.Emotes)
                .FirstOrDefault(x => x.Name.IndexOf(
                    name, StringComparison.OrdinalIgnoreCase) != -1);
            return Task.FromResult(emote);
        }
    }
}