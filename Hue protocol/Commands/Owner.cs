using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HueProtocol.Database;
using HueProtocol.models;
using HueProtocol.Tools;
using HueProtocol.Events;
using HueProtocol.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Commands
{
    public class Owner : ModuleBase<SocketCommandContext>
    {
        private readonly EFContext _db;
        private readonly CommandService _commands;
        private readonly ILogger _logger;
        private readonly Configuration _configuration;
        private readonly MiscDatabaseMethodsService _misc;
        private readonly DatabaseTimeEvents _events;
        private readonly GearDatabase _gearDatabase;

        public Owner(EFContext db, CommandService commands, ILogger<Owner> logger, Configuration configuration, MiscDatabaseMethodsService misc, DatabaseTimeEvents events, GearDatabase gearDatabase)
        {
            this._db = db;
            this._commands = commands;
            _misc = misc;
            _logger = logger;
            _configuration = configuration;
            _events = events;
            _gearDatabase = gearDatabase;
        }

        [Command("say")]
        [RequireOwner]
        public async Task Ping([Remainder] string response)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync(response);
        }

        [Command("SetStatus")]
        [RequireOwner]
        [Summary("Sets the status of the bot")]
        public async Task SetStatus(string status)
        {
            await Context.Client.SetGameAsync(status, null, ActivityType.Playing);
            await Context.Message.DeleteAsync();
            await ReplyAsync("New status for the bot!");
        }

        [Command("GenerateCommandList", RunMode = RunMode.Async)]
        [Alias("gcl")]
        [RequireOwner]
        [Summary("Extracts commands to html")]
        public async Task ListCommands()
        {
            var AdminCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Admin").ToList();
            var FunCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Fun").ToList();
            var AnnouncementCommands =
                _commands.Commands.Where(x => x.Module.Name.ToString() == "Announcement").ToList();
            var MusicCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Audio").ToList();
            var Other = _commands.Commands.Where(x => x.Module.Name.ToString() == "Other").ToList();
            var Economy = _commands.Commands.Where(x => x.Module.Name.ToString() == "Economy").ToList();

            var Generator = new List<List<CommandInfo>> {
                AdminCommands,
                FunCommands,
                AnnouncementCommands,
                MusicCommands,
                Other,
                Economy
            };

            string[] ids = { "Admin", "Fun", "Announcement", "Audio", "Other", "Economy" };

            var generated = new StringBuilder();

            foreach (var item in Generator)
            {
                generated.AppendLine($"<!-- {item[0].Module.Name} -->");

                if (ids.Any(x => x == item[0].Module.Name))
                    generated.AppendLine($"<div class=\"bot-main\" id=\"{item[0].Module.Name.ToLower()}\">");
                else
                    return;

                foreach (var subItem in item)
                {
                    var summary = "";
                    if (!string.IsNullOrWhiteSpace(subItem.Summary))
                    {
                        summary = subItem.Summary.Replace('<', '[');
                        summary = summary.Replace('>', ']');
                    }

                    summary = summary.Replace("\n", "<br>");
                    generated.AppendLine("<div class=\"main-item\">");
                    generated.AppendLine($"<div class=\"item-command\">${subItem.Name}</div>");
                    if (subItem.Aliases.Count > 1)
                        generated.AppendLine($"<div class=\"item-alias\">Alias: ${subItem.Aliases[1]}</div>");
                    generated.AppendLine($"<div class=\"item-description\">{summary}</div>");
                    generated.AppendLine("</div>");
                }

                generated.AppendLine("</div>");
                generated.AppendLine($"<!-- End {item[0].Module.Name} -->");
            }

            var toSave = generated.ToString();

            if (!Directory.Exists($"{Directory.GetCurrentDirectory()}/commands"))
                Directory.CreateDirectory($"{Directory.GetCurrentDirectory()}/commands");
            File.WriteAllText($"{Environment.CurrentDirectory}/Commands/commands.html", toSave);

            await ReplyAsync("Done <:11:702505653675229256>");
        }

        [Command("RemoveUser", RunMode = RunMode.Async)]
        [RequireOwner]
        public async Task RemoveUser(ulong id)
        {
            var user = await _db.UserData.FirstOrDefaultAsync(x => x.UserId == id);
            _db.UserData.Remove(user);
            await _db.SaveChangesAsync();
            await ReplyAsync("User deleted");

            _logger.LogInformation($"Removed user with id {id}");
        }

        [Command("servers")]
        [RequireOwner]
        [Summary("Lists servers with this bot")]
        public async Task ServerList()
        {
            var sb = new StringBuilder();
            var guilds = Context.Client.Guilds.ToList();
            foreach (var g in guilds)
            {
                sb.AppendLine($"Name: {g.Name}, ID: {g.Id}, Owner: {g.Owner}");
            }

            await ReplyAsync(sb.ToString());
        }

        [Command("ping")]
        [RequireOwner]
        [Summary("Check the bot latency")]
        public async Task Ping()
        {
            var pingTime = $"Ping: {Context.Client.Latency}ms";
            await ReplyAsync(pingTime);
        }

        //TODO
        [Command("ConfigureBot", RunMode = RunMode.Async)]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        [Summary(
            "Sets the variables and uploads or updates them to database\nThis commands runs automatically after joining to server")]
        public async Task ConfigureBot()
        {
            var exist = _db.ServersSettings.Any(server => server.ServerId == Context.Guild.Id);
            if (!exist)
            {
                //Adds server to database
                var server = new ServerGuild()
                {
                    ServerId = Context.Guild.Id,
                    ServerName = Context.Guild.Name,
                    OwnerId = Context.Guild.OwnerId,
                    OwnerName = Context.Guild.Owner.Username,
                    prefix = _configuration.Prefix,
                    DoGreet = false
                };

                _db.ServersSettings.Add(server);
                await _db.SaveChangesAsync();
                await ReplyAsync("Server added to H.u.e database");
            }
            else
            {
                //Updates server 
                var server =
                    await _db.ServersSettings.FirstOrDefaultAsync(server => server.ServerId == Context.Guild.Id);
                server.ServerId = Context.Guild.Id;
                server.ServerName = Context.Guild.Name;
                server.OwnerId = Context.Guild.OwnerId;
                server.OwnerName = Context.Guild.Owner.Username;
                _db.ServersSettings.Update(server);
                await _db.SaveChangesAsync();
                await ReplyAsync("Server updated to H.u.e database");
            }
        }

        //TODO
        [Command("ConfigureRolePost")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Summary("Configures the listening for role post reactions (Require channel ID)")]
        public async Task ConfigureRole(ulong post)
        {
            var server = await _misc.GetOrCreateServer(Context.Guild.Id);
            server.RolePostId = post;
            _db.ServersSettings.Update(server);
            await _db.SaveChangesAsync();
        }

        [Command("BdTime")]
        [RequireOwner]
        public async Task ForceBirthday()
        {
            await _events.BirthdayEvent();
            await ReplyAsync("Done");
        }

        [Command("ForceDailyReset")]
        [RequireOwner]
        public async Task ForceDaily()
        {
            await _events.BirthdayEvent();
            await _events.ClearOldPosts();
            await _events.CoinReset();
        }

        [Command("AddItem")]
        [Summary("Usage: $AddItem <string name> <string description> <string userClass> <string itemType>\n<int score> <int damage> <int lifeSteal> <int attackSpeed> <int magicDamage>\n<int armor> <int magicResistance> <int health> <int price>")]
        [RequireOwner]
        public async Task AddItem(string name, string description, string userClass, string itemType,
                int score, int damage, int lifeSteal, int attackSpeed, int magicDamage,
                int armor, int magicResistance, int health, int price)
        {
            //check if item type exists
            if (itemType.ToLower() != ItemTypes.Armor && itemType.ToLower() != ItemTypes.Weapon)
            {
                await ReplyAsync("There's no item type like this");
                return;
            }

            //check if class exists
            var dbClass = await _db.userclass.FirstOrDefaultAsync(e => e.Name.ToLower() == userClass.ToLower());
            if (dbClass == null)
            {
                await ReplyAsync("This class doesn't exist");
                return;
            }

            //check if item already exists
            var itemExists = _db.Items.Any(e => e.Name == name);
            if (itemExists)
            {
                await ReplyAsync("This item already exist");
                return;
            }

            //create attributes
            var itemAttribute = new ItemAttribute()
            {
                Score = score,
                Damage = damage,
                LifeSteal = lifeSteal
            };

            var item = new Item()
            {
                Name = name,
                Description = description,
                UserClass = dbClass,
                ItemAttribute = itemAttribute,
                ItemType = itemType,
                Price = price
            };

            await _db.Items.AddAsync(item);
            await _db.SaveChangesAsync();
            await ReplyAsync("Item added!");
            _logger.LogInformation("New item added");
        }

        [Command("AdminGiveItem")]
        [Summary("Gives item via admin profile")]
        [RequireOwner]
        public async Task AdminGiveItemAsync(IUser usr, string name)
        {
            var item = await _db.Items.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
            if(item == null)
            {
                await ReplyAsync("This item doesn't exist");
                return;
            }
            var user = await _gearDatabase.GetUserWithEquipmentAsync(usr.Id);
            
            var newEq = new Equipment()
            {
                Owner = user,
                Item = item
            };

            // var itemList = user.Equipment.ToList();
            // itemList.Add(newEq);
            user.Equipment.Add(newEq);
            _db.UserData.Update(user);
            await _db.SaveChangesAsync();
            await ReplyAsync($":crossed_swords:  {usr.Mention} granted **{name}**  :crossed_swords:");
        }

        [Command("RemoveItem")]
        [Summary("Usage: $RemoveItem <string name>")]
        [RequireOwner]
        public async Task RemoveItem(string name)
        {
            var item = await _db.Items.FirstOrDefaultAsync(x => x.Name == name);
            if (item == null)
            {
                await ReplyAsync("This Item doesn't exist");
                return;
            }

            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }
        
        [Command("AdminResetClass")]
        [RequireOwner]
        public async Task AdminResetClass(IUser user)
        {
            var gear = await _db.gear.FirstOrDefaultAsync(x => x.UserId == user.Id);
            gear.UserClass = await _db.userclass.FirstOrDefaultAsync(x => x.Name == UserClasses.Pleb);
            _db.gear.Update(gear);
            await _db.SaveChangesAsync();
            await ReplyAsync("Class got resetted");
        }
    }
}