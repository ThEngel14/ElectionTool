using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class PartySelectionViewModel
    {
        public int ElectionId { get; set; }

        public IEnumerable<PartyViewModel> Parties { get; set; } 
    }
}