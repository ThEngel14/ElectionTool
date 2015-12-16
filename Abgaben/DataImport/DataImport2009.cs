using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectionTool.Entity_Framework;

namespace ElectionTool.DataImport
{
    class DataImport2009
    {
        public const int electionId = 1;

        public static void AddElection()
        {
            DataImportGeneral.AddElection(new DateTime(2009, 9, 27), 598);
        }

        public static void AddPopulationData()
        {
            var populationDictionary = new Dictionary<string, int>
            {
                {"Schleswig-Holstein", 2834300},
                {"Mecklenburg-Vorpommern", 1664400},
                {"Hamburg", 1772100},
                {"Niedersachsen", 7947200},
                {"Bremen", 661900},
                {"Brandenburg", 2522500},
                {"Sachsen-Anhalt", 2381900},
                {"Berlin", 3431700},
                {"Nordrhein-Westfalen", 17933100},
                {"Sachsen", 4192800},
                {"Hessen", 6065000},
                {"Thüringen", 2267800},
                {"Rheinland-Pfalz", 4028400},
                {"Bayern", 12519700},
                {"Baden-Württemberg", 10749500},
                {"Saarland", 1030300}
            };

            DataImportGeneral.AddPopulationData(electionId, populationDictionary);
        }

        private static List<string> GetTitleFirstnameLastnamePartynameFromLine(IReadOnlyList<string> line)
        {
            const int firstnameIndex = 0;
            const int lastnameIndex = 1;
            const int partyIndex = 3;

            var firstname = line[firstnameIndex].Trim();
            var lastname = line[lastnameIndex].Trim();
            var partyname = line[partyIndex].Trim();

            // Extract title from firstname
            string title = null;
            for (var k = 0; k < DataImportGeneral.TitleList.Count; k++)
            {
                var titleElement = DataImportGeneral.TitleList.ElementAt(k);
                if (firstname.StartsWith(titleElement))
                {
                    title = titleElement;
                    firstname = firstname.Substring(titleElement.Length).Trim();
                    break;
                }
            }

            // Add default party to impartially people
            if (string.IsNullOrWhiteSpace(partyname))
            {
                partyname = DataImportGeneral.Impartially;
            }

            // Match parties to the same name
            if (DataImportGeneral.PartyNameDictionary.ContainsKey(partyname))
            {
                partyname = DataImportGeneral.PartyNameDictionary[partyname];
            }

            return new List<string> {title, firstname, lastname, partyname};
        }

        public static void AddPeople(string filename)
        {
            const int titleIndex = 0;
            const int firstnameIndex = 1;
            const int lastnameIndex = 2;
            const int partynameIndex = 3;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            const int startIndex = 1;

            using (var context = new ElectionDBEntities())
            {
                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    var result = GetTitleFirstnameLastnamePartynameFromLine(line);
                    var title = result.ElementAt(titleIndex);
                    var firstname = result.ElementAt(firstnameIndex);
                    var lastname = result.ElementAt(lastnameIndex);
                    var partyname = result.ElementAt(partynameIndex);

                    var existing = context.People.Where(p => p.Firstname == firstname && p.Lastname == lastname);
                    if (existing.Any(e => e.PartyAffiliations.Any(a => a.Party.Name == partyname)))
                    {
                        if (!(firstname == "Andreas" && lastname == "Müller" && int.Parse(line[2]) == 1959))
                        {
                            Console.WriteLine("{0} {1} {2} ({3}) does already exist", title ?? "", firstname, lastname,
                                partyname);
                            continue;
                        }
                    }

                    var entry = new Person
                    {
                        Title = title,
                        Firstname = firstname,
                        Lastname = lastname
                    };

                    context.People.Add(entry);

                    Console.WriteLine("Added {0} {1} {2} ({3})", title ?? "", firstname, lastname, partyname);
                }

                context.SaveChanges();
            }
        }

