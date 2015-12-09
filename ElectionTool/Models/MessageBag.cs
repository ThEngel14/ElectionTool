using System;
using System.Collections.Generic;
using System.Linq;

namespace ElectionTool.Models
{
    public class MessageBag
    {
        public MessageBag()
        {
            Danger = new List<string>();
            Info = new List<string>();
            Success = new List<string>();
            Warning = new List<string>();
        }

        public List<string> Danger { get; set; }

        public List<string> Info { get; set; }

        public List<string> Success { get; set; }

        public List<string> Warning { get; set; }

        public MessageBag Combine(MessageBag other)
        {
            if (other == null)
                return new MessageBag
                {
                    Danger = Danger.ToList(),
                    Info = Info.ToList(),
                    Success = Success.ToList(),
                    Warning = Warning.ToList()
                };

            return new MessageBag
            {
                Danger = Danger.Concat(other.Danger).ToList(),
                Info = Info.Concat(other.Info).ToList(),
                Success = Success.Concat(other.Success).ToList(),
                Warning = Warning.Concat(other.Warning).ToList()
            };
        }

        public void Add(MessageBag other)
        {
            if (other == null)
                throw new ArgumentException("other");

            Danger.AddRange(other.Danger);
            Info.AddRange(other.Info);
            Success.AddRange(other.Success);
            Warning.AddRange(other.Warning);
        }
    }
}