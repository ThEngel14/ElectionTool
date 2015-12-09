using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionTool.Service;

namespace ElectionTool.Models
{
    public class ElectionVoteViewModel
    {
        public string TokenString { get; set; }

        public ElectionViewModel Election { get; set; }

        public WahlkreisViewModel Wahlkreis { get; set; }

        public int? VotedPartyId { get; set; }

        public IEnumerable<PartyViewModel> Parties { get; set; }

        public int? VotedPersonId { get; set; }

        public IEnumerable<PersonWithPartyViewModel> People { get; set; } 
    }
}