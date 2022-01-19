using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Models
{
    public class ChatAppContext : DbContext
    {

        public ChatAppContext(DbContextOptions<ChatAppContext> options)
          : base(options)
        {

        }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
