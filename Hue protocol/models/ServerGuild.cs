using System.ComponentModel.DataAnnotations;
using HueProtocol.models;
using System.Collections.Generic;

namespace HueProtocol.models
{
    public class ServerGuild
    {
        [Key]
        public ulong ServerId { get; set; }
        public string ServerName { get; set; }
        public ulong OwnerId { get; set; }
        public string OwnerName { get; set; }
        public char prefix { get; set; }
        public ulong GreetingRoom { get; set; }
        public string GreetingMsg { get; set; }
        public bool DoGreet { get; set; }
        public ulong BirthdayRoom { get; set; }
        public bool DoBrithday { get; set; }
        public ulong RolePostId { get; set; }
    }
}