        public static void AddPartyAffiliations(string filename)
        {
            const int firstnameIndex = 1;
            const int lastnameIndex = 2;
            const int partynameIndex = 3;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            const int startIndex = 1;

            using (var context = new ElectionDBEntities())
            {
                var partyDictionary = context.Parties.ToDictionary(p => p.Name, p => p.Id);

                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    var result = GetTitleFirstnameLastnamePartynameFromLine(line);
                    var firstname = result.ElementAt(firstnameIndex);
                    var lastname = result.ElementAt(lastnameIndex);
                    var partyname = result.ElementAt(partynameIndex);

                    var partyId = partyDictionary[partyname];

                    // Hack because there are two persons with the same name that cannot be distinguished on a better way
                    Person existing;
                    if (firstname == "Stephan" && lastname == "Beyer")
                    {
                        var allExisting = context.People.Where(p => p.Firstname == firstname && p.Lastname == lastname);
                        existing = partyname == DataImportGeneral.Impartially ? allExisting.OrderBy(r => r.Id).First() : allExisting.OrderByDescending(r => r.Id).First();
                    }
                    else if (firstname == "Mario" && lastname == "Ertel")
                    {
                        var allExisting = context.People.Where(p => p.Firstname == firstname && p.Lastname == lastname);
                        existing = partyname == "NPD" ? allExisting.OrderBy(r => r.Id).First() : allExisting.OrderByDescending(r => r.Id).First();
                    }
                    else if (firstname == "Christian" && lastname == "Fischer")
                    {
                        var allExisting = context.People.Where(p => p.Firstname == firstname && p.Lastname == lastname);
                        existing = partyname == "NPD" ? allExisting.OrderBy(r => r.Id).First() : allExisting.OrderByDescending(r => r.Id).First();
                    }
                    else if (firstname == "Frank" && lastname == "Hofmann")
                    {
                        var allExisting = context.People.Where(p => p.Firstname == firstname && p.Lastname == lastname);
                        existing = partyname == "SPD" ? allExisting.OrderBy(r => r.Id).First() : allExisting.OrderByDescending(r => r.Id).First();
                    }
                    else if (firstname == "Andreas" && lastname == "Schwarz")
                    {
                        var allExisting = context.People.Where(p => p.Firstname == firstname && p.Lastname == lastname);
                        existing = partyname == "GRÜNE" ? allExisting.OrderBy(r => r.Id).First() : allExisting.OrderByDescending(r => r.Id).First();
                    }
                    else if (firstname == "Andreas" && lastname == "Weber")
                    {
                        var allExisting = context.People.Where(p => p.Firstname == firstname && p.Lastname == lastname);
                        existing = partyname == "BüSo" ? allExisting.OrderBy(r => r.Id).First() : allExisting.OrderByDescending(r => r.Id).First();
                    }
                    else if (firstname == "Andreas" && lastname == "Müller")
                    {
                        var allExisting = context.People.Where(p => p.Firstname == firstname && p.Lastname == lastname);
                        existing = int.Parse(line[2]) == 1964 ? allExisting.OrderBy(r => r.Id).First() : allExisting.OrderByDescending(r => r.Id).First();
                    }
                    else
                    {
                        existing = context.People.Single(p => p.Firstname == firstname && p.Lastname == lastname &&
                                                                  (!p.PartyAffiliations.Any() ||
                                                                   p.PartyAffiliations.Any(
                                                                       a =>
                                                                           a.Election_Id == DataImport2013.electionId &&
                                                                           a.Party_Id == partyId)));
                    }
                    var entry = new PartyAffiliation
                    {
                        Election_Id = electionId,
                        Person_Id = existing.Id,
                        Party_Id = partyId
                    };

                    context.PartyAffiliations.Add(entry);

                    Console.WriteLine("Added person {0} to party {1} in election {2}", existing.Id, partyId, electionId);
                }

                context.SaveChanges();
            }
        }

        private static Person GetExistingCandidate(ElectionDBEntities context, string firstname, string lastname, string partyname, string birthyear)
        {
            Person existing;
            // Hack because the two persons cannot be distinguished otherwise
            if (firstname == "Andreas" && lastname == "Müller" && partyname == "DIE LINKE")
            {
                var allExisting =
                    context.People.Where(
                        p =>
                            p.Firstname == firstname && p.Lastname == lastname &&
                            p.PartyAffiliations.Any(
                                a => a.Election_Id == electionId && a.Party.Name == partyname));
                existing = int.Parse(birthyear) == 1964
                    ? allExisting.OrderBy(r => r.Id).First()
                    : allExisting.OrderByDescending(r => r.Id).First();
            }
            else
            {
                existing =
                    context.People.Single(
                        p =>
                            p.Firstname == firstname && p.Lastname == lastname &&
                            p.PartyAffiliations.Any(
                                a => a.Election_Id == electionId && a.Party.Name == partyname));
            }

            return existing;
        }

        public static void AddIsElectableCandidate(string filename)
        {
            const int firstnameIndex = 1;
            const int lastnameIndex = 2;
            const int partynameIndex = 3;
            const int wahlkreisIdIndex = 4;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            const int startIndex = 1;

            using (var context = new ElectionDBEntities())
            {
                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    var result = GetTitleFirstnameLastnamePartynameFromLine(line);
                    var firstname = result.ElementAt(firstnameIndex);
                    var lastname = result.ElementAt(lastnameIndex);
                    var partyname = result.ElementAt(partynameIndex);

                    int wahlkreisId;
                    var parsed = int.TryParse(line[wahlkreisIdIndex], out wahlkreisId);
                    if (!parsed)
                    {
                        continue;
                    }

                    var existing = GetExistingCandidate(context, firstname, lastname, partyname, line[2]);

                    var entry = new IsElectableCandidate
                    {
                        Election_Id = electionId,
                        Person_Id = existing.Id,
                        Wahlkreis_Id = wahlkreisId
                    };

                    context.IsElectableCandidates.Add(entry);

                    Console.WriteLine("Added person {0} is electable at wahlkreis {1} in election {2}", existing.Id, wahlkreisId, electionId);
                }

                context.SaveChanges();
            }
        }

        public static void AddCandidateList(string filename)
        {
            const int firstnameIndex = 1;
            const int lastnameIndex = 2;
            const int partynameIndex = 3;
            const int bundeslandIndex = 5;
            const int positionIndex = 6;

            var parsedFile = DataImportGeneral.ParseFile(filename);

            const int startIndex = 1;

            using (var context = new ElectionDBEntities())
            {
                var bundeslandDictionary = context.Bundeslands.ToDictionary(b => b.Name, b => b.Id);

                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    var result = GetTitleFirstnameLastnamePartynameFromLine(line);
                    var firstname = result.ElementAt(firstnameIndex);
                    var lastname = result.ElementAt(lastnameIndex);
                    var partyname = result.ElementAt(partynameIndex);

                    var bundeslandName = line[bundeslandIndex].Trim();
                    if (string.IsNullOrWhiteSpace(bundeslandName))
                    {
                        continue;
                    }

                    var bundeslandId = bundeslandDictionary[bundeslandName];

                    var position = int.Parse(line[positionIndex]);

                    var existing = GetExistingCandidate(context, firstname, lastname, partyname, line[2]);

                    var entry = new CandidateList
                    {
                        Election_Id = electionId,
                        Bundesland_Id = bundeslandId,
                        Person_Id = existing.Id,
                        Position = position
                    };

                    context.CandidateLists.Add(entry);

                    Console.WriteLine("Added person {0} for bundesland {1} on position {2} in election {3}", existing.Id, bundeslandId, position, electionId);
                }

                context.SaveChanges();
            }
        }
    }
}
