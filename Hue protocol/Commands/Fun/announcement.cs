using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HueProtocol.Tools;
using Microsoft.Extensions.Logging;

namespace HueProtocol.Commands.Fun {
    [Name("Announcement")]
    public class Announcement : ModuleBase<SocketCommandContext> {
        private readonly ILogger<Announcement> _logger;
        private readonly MiscDatabaseMethodsService _misc;


        private const string defaultRaidPicture = "https://i.pinimg.com/originals/ef/0b/5c/ef0b5cd2663c4412a71e13642fb1f2da.jpg";
        private const string defaultPartyPicture = "https://i.pinimg.com/originals/87/aa/84/87aa84cd79747d3d8adb50a7818aa72d.jpg";

        public Announcement(MiscDatabaseMethodsService misc, ILogger<Announcement> logger) {
            _logger = logger;
            _misc = misc;
        }

        [Command("Raid", RunMode = RunMode.Async)]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [Summary(
            "Set up the raid! The user limit is optional parameter\nUsage: $Raid [title] [time] [details] [Player limit]")]
        public async Task CreateRaid(string title, string time, string details, int playerLimit = 12) {
            await _misc.CreatePost(title, time, details, "Raid", Context, playerLimit, defaultRaidPicture);
        }

        [Command("Party", RunMode = RunMode.Async)]
        [Summary(
            "Set up the party! The user limit is optional parameter\nUsage: $Party [time] [details] [Player limit]")]
        public async Task CreateDaily(string time, string details, int playerLimit = 6) {
            await _misc.CreatePost("Party run!", time, details, "Daily", Context, playerLimit, defaultPartyPicture);
        }

        [Command("Announce", RunMode = RunMode.Async)]
        [Alias("ann")]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [Summary(
            "Create custom announcement. Thumbnail is optional and it's your avatar by default\nUsage: $Announce [title] [description] [thumbnailUrl]")]
        public async Task Announce(string title = "title", string description = "description",
                                   string thumbnailUrl = null) {
            if (thumbnailUrl == null)
                thumbnailUrl = Context.User.GetAvatarUrl();

            var emb = new EmbedBuilder();
            emb.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
            emb.Title = title;
            emb.Description = description;
            emb.ThumbnailUrl = thumbnailUrl;
            emb.WithColor(Color.DarkPurple);
            emb.WithFooter(@"Post made at ");
            emb.WithCurrentTimestamp();

            await Context.Message.DeleteAsync();
            await ReplyAsync("", false, emb.Build());
        }
    }
}