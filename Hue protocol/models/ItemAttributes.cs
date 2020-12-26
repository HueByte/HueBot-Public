using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class ItemAttribute
    {
        [Key]
        public int ItemAttributesId { get; set; }
        public int Score { get; set; }
        public int Damage { get; set; }
        public int LifeSteal { get; set; }
        public int AttackSpeed { get; set; }
        public int MagicDamage { get; set; }
        public int Armor { get; set; }
        public int MagicResistance { get; set; }
        public int Health { get; set; }
    }
}