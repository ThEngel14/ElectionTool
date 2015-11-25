using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WahlkreisListViewModel : IComparable<WahlkreisListViewModel>
    {
        public int WahlkreisId { get; set; }

        public string WahlkreisName { get; set; }

        public int CompareTo(WahlkreisListViewModel other)
        {
            return WahlkreisId - other.WahlkreisId;
        }
    }
}