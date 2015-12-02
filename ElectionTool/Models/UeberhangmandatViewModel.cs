using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class UeberhangmandatViewModel
    {
        public int ElectionId { get; set; }

        public IEnumerable<UeberhangmandatEntryViewModel> Mandate { get; set; } 
    }
}