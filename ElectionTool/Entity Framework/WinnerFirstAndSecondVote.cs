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
    
    public partial class WinnerFirstAndSecondVote
    {
        public int Election_Id { get; set; }
        public int Bundesland_Id { get; set; }
        public string Bundesland_Name { get; set; }
        public int Wahlkreis_Id { get; set; }
        public string Wahlkreis_Name { get; set; }
        public Nullable<int> Person_Id { get; set; }
        public string Title { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int FirstVote_Party_Id { get; set; }
        public string FirstVote_Party_Name { get; set; }
        public Nullable<int> SecondVote_Party_Id { get; set; }
        public string SecondVote_Party_Name { get; set; }
    }
}