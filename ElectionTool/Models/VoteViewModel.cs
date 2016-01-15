using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class VoteViewModel : IComparable<VoteViewModel>
    {
        [Display(Name = "Anzahl")]
        [DisplayFormat(DataFormatString = "{0:0,0}")]
        public int Amount { get; set; }

        public decimal Votes { get; set; }

        [Display(Name = "Prozent")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal VotesPercent { get { return 100*Votes; } }

        public decimal? LastVotes { get; set; }

        [Display(Name = "Zur letzen Wahl")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal? LastVotesPercent { get { return 100*LastVotes; } }

        public int CompareTo(VoteViewModel other)
        {
            return other.Amount - Amount;
        }
    }
}