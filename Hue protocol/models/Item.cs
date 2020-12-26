using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }
        public int UserClassId { get; set; }
        public int ItemAttributeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ItemType { get; set; }
        public int Price { get; set; }
        public virtual UserClass UserClass { get; set; }
        public virtual ItemAttribute ItemAttribute { get; set; }
    }
}