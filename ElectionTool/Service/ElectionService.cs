using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ElectionTool.Entity_Framework;
using ElectionTool.Models;

namespace ElectionTool.Service
{
    public class ElectionService
    {
        public AllWinnerFirstVotesModel GetAllWinnerFirstVotes()
        {
            var winner = new AllWinnerFirstVotesModel();

            using (var context = new ElectionDBEntities())
            {
                var winnerFirstVotes = context.WinnerErststimmes
                    .Where(w => w.Election_Id == 2).AsEnumerable();

                winner.Winner = ViewModelMap.ViewModelMap.GetWinnerFirstVotes(winnerFirstVotes).ToList().OrderBy(r => r);
            }

            return winner;
        }
    }
}