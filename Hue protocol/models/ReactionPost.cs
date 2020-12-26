using System;
using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class ReactionPost
    {
        [Key]
        public ulong Id { get; set; }
        public ulong ChannelId { get; set; }
        public ulong ServerId{ get; set; }
        public int UserMax { get; set; }
        public string Details { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }
}
