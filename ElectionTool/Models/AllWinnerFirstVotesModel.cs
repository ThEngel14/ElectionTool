using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class AllWinnerFirstVotesModel
    {
        public IEnumerable<WinnerFirstVotesModel> Winner { get; set; } 
    }
}