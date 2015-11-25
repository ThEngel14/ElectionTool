using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class MemberOfBundestagViewModel : IComparable<MemberOfBundestagViewModel>
    {
        public int ElectionId { get; set; }

        public PersonWithPartyViewModel Member { get; set; }

        public string Bundesland { get; set; }

        public string Wahlkreis { get; set; }
        public int CompareTo(MemberOfBundestagViewModel other)
        {
            var membercomp = Member.CompareTo(other.Member);
            if (membercomp != 0)
            {
                return membercomp;
            }

            var bundeslandcomp = Bundesland.CompareTo(other.Bundesland);
            if (bundeslandcomp != 0)
            {
                return bundeslandcomp;
            }

            if (Wahlkreis == null)
            {
                return -1;
            }

            return Wahlkreis.CompareTo(other.Wahlkreis);
        }
    }
}