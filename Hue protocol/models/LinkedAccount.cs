using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HueProtocol.models
{
    public class LinkedAccount
    {
        [Key]
        public int Id { get; set; }
        public UserData User { get; set; }
        public string LinkedServiceLink { get; set; }
        public string Type { get; set; }
    }
}
