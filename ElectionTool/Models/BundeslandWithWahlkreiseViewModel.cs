using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class BundeslandWithWahlkreiseViewModel<T> : IComparable<BundeslandWithWahlkreiseViewModel<T>> where T : WahlkreisViewModel
    {
        [Display(Name = "Bundesland")]
        public BundeslandViewModel Bundesland { get; set; }

        [Display(Name = "Wahlkreis")]
        public IEnumerable<T> Wahlkreise { get; set; }

        public int CompareTo(BundeslandWithWahlkreiseViewModel<T> other)
        {
            return Bundesland.CompareTo(other.Bundesland);
        }
    }
}