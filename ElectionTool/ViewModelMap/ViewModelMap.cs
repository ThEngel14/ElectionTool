using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionTool.Entity_Framework;
using ElectionTool.Models;

namespace ElectionTool.ViewModelMap
{
    public class ViewModelMap
    {
        public static IEnumerable<WinnerFirstVotesModel> GetWinnerFirstVotes(IEnumerable<WinnerErststimme> entries)
        {
            return entries.Select(GetWinnerFirstVotes);
        }

        public static WinnerFirstVotesModel GetWinnerFirstVotes(WinnerErststimme entry)
        {
            return new WinnerFirstVotesModel
            {
                ElectionId = entry.Election_Id,
                Party = entry.Party_Name,
                Wahlkreis = entry.Wahlkreis_Name,
                Person = string.Format("{0} {1} {2}", entry.Title, entry.Firstname, entry.Lastname)
            };
        }
    }
}