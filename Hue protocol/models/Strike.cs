using System;
using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class Strike
    {
        [Key]
        public long StrikeId { get; set; }
        public UserData User { get; set; }
        public UserData Creator { get; set; }
        public ulong GuildId { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }
}
