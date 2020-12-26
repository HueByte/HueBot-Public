using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class Gear
    {
        [Key]
        public int GearId { get; set; }
        public int ItemWeaponId { get; set; }
        public int ItemArmorId { get; set; }
        public int UserClassId { get; set; }
        public ulong UserId { get; set; }
        public int GearScore { get; set; }
        public virtual UserData User { get; set; }
        public virtual Item ItemWeapon { get; set; }
        public virtual Item ItemArmor { get; set; }
        public virtual UserClass UserClass { get; set; }
    }
}