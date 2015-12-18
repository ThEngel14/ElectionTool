using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class MemberOfBundestagViewModel : IComparable<MemberOfBundestagViewModel>
    {
        public int ElectionId { get; set; }

        public PersonWithPartyViewModel Member { get; set; }

        public BundeslandViewModel Bundesland { get; set; }

        public WahlkreisViewModel Wahlkreis { get; set; }

        public int CompareTo(MemberOfBundestagViewModel other)
        {
            return Member.CompareTo(other.Member);
        }
    }
}