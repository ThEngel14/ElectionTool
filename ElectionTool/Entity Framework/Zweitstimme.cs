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
    
    public partial class Zweitstimme
    {
        public int Id { get; set; }
        public int Election_Id { get; set; }
        public Nullable<int> Party_Id { get; set; }
        public int Wahlkreis_Id { get; set; }
    
        public virtual Election Election { get; set; }
        public virtual Party Party { get; set; }
        public virtual Wahlkrei Wahlkrei { get; set; }
    }
}
