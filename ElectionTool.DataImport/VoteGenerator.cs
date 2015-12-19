using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectionTool.Entity_Framework;

namespace ElectionTool.DataImport
{
    class VoteGenerator
    {
        private static readonly string GeneratedPeopleLastname = "Generated";

        // Quite similar to GenerateVotesFor2009. High number of clones!
        public static void GenerateVotesFor2013(string filename)
        {
            const int electionId = 2;

            const int partyNameLineIndex = 2;
            const int startLineIndex = 5;

            const int wahlkreisIdIndex = 0;
            const int bundeslandIdIndex = 2;
            const int allowedVoterIndex = 3;
            const int voterIndex = 7;
            const int invalidErststimmeIndex = 11;
            const int invalidZweitstimmeIndex = 13;
            const int startPartyColumnIndex = 19;

            const int skippedBundeslandId = 99;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            using (var context = new ElectionDBEntities())
            {
                var election = context.Elections.Single(e => e.Id == electionId);
                Console.WriteLine("Generate votes for election {0}", electionId);
                Console.WriteLine();

                var partyDictionary = context.Parties.ToDictionary(p => p.Name, p => p.Id);

                for (var i = startLineIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    int wahlkreisId;
                    var parsed = int.TryParse(line[wahlkreisIdIndex], out wahlkreisId);
                    if (!parsed || string.IsNullOrWhiteSpace(line[bundeslandIdIndex]) || int.Parse(line[bundeslandIdIndex]) == skippedBundeslandId)
                    {
                        // Increse i because net line is empty
                        i++;
                        continue;
                    }

                    Console.WriteLine("Process wahlkreis {0}", wahlkreisId);

                    var amountAllowedVoters = int.Parse(line[allowedVoterIndex]);
                    var amountVoters = int.Parse(line[voterIndex]);
                    var invalidErstimmen = int.Parse(line[invalidErststimmeIndex]);
                    var invalidZweitstimmen = int.Parse(line[invalidZweitstimmeIndex]);

                    GeneratePeopleForWahlkreis(context, election, wahlkreisId, amountAllowedVoters, amountVoters);

                    GenerateErststimmenVotes(context, electionId, wahlkreisId, invalidErstimmen, null);
                    GenerateZweitstimmenVotes(context, electionId, wahlkreisId, invalidZweitstimmen, null);

                    // Generate votes for parties
                    for (var k = startPartyColumnIndex; k < line.Count-1; k += 4)
                    {
                        var partyName = parsedFile.ElementAt(partyNameLineIndex)[k].Trim();
                        var filteredPartyName = DataImportGeneral.PartyNameDictionary.ContainsKey(partyName)
                            ? DataImportGeneral.PartyNameDictionary[partyName]
                            : partyName;

                        var partyId = partyDictionary[filteredPartyName];

                        // Erststimmen
                        int partyErststimmen;
                        var parsedErst = int.TryParse(line[k], out partyErststimmen);
                        if (!parsedErst)
                        {
                            partyErststimmen = 0;
                        }

                        // Calculate only if needed afterwards
                        var person = partyErststimmen == 0 ? null :
                            context.People.FirstOrDefault(
                                p =>
                                    p.IsElectableCandidates.Any(
                                        c => c.Election_Id == electionId && c.Wahlkreis_Id == wahlkreisId) &&
                                    p.PartyAffiliations.Any(a => a.Election_Id == electionId && a.Party_Id == partyId));

                        // Hack because some candidates are missing
                        if (person == null && partyErststimmen > 0)
                        {
                            person = new Person
                            {
                                Lastname = GeneratedPeopleLastname
                            };
                            context.People.Add(person);

                            context.PartyAffiliations.Add(new PartyAffiliation
                            {
                                Election_Id = electionId,
                                Party_Id = partyId,
                                Person = person
                            });

                            context.IsElectableCandidates.Add(new IsElectableCandidate
                            {
                                Election_Id = electionId,
                                Wahlkreis_Id = wahlkreisId,
                                Person = person
                            });
                        }

                        GenerateErststimmenVotes(context, electionId, wahlkreisId, partyErststimmen, person);

                        // Zweitstimmen
                        int partyZweitstimmen;
                        var parsedZweit = int.TryParse(line[k + 2], out partyZweitstimmen);
                        if (!parsedZweit)
                        {
                            partyZweitstimmen = 0;
                        }

                        GenerateZweitstimmenVotes(context, electionId, wahlkreisId, partyZweitstimmen, partyId);
                    }

                    Console.WriteLine("");
                }

                context.SaveChanges();
            }
        }

