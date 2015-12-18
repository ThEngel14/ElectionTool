using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class BundeslandViewModel : IComparable<BundeslandViewModel>
    {
        public int Id { get; set; }

        [Display(Name = "Bundesland")]
        public string Name { get; set; }
        public int CompareTo(BundeslandViewModel other)
        {
            return Id - other.Id;
        }
    }
}