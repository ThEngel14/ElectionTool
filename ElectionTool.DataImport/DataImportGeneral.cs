using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectionTool.Entity_Framework;

namespace ElectionTool.DataImport
{
    class DataImportGeneral
    {
        public static string Impartially = "Parteilos";

        public static Dictionary<int, string> BundeslaenderDictionary = new Dictionary<int, string>
        {
                {1, "Schleswig-Holstein"},
                {2, "Hamburg"},
                {3, "Niedersachsen"},
                {4, "Bremen"},
                {5, "Nordrhein-Westfalen"},
                {6, "Hessen"},
                {7, "Rheinland-Pfalz"},
                {8, "Baden-Württemberg"},
                {9, "Bayern"},
                {10, "Saarland"},
                {11, "Berlin"},
                {12, "Brandenburg"},
                {13, "Mecklenburg-Vorpommern"},
                {14, "Sachsen"},
                {15, "Sachsen-Anhalt"},
                {16, "Thüringen"}
            };

        public static Dictionary<string, string> BundeslaenderShortDictionary =  new Dictionary<string, string>
        {
                {"BW", "Baden-Württemberg"},
                {"BE", "Berlin"},
                {"BB", "Brandenburg"},
                {"HB", "Bremen"},
                {"HH", "Hamburg"},
                {"HE", "Hessen"},
                {"MV", "Mecklenburg-Vorpommern"},
                {"RP", "Rheinland-Pfalz"},
                {"NI", "Niedersachsen"},
                {"BY", "Bayern"},
                {"NW", "Nordrhein-Westfalen"},
                {"SL", "Saarland"},
                {"SN", "Sachsen"},
                {"ST", "Sachsen-Anhalt"},
                {"SH", "Schleswig-Holstein"},
                {"TH", "Thüringen"}
            };

        public static readonly Dictionary<string, string> PartyNameDictionary = new Dictionary<string, string>
        {
            {"VIOLETTEN", "DIE VIOLETTEN"},
            {"ödp", "ÖDP"},
            {"RRP", "Bündnis 21/RRP"},
            {"Tierschutz", "Tierschutzpartei"},
            {"Die Tierschutzpartei", "Tierschutzpartei"},
            {"Volksabst.", "Volksabstimmung"},
            {"FWD", "FREIE WÄHLER"},
            {"CM", "Christliche Mitte"},
            {"VERNUNFT", "PARTEI DER VERNUNFT"},
            {"Bündnis21/RRP", "Bündnis 21/RRP"},
            {"Übrige", Impartially}
        };

        public static List<string> TitleList = new List<string> { "Prof. Dr. Dr.", "Prof. Dr.", "Prof.", "Dr. Dr.", "Dr. jur.", "Dr."}; 

        public static List<List<string>> ParseFile(string filename, char splitter = ';')
        {
            var result = new List<List<string>>();

            string line;

            // Read the file and display it line by line.
            StreamReader file =
                new StreamReader(filename, Encoding.UTF8);
            while ((line = file.ReadLine()) != null)
            {
                result.Add(line.Split(splitter).ToList());
            }

            file.Close();

            return result;
        }

        public static void AddElection(DateTime date, int seats)
        {
            using (var context = new ElectionDBEntities())
            {
                context.Elections.Add(new Election
                {
                    Date = date,
                    SeatsBundestag = seats
                });

                context.SaveChanges();
            }
        }

        public static void AddBundeslaender()
        {
            using (var context = new ElectionDBEntities())
            {
                var bundesland = context.Bundeslands;
                bundesland.RemoveRange(bundesland.AsEnumerable());

                foreach (var entry in BundeslaenderDictionary)
                {
                    bundesland.Add(new Bundesland
                    {
                        Id = entry.Key,
                        Name = entry.Value
                    });
                }

                context.SaveChanges();
            }
        }

        public static void AddPopulationData(int electionId, Dictionary<string, int> population)
        {
            using (var context = new ElectionDBEntities())
            {
                var bundeslandIdDictionary = context.Bundeslands.ToDictionary(b => b.Name, b => b.Id);

                foreach (var entry in population)
                {

                    var bundeslandId = bundeslandIdDictionary[entry.Key];
                    context.PopulationBundeslands.Add(new PopulationBundesland
                    {
                        Election_Id = electionId,
                        Bundesland_Id = bundeslandId,
                        Count = entry.Value
                    });
                }

                context.SaveChanges();
            }
        }

        public static void AddWahlkreise(string filename)
        {
            const int wahlkreisIdIndex = 0;
            const int wahlkreisNameIndex = 1;
            const int bundeslandIdIndex = 2;

            const int skippedBundeslandId = 99;


            var parsedFile = ParseFile(filename);

            const int startIndex = 5;

            using (var context = new ElectionDBEntities())
            {
                for (var i = startIndex; i < parsedFile.Count; i++)
                {
                    var line = parsedFile.ElementAt(i);

                    int bundeslandId;
                    var parsed = Int32.TryParse(line[bundeslandIdIndex], out bundeslandId);

                    if (!parsed || bundeslandId == skippedBundeslandId)
                    {
                        // increase i because of next line is empty
                        i++;
                        continue;
                    }

                    context.Wahlkreis.Add(new Wahlkrei
                    {
                        Id = Int32.Parse(line[wahlkreisIdIndex]),
                        Name = line[wahlkreisNameIndex].Trim(),
                        Bundesland_Id = bundeslandId
                    });
                }

                context.SaveChanges();
            }
        }
    }
}
