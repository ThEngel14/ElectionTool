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
        static void Main(string[] args)
        {
            using (var context = new ElectionDBEntities())
            {
                // Tabelle bekommen
                var bundesland = context.Bundeslands;

                foreach (var element in bundesland.AsEnumerable())
                {
                    // Tu etwas...
                    var id = element.Id;
                    var name = element.Name;

                    var alleWahlkreise = element.Wahlkreis.AsEnumerable();
                    //...
                }

                // Datensatz löschen
                var entry = bundesland.FirstOrDefault(r => r.Id == 9);
                if (entry != null)
                {
                    bundesland.Remove(entry);
                }

                // speichern
                context.SaveChanges();

                // Datensatz hinzufügen
                bundesland.Add(new Bundesland
                {
                    Id = 9,
                    Name = "Bayern"
                });

                context.SaveChanges();
            }
        }
    }
}
