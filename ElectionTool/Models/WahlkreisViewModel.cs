using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WahlkreisViewModel : IComparable<WahlkreisViewModel>
    {
        public int Id { get; set; }

        [Display(Name = "Wahlkreis")]
        public string Name { get; set; }

        [Display(Name = "Bundesland")]
        public int BundeslandId { get; set; }

        public int CompareTo(WahlkreisViewModel other)
        {
            return Id - other.Id;
        }
    }
}