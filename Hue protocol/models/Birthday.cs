using System;
using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class Birthday
    {
        [Key]
        public int BirthId { get; set; }
        public ulong UserId { get; set; }
        public DateTime Date { get; set; }
        public bool DoDisplay { get; set; }
        public virtual UserData User { get; set; }
    }
}