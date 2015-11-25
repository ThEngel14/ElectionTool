using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class BundeslandListViewModel : IComparable<BundeslandListViewModel>
    {
        public int BundeslandId { get; set; }

        public string BundeslandName { get; set; }

        public IEnumerable<WahlkreisListViewModel> Wahlkreise { get; set; }

        public int CompareTo(BundeslandListViewModel other)
        {
            return BundeslandId - other.BundeslandId;
        }
    }
}