using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class UeberhangmandatEntryViewModel : IComparable<UeberhangmandatEntryViewModel>
    {
        public int ElectionId { get; set; }

        public BundeslandViewModel Bundesland { get; set; }

        public PartyViewModel Party { get; set; }

        [Display(Name = "Anzahl")]
        public int Amount { get; set; }

        public int CompareTo(UeberhangmandatEntryViewModel other)
        {
            var blandcomp = Bundesland.CompareTo(other.Bundesland);
            return blandcomp != 0 ? blandcomp : Party.CompareTo(other.Party);
        }
    }
}