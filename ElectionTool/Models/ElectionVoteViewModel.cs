using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ElectionTool.Service;

namespace ElectionTool.Models
{
    public class ElectionVoteViewModel
    {
        public string TokenString { get; set; }

        [Display(Name = "Bundestagswahl")]
        public ElectionViewModel Election { get; set; }

        [Display(Name = "Wahlkreis")]
        public WahlkreisViewModel Wahlkreis { get; set; }

        public int? VotedPartyId { get; set; }

        [Display(Name = "Zweitstimme")]
        public IEnumerable<PartyViewModel> Parties { get; set; }

        public int? VotedPersonId { get; set; }

        [Display(Name = "Erststimme")]
        public IEnumerable<PersonWithPartyViewModel> People { get; set; } 
    }
}