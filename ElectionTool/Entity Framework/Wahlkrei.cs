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
    
    public partial class Wahlkrei
    {
        public Wahlkrei()
        {
            this.AllowedToElects = new HashSet<AllowedToElect>();
            this.Erststimmes = new HashSet<Erststimme>();
            this.IsElectableCandidates = new HashSet<IsElectableCandidate>();
            this.Zweitstimmes = new HashSet<Zweitstimme>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public int Bundesland_Id { get; set; }
    
        public virtual ICollection<AllowedToElect> AllowedToElects { get; set; }
        public virtual Bundesland Bundesland { get; set; }
        public virtual ICollection<Erststimme> Erststimmes { get; set; }
        public virtual ICollection<IsElectableCandidate> IsElectableCandidates { get; set; }
        public virtual ICollection<Zweitstimme> Zweitstimmes { get; set; }
    }
}
