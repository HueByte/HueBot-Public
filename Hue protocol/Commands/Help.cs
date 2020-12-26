using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Commands
{
    [Name("Others")]
    public class Help : ModuleBase<SocketCommandContext>
    {
        private CommandService _commands;
        private readonly ILogger<Help> _logger;
        private DiscordShardedClient _client;

        public Help(CommandService commands, DiscordShardedClient client, ILogger<Help> logger)
        {
            _commands = commands;
            _client = client;
            _logger = logger;
        }

        [Command("HelpOld", RunMode = RunMode.Async)]
        public async Task HelpMe()
        {
            //Get commands from modules
            var AdminCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Admin").ToList();
            var FunCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Fun").ToList();
            var AnnouncementCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Announcement").ToList();
            var MusicCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Audio").ToList();

            //Get user and create embed
            IUser creator = _client.GetUser(215556401467097088);
            var embed = new EmbedBuilder();
            embed.WithAuthor(creator.Username, creator.GetAvatarUrl());
            embed.Title = "Hue comes to help";
            embed.Description = $"Command list";
            embed.ThumbnailUrl = $"{_client.CurrentUser.GetAvatarUrl()}";
            embed.WithColor(Color.DarkPurple);
            embed.WithFooter("Bot made with love by Hue 🤍");

            #region Command Loops

            var adminCom = new StringBuilder();
            foreach (var com in AdminCommands)
            {
                adminCom.AppendLine("- " + com.Name);
                adminCom.AppendLine(com.Summary + "\n");
            }
            var funCom = new StringBuilder();
            foreach (var com in FunCommands)
            {
                funCom.AppendLine("- " + com.Name);
                funCom.AppendLine(com.Summary + "\n");
            }
            var announCom = new StringBuilder();
            foreach (var com in AnnouncementCommands)
            {
                announCom.AppendLine("- " + com.Name);
                announCom.AppendLine(com.Summary + "\n");
            }
            var musicCom = new StringBuilder();
            foreach (var com in MusicCommands)
            {
                musicCom.AppendLine("- " + com.Name);
                musicCom.AppendLine(com.Summary + "\n");
            }

            #endregion

            embed.AddField("Admin: ", adminCom.ToString());
            embed.AddField("Fun commands: ", funCom.ToString());
            embed.AddField("Announcement Commands: ", announCom.ToString());

            try
            {
                var emb = new EmbedBuilder();
                emb.WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl());
                emb.WithColor(Color.DarkPurple);
                emb.WithTitle("Your help is here! I've sent you quick command list to DM's");
                emb.WithCurrentTimestamp();
                emb.WithFooter($"Help requested by {Context.User.Username}");
                emb.AddField("Website: ", "https://huebyte.github.io/Bot");
                emb.AddField("Invite link: ", "https://discord.com/api/oauth2/authorize?client_id=704857702953779210&permissions=8&scope=bot");

                await ReplyAsync("", false, emb.Build());
                await Context.User.SendMessageAsync("", false, embed.Build());
            }
            catch
            {
                await ReplyAsync("I can't send you DM");
            }
        }

        [Command("Help", RunMode = RunMode.Async)]
        [Summary("Shows the help you can also use $Help <command>")]
        public async Task Helper(string command = null)
        {
            if (command == null)
            {
                try
                {
                    var emb = new EmbedBuilder();
                    emb.WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl());
                    emb.WithColor(Color.DarkPurple);
                    emb.WithTitle("Your help is here!");
                    emb.WithCurrentTimestamp();
                    emb.WithFooter($"Help requested by {Context.User.Username}");
                    emb.AddField("Website: ", "https://huebyte.github.io/Bot");
                    emb.AddField("Invite link: ", "https://discord.com/api/oauth2/authorize?client_id=704857702953779210&permissions=2146958454&scope=bot");
                    emb.AddField("Additional info", "You can also use the $help <command> for details about specific command");
                    await ReplyAsync("", false, emb.Build());
                }
                catch
                {
                    await ReplyAsync("Something went wrong");
                }
            }
            else
            {
                var cmd = _commands.Commands.FirstOrDefault(com => com.Name.ToLower() == command.ToLower()) as CommandInfo;
                if(cmd == null)
                {
                    await ReplyAsync("That command doesn't exist");
                    return;
                }
                var emb = new EmbedBuilder();
                emb.WithTitle($"${command}");
                emb.WithColor(Color.DarkPurple);
                emb.WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl());
                emb.WithDescription($"{cmd.Summary}");
                var sb = new StringBuilder();

                await ReplyAsync($"", false, emb.Build());
            }
        }
    }
}
