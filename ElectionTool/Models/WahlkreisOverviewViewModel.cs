using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WahlkreisOverviewViewModel
    {
        public int ElectionId { get; set; }

        public int WahlkreisId { get; set; }

        public string WahlkreisName { get; set; }

        public PersonWithPartyViewModel Candidate { get; set; }

        public decimal Participation { get; set; }

        public decimal ParticipationPercent { get { return 100*Math.Round(Participation, 3); } }

        public IEnumerable<WahlkreisFirstVotesViewModel> FirstVotes { get; set; }

        public IEnumerable<WahlkreisSecondVotesViewModel> SecondVotes { get; set; } 
    }
}