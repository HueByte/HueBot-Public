using System;
using System.Threading.Tasks;
using HueProtocol.Database;
using HueProtocol.Entities;
using HueProtocol.models;
using HueProtocol.Services;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HueProtocol.Tools
{
    public class GearDatabase : IInjectableService
    {
        private readonly EFContext _database;
        private readonly MiscDatabaseMethodsService _misc;
        public GearDatabase(EFContext database, MiscDatabaseMethodsService misc)
        {
            _database = database;
            _misc = misc;
        }

        ///<summary>
        ///Gets or creates user with his gear and equipment asynchronously
        ///</summary>
        ///<param name="UserId">discord user's ID</param>
        ///<returns>
        ///<see cref="UserData"/>
        ///</returns>
        public async Task<UserData> GetOrCreateUserWithGearAsync(ulong UserId)
        {
            //Multiple level Eager loading
            UserData user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                .FirstOrDefaultAsync<UserData>(_database.UserData
                    .Include(entity => entity.Gear)
                        .ThenInclude(entity => entity.UserClass)
                    .Include(entity => entity.Gear)
                        .ThenInclude(entity => entity.ItemArmor)
                            .ThenInclude(entity => entity.UserClass)
                    .Include(entity => entity.Gear)
                        .ThenInclude(entity => entity.ItemWeapon)
                            .ThenInclude(entity => entity.UserClass)
                    .Include(entity => entity.Equipment)
                        .ThenInclude(entity => entity.Item)
                            .ThenInclude(entity => entity.ItemAttribute)
                , usr => usr.UserId == UserId);

            //If User doesn't exist
            if (user == null)
            {
                user = await _misc.GetOrCreateUser(UserId);
            }

            //check if gear exists
            if (user.Gear == null)
            {
                UserClass userClass = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                        .FirstOrDefaultAsync(_database.userclass, x => x.Name == UserClasses.Pleb);

                var gear = new Gear()
                {
                    ItemWeapon = null,
                    ItemArmor = null,
                    User = user,
                    UserClass = userClass
                };

                user.Gear = gear;
                _database.UserData.Update(user);
                await _database.SaveChangesAsync();
            }

            return user;
        }
        
        ///<summary>
        ///Gets or creates user's his gear asynchronously
        ///</summary>
        ///<param name="UserId">discord user's ID</param>
        ///<returns>
        ///<see cref="Gear"/>
        ///</returns>
        public async Task<Gear> GetOrCreateGearAsync(ulong UserId)
        {
            Gear gear;

            //Multiple level Eager loading
            UserData user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                .FirstOrDefaultAsync<UserData>(_database.UserData
                    .Include(entity => entity.Gear)
                        .ThenInclude(entity => entity.UserClass)
                    .Include(entity => entity.Gear)
                        .ThenInclude(entity => entity.ItemArmor)
                            .ThenInclude(entity => entity.UserClass)
                    .Include(entity => entity.Gear)
                        .ThenInclude(entity => entity.ItemWeapon)
                            .ThenInclude(entity => entity.UserClass)
                , usr => usr.UserId == UserId);

            //If User doesn't exist
            if (user == null)
            {
                user = await _misc.GetOrCreateUser(UserId);
            }

            if (user.Gear == null)
            {
                UserClass userClass = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                        .FirstOrDefaultAsync(_database.userclass, x => x.Name == UserClasses.Pleb);

                gear = new Gear()
                {
                    ItemWeapon = null,
                    ItemArmor = null,
                    User = user,
                    UserClass = userClass
                };

                user.Gear = gear;
                _database.UserData.Update(user);
                await _database.SaveChangesAsync();
            }

            return user.Gear;
        }

        ///<summary>
        ///Gets or creates user with his equipment
        ///</summary>
        ///<param name="UserId">discord user's ID</param>
        ///<returns>
        ///<see cref="UserData"/>
        ///</returns>
        public async Task<UserData> GetUserWithEquipmentAsync(ulong userId)
        {
            var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                .FirstOrDefaultAsync(
                    _database.UserData
                        .Include(entity => entity.Equipment)
                            .ThenInclude(entity => entity.Item)
                                .ThenInclude(entity => entity.ItemAttribute)
                , usr => usr.UserId == userId);
            if(user == null)
            {
                user = await _misc.GetOrCreateUser(userId);
            }
            return user;
        }
        
        ///<summary>
        ///Gets user's equipment
        ///</summary>
        ///<param name="userId">discord user's ID</param>
        ///<returns>
        ///<see cref="IList{T}"/> of <see cref="Equipment"/>
        ///</returns>
        public async Task<IList<Equipment>> GetEquipmentAsync(ulong userId)
        {
            var eq = await System.Linq.Queryable.Where(_database.Equipments.Include(entity => entity.Item).ThenInclude(entity => entity.ItemAttribute), x => x.OwnerId == userId).ToListAsync();
            return eq;
        }
    }
}