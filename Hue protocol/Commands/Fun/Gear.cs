using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HueProtocol.models;
using HueProtocol.Database;
using HueProtocol.Tools;
using HueProtocol.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HueProtocol.Commands.Fun
{
    [Name("Gear")]
    public class Gear : ModuleBase<SocketCommandContext>
    {
        private readonly EFContext _database;
        private readonly MiscDatabaseMethodsService _misc;
        private readonly GearDatabase _gearDatabase;
        public Gear(EFContext database, MiscDatabaseMethodsService misc, GearDatabase gearDatabase)
        {
            _database = database;
            _misc = misc;
            _gearDatabase = gearDatabase;
        }

        [Command("GetGear")]
        [Summary("Get your gear")]
        public async Task GetGear()
        {
            var gear = await _gearDatabase.GetOrCreateGearAsync(Context.User.Id);
            var embed = new EmbedBuilder();
            embed.WithAuthor($"{Context.User.Username}");
            embed.WithColor(Discord.Color.Green);
            embed.AddField("Class", gear.UserClass.Name);
            embed.AddField("Gear Score", gear.GearScore);
            embed.AddField("Coins", gear.User.Coins);
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            if (gear.ItemWeapon == null)
                embed.AddField("weapon", "None");
            else
                embed.AddField("Weapon", gear.ItemWeapon.Name);
            if (gear.ItemArmor == null)
                embed.AddField("Armor", "None");
            else
                embed.AddField("Armor", gear.ItemArmor.Name);

            await ReplyAsync("", false, embed.Build());
        }

        [Command("Stats")]
        [Summary("Get your gear statistics")]
        public async Task GetStatsAsync()
        {
            var gear = await _gearDatabase.GetOrCreateGearAsync(Context.User.Id);
            var embed = new EmbedBuilder();
            embed.WithAuthor($"{Context.User.Username}");
            embed.WithColor(Discord.Color.Magenta);
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            if (gear.ItemWeapon == null)
            {
                embed.AddField("weapon", "None");
                gear.ItemWeapon = new Item()
                {
                    Name = "None",
                    ItemAttribute = new ItemAttribute()
                };
            }
            else
                embed.AddField("Weapon", gear.ItemWeapon.Name);
            if (gear.ItemArmor == null)
            {
                gear.ItemArmor = new Item()
                {
                    Name = "None",
                    ItemAttribute = new ItemAttribute()
                };
            }
            else
                embed.AddField("Armor", gear.ItemArmor.Name);
            
            embed.AddField("Gear score", gear.GearScore);
            embed.AddField("Damage", gear.ItemArmor.ItemAttribute.Damage + gear.ItemWeapon.ItemAttribute.Damage);
            embed.AddField("LifeSteal", gear.ItemArmor.ItemAttribute.LifeSteal + gear.ItemWeapon.ItemAttribute.LifeSteal);
            embed.AddField("Attack Speed", gear.ItemArmor.ItemAttribute.AttackSpeed + gear.ItemWeapon.ItemAttribute.AttackSpeed);
            embed.AddField("Magic Damage", gear.ItemArmor.ItemAttribute.MagicDamage + gear.ItemWeapon.ItemAttribute.MagicDamage);
            embed.AddField("Armor", gear.ItemArmor.ItemAttribute.Armor + gear.ItemWeapon.ItemAttribute.Armor);
            embed.AddField("Magic Resistance", gear.ItemArmor.ItemAttribute.MagicDamage+ gear.ItemWeapon.ItemAttribute.MagicDamage);


            await ReplyAsync("", false, embed.Build());

        }

        [Command("SelectClass")]
        [Summary("Select your class")]
        public async Task SelectClass(string userClass)
        {
            var dbClass = await _database.userclass.FirstOrDefaultAsync(x => x.Name.ToLower() == userClass.ToLower());
            if (dbClass == null)
            {
                await ReplyAsync("This class doesn't exist");
                return;
            }

            //TODO
            var gear = await _gearDatabase.GetOrCreateGearAsync(Context.User.Id);
            if (gear.UserClass.Name != UserClasses.Pleb)
            {
                await ReplyAsync("You cannot change your class currently");
                return;
            }

            gear.UserClass = dbClass;
            _database.gear.Update(gear);
            await _database.SaveChangesAsync();
            await ReplyAsync($"You've changed your class to {dbClass.Name}");
        }

        [Command("Weapon")]
        [Summary("Equip your weapon")]
        public async Task SelectWeapon([Remainder] string weapon)
        {
            var userGear = await _gearDatabase.GetOrCreateUserWithGearAsync(Context.User.Id);
            if (userGear.Gear.UserClass.Name == UserClasses.Pleb)
            {
                await ReplyAsync("You need to select your class first! Use $SelectClass for that");
                return;
            }

            var eqItem = System.Linq.Enumerable.FirstOrDefault(userGear.Equipment, x => x.Item.Name.ToLower() == weapon.ToLower());
            if (eqItem == null)
            {
                await ReplyAsync("You don't have that item");
                return;
            }
            if (eqItem.Item.ItemType != ItemTypes.Weapon)
            {
                await ReplyAsync("You cannot equip that item type as armor");
                return;
            }

            userGear.Gear.ItemWeapon = eqItem.Item;
            userGear.Gear.GearScore = (userGear.Gear.ItemArmor != null ? userGear.Gear.ItemArmor.ItemAttribute.Score : 0) + userGear.Gear.ItemWeapon.ItemAttribute.Score;

            _database.UserData.Update(userGear);
            await _database.SaveChangesAsync();
            await ReplyAsync($"You equipped **{eqItem.Item.Name}** as your weapon");
        }

        [Command("Armor")]
        [Summary("Equip your armor")]
        public async Task SelectArmor([Remainder] string armor)
        {
            var userGear = await _gearDatabase.GetOrCreateUserWithGearAsync(Context.User.Id);
            if (userGear.Gear.UserClass.Name == UserClasses.Pleb)
            {
                await ReplyAsync("You need to select your class first! Use $SelectClass for that");
                return;
            }

            var eqItem = System.Linq.Enumerable.FirstOrDefault(userGear.Equipment, x => x.Item.Name.ToLower() == armor.ToLower());
            if (eqItem == null)
            {
                await ReplyAsync("You don't have that item");
                return;
            }
            if (eqItem.Item.ItemType != ItemTypes.Armor)
            {
                await ReplyAsync("You cannot equip that item type as armor");
                return;
            }

            userGear.Gear.ItemArmor = eqItem.Item;
            userGear.Gear.GearScore = (userGear.Gear.ItemWeapon != null ? userGear.Gear.ItemWeapon.ItemAttribute.Score : 0) + userGear.Gear.ItemArmor.ItemAttribute.Score;

            _database.UserData.Update(userGear);
            await _database.SaveChangesAsync();
            await ReplyAsync($"You equipped **{eqItem.Item.Name}** as your armor");
        }

        [Command("ShowClasses")]
        [Summary("Show available classes")]
        public async Task ShowClasses()
        {
            var classes = await _database.userclass.ToListAsync();
            var emb = new EmbedBuilder();
            foreach (var x in classes)
            {
                emb.AddField(x.UserClassId.ToString(), x.Name);
            }

            await ReplyAsync("", false, emb.Build());
        }

        [Command("Equipment")]
        [Summary("Show your equipment")]
        public async Task ShowEquipment()
        {
            var eq = await _gearDatabase.GetEquipmentAsync(Context.User.Id);
            var embed = new EmbedBuilder();
            embed.WithColor(Discord.Color.Gold);
            embed.WithCurrentTimestamp();
            embed.WithThumbnailUrl(Context.User.GetAvatarUrl());
            embed.WithFooter("Hue RPG system");

            if (eq.Count == 0)
            {
                embed.Description = "Nothing here";
            }

            else
            {
                foreach (var x in eq)
                {
                    embed.Description += $"> **{x.Item.Name}** - {x.Item.ItemType}\n";
                }
            }
            await ReplyAsync("", false, embed.Build());
        }
    }
}