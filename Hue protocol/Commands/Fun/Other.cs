using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HueProtocol.Tools;

namespace HueProtocol.Commands.Fun
{
    [Name("Other")]
    public class Other : ModuleBase<SocketCommandContext>
    {
        private DiscordShardedClient _client;
        private EFContext _database;
        private MiscDatabaseMethodsService _misc;
        private readonly string[] supportedLinks = new string[]
        {
            "bns", "steam", "twitch"
        };

        public Other(DiscordShardedClient client, EFContext database, MiscDatabaseMethodsService misc)
        {
            _client = client;
            _database = database;
            _misc = misc;
        }

        [Command("Invite")]
        [Summary("Get invite link for bot")]
        public async Task InviteLink()
        {
            var embed = new EmbedBuilder();
            IUser creator = _client.GetUser(215556401467097088);
            embed.WithAuthor(creator.Username, creator.GetAvatarUrl());
            embed.WithTitle("Want to invite me to your server? Click the link below!");
            embed.WithDescription("https://discord.com/api/oauth2/authorize?client_id=704857702953779210&permissions=2146958454&scope=bot");
            embed.WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl());
            embed.WithColor(Color.Teal);
            embed.WithFooter("Bot made with love by Hue 🤍");

            await ReplyAsync("", false, embed.Build());
        }

        [Command("Link")]
        public async Task LinkAccount(string linkedService, string link)
        {
            if (supportedLinks.Any(x => x.Equals(linkedService, StringComparison.OrdinalIgnoreCase)))
            {
                LinkedAccount acc = new LinkedAccount
                {
                    User = await _database.UserData.FirstOrDefaultAsync(usr => usr.UserId == Context.User.Id),
                    LinkedServiceLink = link,
                    Type = linkedService
                };
            }
            else
            {
                await ReplyAsync("This service is not supported for linking");
                return;
            }
        }

        [Command("Birthday")]
        [Summary("Add your birthday!")]
        public async Task Birthday(DateTime date)
        {
            var birthday = await _database.Birthday.FirstOrDefaultAsync(owner => owner.UserId == Context.User.Id);

            var newBirthday = new Birthday
            {
                Date = date,
                DoDisplay = true,
                UserId = Context.User.Id,
                User = await _misc.GetOrCreateUser(Context.User.Id)
            };

            try
            {
                if(birthday != null)
                {
                    _database.Birthday.Update(newBirthday);
                    await ReplyAsync("Your birthday has been updated!");
                }
                else
                {
                    await _database.Birthday.AddAsync(newBirthday);
                    await ReplyAsync("Your birthday has been added!");
                }
                await _database.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("ToggleBirthday")]
        [Summary("Turn on/off displaying your birthday")]
        public async Task BirthdaySwitch()
        {
            var user = await _misc.GetOrCreateUser(Context.User.Id);
            if (user.BirthdayDate == null)
            {
                await ReplyAsync("You didn't add your birthday yet");
                return;
            }

            user.BirthdayDate.DoDisplay = !user.BirthdayDate.DoDisplay;
            _database.UserData.Update(user);
            await _database.SaveChangesAsync();

            if (user.BirthdayDate.DoDisplay)
            {
                await ReplyAsync("I will display your birthday now");
            }
            else
            {
                await ReplyAsync("I won't display your birthday now");
            }
        }

    }
}
