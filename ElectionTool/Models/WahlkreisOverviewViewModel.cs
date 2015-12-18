using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WahlkreisOverviewViewModel
    {
        public int ElectionId { get; set; }

        public WahlkreisViewModel Wahlkreis { get; set; }

        public PersonWithPartyViewModel Candidate { get; set; }

        [Display(Name = "Wahlbeteiligung")]
        public decimal Participation { get; set; }

        [Display(Name = "Wahlbeteiligung")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal ParticipationPercent { get { return 100*Participation; } }

        public IEnumerable<WahlkreisFirstVotesViewModel> FirstVotes { get; set; }

        public IEnumerable<WahlkreisSecondVotesViewModel> SecondVotes { get; set; } 
    }
}