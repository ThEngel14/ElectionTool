//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ElectionTool.Entity_Framework
{
    using System;
    using System.Collections.Generic;
    
    public partial class ZweitstimmeWahlkreisOverview
    {
        public int Election_Id { get; set; }
        public int Wahlkreis_Id { get; set; }
        public string Wahlkreis_Name { get; set; }
        public Nullable<int> Party_Id { get; set; }
        public string Party_Name { get; set; }
        public int Votes { get; set; }
        public Nullable<int> Overall { get; set; }
        public Nullable<decimal> PercentVotes { get; set; }
        public Nullable<decimal> Previous { get; set; }
    }
}
