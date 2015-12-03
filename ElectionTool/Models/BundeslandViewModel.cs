using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class BundeslandViewModel : IComparable<BundeslandViewModel>
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int CompareTo(BundeslandViewModel other)
        {
            return other.Id - Id;
        }
    }
}