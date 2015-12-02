using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WinnerWahlkreiseViewModel
    {
        public int ElectionId { get; set; }

        public IEnumerable<BundeslandWithWahlkreiseViewModel<WahlkreisWithWinnerViewModel>> Bundeslands { get; set; } 
    }
}