using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HueProtocol.models
{
    public class UserData
    {
        [Key]
        public ulong UserId { get; set; }
        public string UserName { get; set;}
        public ulong Coins { get; set; }
        public bool UserDaily { get; set; }
        public Quest CurrentQuest { get; set; }
        public int Level { get; set; }
        public float EggPlant { get; set; }
        public bool EggplantDaily { get; set; }
        public Birthday BirthdayDate { get; set; }
        public Gear Gear { get; set; }
        public ICollection<Equipment> Equipment { get; set; }
    }
}
