using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class Quest
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public uint xp { get; set; }
        public uint coins { get; set; }
    }
}
