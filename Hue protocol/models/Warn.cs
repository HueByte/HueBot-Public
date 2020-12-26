using System;
using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class Warn
    {
        [Key]
        public long WarnId { get; set; }
        public UserData User { get; set; }
        public UserData Creator { get; set; }
        public ulong GuildId { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
    }
}
