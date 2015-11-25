using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WahlkreisSelectionViewModel
    {
        public int ElectionId { get; set; }
        public IEnumerable<BundeslandListViewModel> Bundeslands { get; set; } 
    }
}