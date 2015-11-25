using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class WinnerFirstVotesModel : IComparable<WinnerFirstVotesModel>
    {
        public int ElectionId { get; set; }

        public string Wahlkreis { get; set; }

        public string Party { get; set; }

        public string Person { get; set; }
        public int CompareTo(WinnerFirstVotesModel other)
        {
            return Wahlkreis.CompareTo(other.Wahlkreis);
        }
    }
}