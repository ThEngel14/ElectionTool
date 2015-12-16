using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WahlkreisViewModel : IComparable<WahlkreisViewModel>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int BundeslandId { get; set; }

        public int CompareTo(WahlkreisViewModel other)
        {
            return Id - other.Id;
        }
    }
}