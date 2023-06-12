using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace MessageLogger.Models
{
    public class Message
    {
        public int Id { get; private set; }
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; } = new();
        public User User { get; private set; }

        public Message(string content)
        {
            Content = content;
            CreatedAt = DateTime.Now;
        }
    }
}
