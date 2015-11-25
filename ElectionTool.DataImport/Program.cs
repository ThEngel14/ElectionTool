using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectionTool.Entity_Framework;

namespace ElectionTool.DataImport
{
    class Program
    {
        private const string kerg2013 = @"C:\Users\Thomas\Documents\TUM\Master\Datenbanken\Bundestagswahl Daten\kerg2013.csv";

        private const string wahlbewerber2013 =
            @"C:\Users\Thomas\Documents\TUM\Master\Datenbanken\Bundestagswahl Daten\Wahlbewerber2013\wahlbewerber_mit_platz.csv";

        private const string wahlbewerber2009 = @"C:\Users\Thomas\Documents\TUM\Master\Datenbanken\Bundestagswahl Daten\wahlbewerber2009_mod.csv";

        private const string wkUmrechnung2013 = @"C:\Users\Thomas\Documents\TUM\Master\Datenbanken\Bundestagswahl Daten\wkumrechnung2013.csv";

        static void Main(string[] args)
        {
            //DataImportGeneral.AddBundeslaender();
            //DataImportGeneral.AddWahlkreise(kerg2013);

            //DataImport2013.AddElection();
            //DataImport2013.AddParties(kerg2013);
            //DataImport2013.AddAdditionalParties();
            //DataImport2013.AddPeople(wahlbewerber2013);
            //DataImport2013.AddPartyAffiliations(wahlbewerber2013);
            //DataImport2013.AddIsElectableCandidate(wahlbewerber2013);
            //DataImport2013.AddCandidateList(wahlbewerber2013);
            //DataImport2013.AddPopulationData();

            //DataImport2009.AddElection();
            //DataImport2009.AddPeople(wahlbewerber2009);
            //DataImport2009.AddPartyAffiliations(wahlbewerber2009);
            //DataImport2009.AddIsElectableCandidate(wahlbewerber2009);
            //DataImport2009.AddCandidateList(wahlbewerber2009);
            //DataImport2009.AddPopulationData();

            //VoteGenerator.GenerateVotesFor2009(wkUmrechnung2013);
            //VoteGenerator.GenerateVotesFor2013(kerg2013);
        }
    }
}
