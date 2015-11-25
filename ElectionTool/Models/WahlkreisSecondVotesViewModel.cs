using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WahlkreisSecondVotesViewModel : IComparable<WahlkreisSecondVotesViewModel>
    {
        public int ElectionId { get; set; }

        public int WahlkreisId { get; set; }

        public PartyViewModel Party { get; set; }

        public VoteViewModel Vote { get; set; }

        public int CompareTo(WahlkreisSecondVotesViewModel other)
        {
            return Vote.CompareTo(other.Vote);
        }
    }
}