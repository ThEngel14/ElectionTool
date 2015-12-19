using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class AllSeatsBundestagViewModel
    {
        public int ElectionId { get; set; }

        public IEnumerable<SeatsBundestagViewModel> SeatsDistribution { get; set; }

        public string DiagrammInfo
        {
            get { return string.Join("$", SeatsDistribution.Select(s => s.DiagrammInfo)); }
        }
    }
}