        // Quite similar to GenerateVotesFor2013. High number of clones!
        public static void GenerateVotesFor2009(string filename)
        {
            const int electionId = 1;

            const int partyNameLineIndex = 2;
            const int startLineIndex = 4;

            const int wahlkreisIdIndex = 0;
            const int allowedVoterIndex = 3;
            const int voterIndex = 4;
            const int invalidErststimmeIndex = 5;
            const int invalidZweitstimmeIndex = 6;
            const int startPartyColumnIndex = 9;

            const int skippedBundeslandId = 900;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            using (var context = new ElectionDBEntities())
            {
                var election = context.Elections.Single(e => e.Id == electionId);
                Console.WriteLine("Generate votes for election {0}", electionId);
                Console.WriteLine();

                var partyDictionary = context.Parties.ToDictionary(p => p.Name, p => p.Id);

                for (var i = startLineIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    int wahlkreisId;
                    var parsed = int.TryParse(line[wahlkreisIdIndex], out wahlkreisId);
                    if (!parsed || wahlkreisId >= skippedBundeslandId)
                    {
                        // Increse i because net line is empty
                        i++;
                        continue;
                    }

                    Console.WriteLine("Process wahlkreis {0}", wahlkreisId);

                    var amountAllowedVoters = int.Parse(line[allowedVoterIndex].Replace(" ", ""));
                    var amountVoters = int.Parse(line[voterIndex].Replace(" ", ""));
                    var invalidErstimmen = int.Parse(line[invalidErststimmeIndex].Replace(" ", ""));
                    var invalidZweitstimmen = int.Parse(line[invalidZweitstimmeIndex].Replace(" ", ""));

                    GeneratePeopleForWahlkreis(context, election, wahlkreisId, amountAllowedVoters, amountVoters);

                    GenerateErststimmenVotes(context, electionId, wahlkreisId, invalidErstimmen, null);
                    GenerateZweitstimmenVotes(context, electionId, wahlkreisId, invalidZweitstimmen, null);

                    // Generate votes for parties
                    for (var k = startPartyColumnIndex; k < line.Count; k++)
                    {
                        var partyName = parsedFile.ElementAt(partyNameLineIndex)[k].Trim();
                        var filteredPartyName = DataImportGeneral.PartyNameDictionary.ContainsKey(partyName)
                            ? DataImportGeneral.PartyNameDictionary[partyName]
                            : partyName;

                        var partyId = partyDictionary[filteredPartyName];

                        var erststimme = parsedFile.ElementAt(partyNameLineIndex + 1)[k].Trim().Equals("Erststimmen");

                        if (erststimme)
                        {
                            // Erststimmen
                            int partyErststimmen;
                            var parsedErst = int.TryParse(line[k].Replace(" ", ""), out partyErststimmen);
                            if (!parsedErst)
                            {
                                partyErststimmen = 0;
                            }

                            // Calculate only if needed afterwards
                            var person = partyErststimmen == 0
                                ? null
                                : context.People.FirstOrDefault(
                                    p =>
                                        p.IsElectableCandidates.Any(
                                            c => c.Election_Id == electionId && c.Wahlkreis_Id == wahlkreisId) &&
                                        p.PartyAffiliations.Any(
                                            a => a.Election_Id == electionId && a.Party_Id == partyId));

                            // Hack because some candidates are missing
                            if (person == null && partyErststimmen > 0)
                            {
                                person = new Person
                                {
                                    Lastname = GeneratedPeopleLastname
                                };
                                context.People.Add(person);

                                context.PartyAffiliations.Add(new PartyAffiliation
                                {
                                    Election_Id = electionId,
                                    Party_Id = partyId,
                                    Person = person
                                });

                                context.IsElectableCandidates.Add(new IsElectableCandidate
                                {
                                    Election_Id = electionId,
                                    Wahlkreis_Id = wahlkreisId,
                                    Person = person
                                });
                            }

                            GenerateErststimmenVotes(context, electionId, wahlkreisId, partyErststimmen, person);
                        }
                        else
                        {
                            // Zweitstimmen
                            int partyZweitstimmen;
                            var parsedZweit = int.TryParse(line[k].Replace(" ", ""), out partyZweitstimmen);
                            if (!parsedZweit)
                            {
                                partyZweitstimmen = 0;
                            }

                            GenerateZweitstimmenVotes(context, electionId, wahlkreisId, partyZweitstimmen, partyId);
                        }
                    }

                    Console.WriteLine("");
                }

                context.SaveChanges();
            }
        }

        private static void GeneratePeopleForWahlkreis(ElectionDBEntities context, Election election, int wahlkreisId, int amountAllowedVoters, int amountVoters)
        {
            if (amountAllowedVoters < amountVoters)
            {
                throw new Exception(
                    string.Format(
                        "Amount of allowed voters cannot be lower than the amount of all actual voters: {0} < {1}",
                        amountAllowedVoters, amountVoters));
            }

            context.AllowedToElectAmounts.Add(new AllowedToElectAmount
            {
                Election_Id = election.Id,
                Wahlkreis_Id = wahlkreisId,
                Amount = amountAllowedVoters
            });

            context.ParticipationAmounts.Add(new ParticipationAmount
            {
                Election_Id = election.Id,
                Wahlkreis_Id = wahlkreisId,
                Amount = amountVoters
            });

            Console.WriteLine("Added {0} allowed voters", amountAllowedVoters);
            Console.WriteLine("Added {0} actual voters", amountVoters);
        }

        private static void GenerateErststimmenVotes(ElectionDBEntities context, int electionId, int wahlkreisId,
            int amount, Person person)
        {
            if (amount <= 0)
            {
                return;
            }

            context.ErststimmeAmounts.Add(new ErststimmeAmount
            {
                Election_Id = electionId,
                Person = person,
                Wahlkreis_Id = wahlkreisId,
                Amount = amount
            });

            Console.WriteLine("Created {0} erststimmen for candidate {1}", amount, person == null ? -1 : person.Id);
        }

        private static void GenerateZweitstimmenVotes(ElectionDBEntities context, int electionId, int wahlkreisId,
            int amount, int? partyId)
        {
            if (amount <= 0)
            {
                return;
            }

            context.ZweitstimmeAmounts.Add(new ZweitstimmeAmount
            {
                Election_Id = electionId,
                Wahlkreis_Id = wahlkreisId,
                Party_Id = partyId,
                Amount = amount
            });

            Console.WriteLine("Created {0} zweitstimmen for party {1}", amount, partyId);
        }
    }
}
