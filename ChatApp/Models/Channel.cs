using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class Channel
    {
        public int ChannelId { get; set; }
        public string Name { get; set; }        
        [JsonIgnore]
        public List<Message> Messages { get; set; }
    }
}
