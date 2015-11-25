using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class VoteViewModel : IComparable<VoteViewModel>
    {
        public int Amount { get; set; }

        public decimal Votes { get; set; }

        public decimal VotesPercent { get { return 100*Math.Round(Votes, 3); } }

        public decimal? LastVotes { get; set; }

        public decimal? LastVotesPercent { get { return LastVotes.HasValue ? 100*Math.Round((decimal) LastVotes, 3) : LastVotes; } }
        public int CompareTo(VoteViewModel other)
        {
            return other.Amount - Amount;
        }
    }
}