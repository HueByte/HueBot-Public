using HueProtocol.Database;
using HueProtocol.Entities;
using HueProtocol.models;
using HueProtocol;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HueProtocol.Database
{
    public class SeedDatabase : IInjectableService
    {
        private readonly EFContext _database;
        public SeedDatabase(EFContext database)
        {
            _database = database;

            SeedClasses().GetAwaiter();
        }

        public async Task SeedClasses()
        {
            var exist = _database.userclass.Any(userClass => userClass.Name == UserClasses.Assasing);
            if (exist)
                return;

            var assasin = new UserClass()
            {
                Name = UserClasses.Assasing
            };
            var archer = new UserClass()
            {
                Name = UserClasses.Archer
            };
            var warrior = new UserClass()
            {
                Name = UserClasses.Archer
            };
            var mage = new UserClass()
            {
                Name = UserClasses.Mage
            };
            var pleb = new UserClass()
            {
                Name = UserClasses.Pleb
            };

            await _database.userclass.AddRangeAsync(assasin, archer, warrior, mage, pleb);
            await _database.SaveChangesAsync();
        }

    }
}