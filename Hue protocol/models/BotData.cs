using System.ComponentModel.DataAnnotations;

namespace HueProtocol.models
{
    public class BotData
    {
        [Key]
        public int Id { get; set; }
        public bool UserDailyClear { get; set; }
        public int ServerCount { get; set; }
    }
}
