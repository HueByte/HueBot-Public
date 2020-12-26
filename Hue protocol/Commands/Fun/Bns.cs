using System;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HueProtocol.Services.APIs;
using HueProtocol.Services.APIs.Results;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HueProtocol.Commands.Fun
{
    [Name("Other")]
    public class Bns : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _logger;
        private readonly BnsService _bnsApi;

        public Bns(BnsService bnsApi, ILogger<Bns> logger)
        {
            _logger = logger;
            _bnsApi = bnsApi;
        }

        [Command("BnsGear", RunMode = RunMode.Async)]
        [Summary("Fetch the gear by character nickname from EU server\nUsage: $BnsGear <NameOfCharacter>")]
        public async Task GetGear([Remainder] string character)
        {
            BnsGearResult gear;

            try
            {
                gear = await _bnsApi.GetAsync(character);

                var embed = new EmbedBuilder();

                string characterFormatted = character.Replace(" ", "+");

                embed.WithAuthor(Context.User.Username, Context.User.GetAvatarUrl());
                embed.Title = character;
                embed.WithUrl($"https://bns.life/char/eu/{characterFormatted}");
                embed.Description = $"{character} - {gear.AccountName} - {gear.Guild} - {gear.Faction}";
                embed.WithColor(Color.DarkMagenta);
                embed.WithFooter("Post made at ");
                embed.WithCurrentTimestamp();
                if (gear.CharacterImg.ToString() != null)
                    embed.WithThumbnailUrl(gear.CharacterImg.ToString());
                else
                    embed.WithThumbnailUrl("https://cdn0.iconfinder.com/data/icons/esports-filloutline/64/MMORPG-weapon-interface-player-internet-512.png");

                //weapon
                var gearWeapon = new StringBuilder();
                gearWeapon.AppendLine($"Weapon: {gear.WeaponName}\n");
                gearWeapon.AppendLine($"Gems: \n{gear.Gem1}\n{gear.Gem2}\n{gear.Gem3}\n{gear.Gem4}\n{gear.Gem5}\n{gear.Gem6}\n{gear.Gem7}\n{gear.Gem8}\n\n");

                //rest
                var gearOther = new StringBuilder();
                gearOther.AppendLine($"Ring: {gear.RingName}");
                gearOther.AppendLine($"Earring: {gear.EarringName}");
                gearOther.AppendLine($"Necklace: {gear.NecklaceName}");
                gearOther.AppendLine($"Bracelet: {gear.BraceletName}");
                gearOther.AppendLine($"Belt: {gear.BeltName}");
                gearOther.AppendLine($"Soul: {gear.SoulName}");
                gearOther.AppendLine($"Pet Aura: {gear.PetAuraName}");
                gearOther.AppendLine($"Talisman: {gear.TalismanName}");
                gearOther.AppendLine($"Mystic Badge: {gear.MysticBadgeName}");
                gearOther.AppendLine($"Soul Badge: {gear.SoulBadgeName}");
                gearOther.AppendLine($"Gloves: {gear.Gloves}");
                gearOther.AppendLine($"Outfit: {gear.OutfitName}");

                //soulshield
                var soulshield = new StringBuilder();
                soulshield.AppendLine($"{gear.Soulshield1}");
                soulshield.AppendLine($"{gear.Soulshield2}");
                soulshield.AppendLine($"{gear.Soulshield3}");
                soulshield.AppendLine($"{gear.Soulshield4}");
                soulshield.AppendLine($"{gear.Soulshield5}");
                soulshield.AppendLine($"{gear.Soulshield6}");
                soulshield.AppendLine($"{gear.Soulshield7}");
                soulshield.AppendLine($"{gear.Soulshield8}");

                //stats
                var stats = new StringBuilder();
                stats.AppendLine($"Attack power: {gear.Ap}");
                stats.AppendLine($"Heal points: {gear.Hp}");
                stats.AppendLine($"Critical hit: {gear.Crit} {gear.CritDamageRate}");
                stats.AppendLine($"Critical damage: {gear.CritDamage}");
                stats.AppendLine($"Mystic: {gear.Mystic}");

                //embed build
                embed.AddField("SoulShield: ", soulshield.ToString(), true);
                embed.AddField("Stats: ", stats.ToString(), true);
                embed.AddField("Class: ", gear.PlayerClass, true);
                embed.AddField("Weapon: ", gearWeapon.ToString(), true);
                embed.AddField("Gear: ", gearOther.ToString(), true);

                await ReplyAsync("", false, embed.Build());
            }
            catch
            {
                await ReplyAsync("Couldn't find this character, make sure it's from EU server");
            }
        }
    }
}
