using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class ElectionViewModel
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int SeatsBundestag { get; set; }
    }
}