using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class GenerateTokenViewModel
    {
        [Display(Name = "Bundestagswahl")]
        public IEnumerable<ElectionViewModel> Election { get; set; }

        public int SelectedElectionId { get; set; }

        [Display(Name = "Wahlkreis")]
        public IEnumerable<WahlkreisViewModel> Wahlkreise { get; set; }

        public int SelectedWahlkreisId { get; set; }

        [Display(Name = "Anzahl")]
        [Range(1, int.MaxValue, ErrorMessage = "Anzahl muss größer als 0 sein.")]
        public int Amount { get; set; }

        [Display(Name = "Passwort")]
        [Required]
        public string Password { get; set; }
    }
}