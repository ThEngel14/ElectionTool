using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class SeatsBundestagViewModel : IComparable<SeatsBundestagViewModel>
    {
        public int ElectionId { get; set; }

        public PartyColorViewModel Party { get; set; }

        public VoteViewModel Seats { get; set; }

        public string DiagrammInfo
        {
            get
            {
                return string.Format("{0}-{1}-{2}-{3}", Party.Name, Seats.Amount, Party.Color, Party.Highlight);
            }
        }

        public int CompareTo(SeatsBundestagViewModel other)
        {
            var seatscomp = Seats.CompareTo(other.Seats);
            return seatscomp != 0 ? seatscomp : Party.CompareTo(other.Party);
        }
    }
}