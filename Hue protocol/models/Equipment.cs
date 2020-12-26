using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HueProtocol.models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }
        public int ItemId { get; set; }
        public ulong OwnerId { get; set; }
        public virtual Item Item { get; set; }
        public virtual UserData Owner { get; set; }
    }
}