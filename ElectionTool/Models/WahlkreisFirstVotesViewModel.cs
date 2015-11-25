using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc.Routing;

namespace ElectionTool.Models
{
    public class WahlkreisFirstVotesViewModel : IComparable<WahlkreisFirstVotesViewModel>
    {
        public int ElectionId { get; set; }

        public int WahlkreisId { get; set; }

        public PersonWithPartyViewModel Candidate { get; set; }

        public VoteViewModel Vote { get; set; }
        public int CompareTo(WahlkreisFirstVotesViewModel other)
        {
            return Vote.CompareTo(other.Vote);
        }
    }
}