using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class PartyViewModel : IComparable<PartyViewModel>
    {
        public int Id { get; set; }

        [Display(Name = "Partei")]
        public string Name { get; set; }

        public int CompareTo(PartyViewModel other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}