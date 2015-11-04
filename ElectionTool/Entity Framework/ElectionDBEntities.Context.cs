﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ElectionDBEntities : DbContext
    {
        public ElectionDBEntities()
            : base("name=ElectionDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AllowedToElect> AllowedToElects { get; set; }
        public virtual DbSet<Bundesland> Bundeslands { get; set; }
        public virtual DbSet<CandidateList> CandidateLists { get; set; }
        public virtual DbSet<Election> Elections { get; set; }
        public virtual DbSet<Erststimme> Erststimmes { get; set; }
        public virtual DbSet<IsElectableCandidate> IsElectableCandidates { get; set; }
        public virtual DbSet<IsElectableParty> IsElectableParties { get; set; }
        public virtual DbSet<Party> Parties { get; set; }
        public virtual DbSet<PartyAffiliation> PartyAffiliations { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<PopulationBundesland> PopulationBundeslands { get; set; }
        public virtual DbSet<Wahlkrei> Wahlkreis { get; set; }
        public virtual DbSet<Zweitstimme> Zweitstimmes { get; set; }
    }
}
