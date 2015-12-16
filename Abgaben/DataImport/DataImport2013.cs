using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectionTool.Entity_Framework;

namespace ElectionTool.DataImport
{
    class DataImport2013
    {
        public const int electionId = 2;

        public static void AddElection()
        {
            DataImportGeneral.AddElection(new DateTime(2013, 9, 22), 598);
        }

        public static void AddPopulationData()
        {
            var populationDictionary = new Dictionary<string, int>
            {
                {"Schleswig-Holstein", 2686085},
                {"Mecklenburg-Vorpommern", 1585032},
                {"Hamburg", 1559655},
                {"Niedersachsen", 7354892},
                {"Bremen", 575805},
                {"Brandenburg", 2418267},
                {"Sachsen-Anhalt", 2247673},
                {"Berlin", 3025288},
                {"Nordrhein-Westfalen", 15895182},
                {"Sachsen", 4005278},
                {"Hessen", 5388350},
                {"Thüringen", 2154202},
                {"Rheinland-Pfalz", 3672888},
                {"Bayern", 11353264},
                {"Baden-Württemberg", 9482902},
                {"Saarland", 919402}
            };

            DataImportGeneral.AddPopulationData(electionId, populationDictionary);
        }

        public static void AddParties(string filename)
        {
            const int lineIndex = 2;
            const int lineStartIndex = 19;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            var line = parsedFile.ElementAt(lineIndex);

            using (var context = new ElectionDBEntities())
            {
                for (var i = lineStartIndex; i < line.Count; i++)
                {
                    var name = line[i];

                    if (!string.IsNullOrWhiteSpace(name))
                    {
                        context.Parties.Add(new Party
                        {
                            Name = name.Trim()
                        });
                    }
                }

                context.SaveChanges();
            }
        }

        public static void AddAdditionalParties()
        {
            var additional = new List<string> { "DVU", "ADM", "Freie Union", "Christliche Mitte", "ZENTRUM", DataImportGeneral.Impartially };

            using (var context = new ElectionDBEntities())
            {
                foreach (var entry in additional)
                {
                    context.Parties.Add(new Party
                    {
                        Name = entry
                    });
                }

                context.SaveChanges();
            }
        }

        public static void AddPeople(string filename)
        {
            const int titleIndex = 2;
            const int lastnameIndex = 3;
            const int firstnameIndex = 4;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            const int startIndex = 1;

            using (var context = new ElectionDBEntities())
            {
                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    var person = new Person
                    {
                        Title = string.IsNullOrWhiteSpace(line[titleIndex]) ? null : line[titleIndex].Trim(),
                        Firstname = string.IsNullOrWhiteSpace(line[firstnameIndex]) ? null : line[firstnameIndex].Trim(),
                        Lastname = line[lastnameIndex].Trim()
                    };

                    context.People.Add(person);
                    Console.WriteLine("Added {0} {1} {2}", person.Title, person.Firstname, person.Lastname);
                }

                Console.WriteLine("Save changes...");
                context.SaveChanges();
            }
        }

        public static void AddPartyAffiliations(string filename)
        {
            const int personIdIndex = 1;
            const int partyIndex = 6;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            const int startIndex = 1;

            using (var context = new ElectionDBEntities())
            {
                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    // Get Party
                    var partyName = line[partyIndex].Trim();
                    var filteredPartyName = DataImportGeneral.PartyNameDictionary.ContainsKey(partyName)
                        ? DataImportGeneral.PartyNameDictionary[partyName]
                        : partyName;
                    var party = context.Parties.SingleOrDefault(r => r.Name == filteredPartyName);
                    if (party == null)
                    {
                        // Parteilos if no party was set
                        if (string.IsNullOrWhiteSpace(partyName))
                        {
                            party = context.Parties.Single(p => p.Name == DataImportGeneral.Impartially);
                        }
                        else
                        {
                            // Error: Party was set but not found in the database
                            throw new Exception(string.Format("Party {0} does not exist in the database!", partyName));
                        }
                    }


                    var personId = int.Parse(line[personIdIndex]);

                    context.PartyAffiliations.Add(new PartyAffiliation
                    {
                        Election_Id = electionId,
                        Person_Id = personId,
                        Party_Id = party.Id
                    });

                    Console.WriteLine("Added person {0} to party '{1}' for election {2}", personId, party.Name, electionId);
                }

                context.SaveChanges();
            }
        }

        public static void AddIsElectableCandidate(string filename)
        {
            const int personIdIndex = 1;
            const int wahlkreisIdIndex = 7;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            const int startIndex = 1;

            using (var context = new ElectionDBEntities())
            {
                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    int wahlkreisId;
                    var parsed = int.TryParse(line[wahlkreisIdIndex], out wahlkreisId);
                    if (!parsed)
                    {
                        continue;
                    }

                    var personId = int.Parse(line[personIdIndex]);

                    var entry = new IsElectableCandidate
                    {
                        Election_Id = electionId,
                        Person_Id = personId,
                        Wahlkreis_Id = wahlkreisId
                    };

                    context.IsElectableCandidates.Add(entry);

                    Console.WriteLine("Added person {0} is electable in wahlkreis {1} for election {2}", personId, wahlkreisId, electionId);
                }

                context.SaveChanges();
            }
        }

        public static void AddCandidateList(string filename)
        {
            const int personIdIndex = 1;
            const int bundeslandIndex = 8;
            const int positionIndex = 9;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            const int startIndex = 1;

            using (var context = new ElectionDBEntities())
            {
                var idDictionary = context.Bundeslands.ToDictionary(d => d.Name, d => d.Id);

                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    var bundeslandShort = line[bundeslandIndex];
                    if (string.IsNullOrWhiteSpace(bundeslandShort))
                    {
                        continue;
                    }

                    var bundeslandName = DataImportGeneral.BundeslaenderShortDictionary[bundeslandShort];
                    var bundeslandId = idDictionary[bundeslandName];

                    var personId = int.Parse(line[personIdIndex]);
                    var position = int.Parse(line[positionIndex]);

                    var entry = new CandidateList
                    {
                        Election_Id = electionId,
                        Person_Id = personId,
                        Bundesland_Id = bundeslandId,
                        Position = position
                    };

                    context.CandidateLists.Add(entry);
                }

                context.SaveChanges();
            }
        }
    }
}
