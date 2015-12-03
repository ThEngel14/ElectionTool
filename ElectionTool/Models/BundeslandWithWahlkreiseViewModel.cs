using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class BundeslandWithWahlkreiseViewModel<T> : IComparable<BundeslandWithWahlkreiseViewModel<T>> where T : WahlkreisViewModel
    {
        public BundeslandViewModel Bundesland { get; set; }

        public IEnumerable<T> Wahlkreise { get; set; }

        public int CompareTo(BundeslandWithWahlkreiseViewModel<T> other)
        {
            return Bundesland.CompareTo(other.Bundesland);
        }
    }
}