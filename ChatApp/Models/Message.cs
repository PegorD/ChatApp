using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class Message
    {
        public int ChannelId { get; set; }
        public int MessageId { get; set; }        
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }       
        [JsonIgnore]
        public Channel Channel { get; set; }
    }
}
