using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace HueProtocol.Commands
{
    [Name("Others")]
    public class Help : ModuleBase<SocketCommandContext>
    {
        private CommandService _commands;        
        private DiscordShardedClient _client;

        public Help(CommandService commands, DiscordShardedClient client)
        {
            _commands = commands;
            _client = client;            
        }

        [Command("HelpOld", RunMode = RunMode.Async)]
        public async Task HelpMe()
        {
            var color = Color.Purple; //Default color for embed
            var avatarUrl = _client.CurrentUser.GetAvatarUrl();//Client avatar
            var creatorAvatar = _client.GetUser(215556401467097088).GetAvatarUrl();
            string creatorUsername = _client.GetUser(215556401467097088).Username;

            //Get commands from modules
            var AdminCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Admin").ToList();
            var FunCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Fun").ToList();
            var AnnouncementCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Announcement").ToList();
            var MusicCommands = _commands.Commands.Where(x => x.Module.Name.ToString() == "Audio").ToList();

            //Get user and create embed
            IUser creator = _client.GetUser(215556401467097088);
            var embed = new EmbedBuilder
            { 
                Author = new EmbedAuthorBuilder
                { 
                    Name = creatorUsername,
                    IconUrl = creatorAvatar
                },
                Title = "Hue comes to help",
                Description = "Command list",
                ThumbnailUrl = _client.CurrentUser.GetAvatarUrl(),
                Color = color,
                Footer = new EmbedFooterBuilder
                { 
                    Text = "Bot made with love by Hue 🤍"
                }
            };

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
                var emb = new EmbedBuilder
                {
                    ThumbnailUrl = avatarUrl,
                    Color = color,
                    Title = "Your help is here! I've sent you quick command list to DM's",
                    Timestamp = DateTimeOffset.Now,
                    Footer = new EmbedFooterBuilder
                    { 
                        Text = $"Help requested by {Context.User.Username}"
                    },
                    Fields = new List<EmbedFieldBuilder>
                    { 
                        new EmbedFieldBuilder
                        { 
                            Name = "Website:",
                            Value = "https://huebyte.github.io/Bot"
                        },
                        new EmbedFieldBuilder
                        { 
                            Name = "Invite link:",
                            Value = "https://discord.com/api/oauth2/authorize?client_id=704857702953779210&permissions=8&scope=bot"
                        }
                    }
                };
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
            var color = Color.Purple; //Default color for embed
            var avatarUrl = _client.CurrentUser.GetAvatarUrl();//Client avatar
            var creatorAvatar = _client.GetUser(215556401467097088).GetAvatarUrl();
            string creatorUsername = _client.GetUser(215556401467097088).Username;

            if (command == null)
            {
                try
                {

                    var emb = new EmbedBuilder
                    {
                        ThumbnailUrl = avatarUrl,
                        Color = color,
                        Title = "Your help is here! I've sent you quick command list to DM's",
                        Timestamp = DateTimeOffset.Now,
                        Footer = new EmbedFooterBuilder
                        { 
                            Text = $"Help requested by {Context.User.Username}"
                        },
                        Fields = new List<EmbedFieldBuilder>
                        { 
                            new EmbedFieldBuilder
                            { 
                                Name = "Website:",
                                Value = "https://huebyte.github.io/Bot"
                            },
                            new EmbedFieldBuilder
                            { 
                                Name = "Invite link:",
                                Value = "https://discord.com/api/oauth2/authorize?client_id=704857702953779210&permissions=8&scope=bot"
                            },
                            new EmbedFieldBuilder
                            { 
                                Name = "Additional info",
                                Value = "You can also use the $help <command> for details about specific command"
                            }
                        }
                    };                    
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
