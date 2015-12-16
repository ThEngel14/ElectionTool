

        /*generatormethode , die die untere aufruft. aus CSV auslesen welcher Wahlkreis(zeilennummer), wie viele (Zelleninhalt), 
        welche Partei (Spaltenüberschrift, evtl hardcoden welcher Index welcher Partei entspricht), 
        ob erstStimme oder zweitstimme damit machen ob die Spaltenzahl gerade oder ungerade ist*/
        
      public  static void generateVoteFor2013(string filename) //should be read from wkumrechnung2013 for 2013, or wk2009.csv for 2009 (ADAPT ELECTIONID in this case!)
        {
            const int WahlkreisIdIndex = 0;
            const int startIndex = 4; //passt für beide files
            var parsedFile = ParseFile(filename);

          try
          {
              var lineParties = parsedFile.ElementAt(2);
              var lineKindOfVote = parsedFile.ElementAt(3);


                //*debug line  for (int i= startIndex; i<startIndex+1;i++)
              for (int i = startIndex; i < parsedFile.Count; i++)
              {
                    var line = parsedFile.ElementAt(i);
                    var WahlkreisID = int.Parse(line[WahlkreisIdIndex]);
          
                    
                    //debug line for (int j = 18; j < 19; j++)
                     for (int j = 9; j < line.Count; j++) //ab Spalte 9/J fangen die Stimmdaten an, und hören in Spalte 60 auf- getestet
                    {
                        Debug.WriteLine(line[j].Trim());

                        var amount = line[j].Trim();

                      bool erststimme = (lineKindOfVote[j].Equals("Erststimmen"));
                      var partyname = lineParties[j];

                       // Debug.WriteLine("\n  Aufruf mit folgenden Daten: " + 2013+"(Wahl)    " + WahlkreisID+ "(WahlkreisID)    " + amount +"(Stimmen) fuer die " + partyname + " als Erststimme(" + erststimme +") \n");
                      DataImport.generateVote(2013, WahlkreisID, int.Parse(amount), partyname, erststimme);

            }
                 }


          }


          catch (Exception e)
          {
              throw e;
          }


        }



        static void generateVote(int election, int WahlkreisID, int amount, string ID, bool erststimme)
        //depending: erststimme -> (ID := PersonID), -erststimme -> (ID:=PartyID)
        { 
            using (var context = new ElectionDBEntities())
            {
                if (erststimme)
                {
                    int id = matchPartynameToID(ID);
                    var stimme = context.Erststimmes;

                    for (var i = 0; i < amount; i++)
                    {
                        stimme.Add(new Erststimme
                        {
                            Election_Id = election,
                            Wahlkreis_Id = WahlkreisID,
                            Person_Id = id
                        });
                    }
                }
                else
                {
                    int id = matchPartynameToPersonID(ID,WahlkreisID);
                    var stimme = context.Zweitstimmes;

                    for (var i = 0; i < amount; i++)
                    {
                        stimme.Add(new Zweitstimme
                        {
                            Election_Id = election,
                            Wahlkreis_Id = WahlkreisID,
                            Party_Id = id
                        });
                    }
                }

                context.SaveChanges();


            }
        }


        static int matchPartynameToID(string party)
        {
            /*Idee: um von dem String des Parteinamens auf die ID der Partei zu kommen
            iteriert man durch alle Partein, die in der DB bei dir sein sollten, und wo die Namen übereinstimmen
            gibt man die ID der PArtei zurück (schlüsselattribut!). Danach kann man die generierte Stimme dann 
            einer Partei zuordnen */
            using (var context = new ElectionDBEntities())
            {
                foreach (var p in context.Parties)
                {
                    if (party.Equals(p.Name))
                    {
                        return p.Id;
                    }
                }
                
            }
            return 0;
        }


        static int matchPartynameToPersonID(string party, int Wahlkreis)
        { /*Idee: da jede Partei pro Wahlkreis nur einen Kandidaten aufstellt, kann man durch die Kombination von beiden
            auf die Person (Person-ID) zugreifen. Da versteh ich allerdings das Schema nicht.. bitte ergänzen :(  sollte 
            aber nach muster der anderen MatchMethode machbar sein*/

            return 1;
        }