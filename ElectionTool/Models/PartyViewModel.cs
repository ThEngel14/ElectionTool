using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class PartyViewModel : IComparable<PartyViewModel>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CompareTo(PartyViewModel other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}