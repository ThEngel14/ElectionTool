using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class ClosestWinnerForPartyViewModel
    {
        public int ElectionId { get; set; }

        public PartyViewModel Party { get; set; }

        public IEnumerable<ClosestWinnerEntryViewModel> ClosestWinner { get; set; } 
    }
}