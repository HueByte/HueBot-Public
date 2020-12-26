using System;
using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
