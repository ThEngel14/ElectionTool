using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class ClosestWinnerEntryViewModel : IComparable<ClosestWinnerEntryViewModel>
    {
        public PersonViewModel Person { get; set; }

        public WahlkreisViewModel Wahlkreis { get; set; }

        public int Difference { get; set; }

        public bool IsWinner { get { return Difference > 0; } }

        public string KindOfResult { get { return IsWinner ? "Gewinner" : "Verlierer"; } }

        public int AbsDifference { get { return Math.Abs(Difference); } }

        public int CompareTo(ClosestWinnerEntryViewModel other)
        {
            return AbsDifference - other.AbsDifference;
        }
    }
}