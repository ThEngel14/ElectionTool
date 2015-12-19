using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class PartyColorViewModel : PartyViewModel
    {
        private readonly static Dictionary<int, string> ColorDictionary = new Dictionary<int, string>
        {
            {1, "#000000"},
            {2, "#FF0000"},
            {3, "#FFFF00"},
            {4, "#EE82EE"},
            {5, "#008000"},
            {6, "#000000"}
        };

        private readonly static Dictionary<int, string> HightlightDictionary = new Dictionary<int, string>
        {
            {1, "#404040"},
            {2, "#FF4040"},
            {3, "#FFFF40"},
            {4, "#EEA2EE"},
            {5, "#00BB00"},
            {6, "#404040"}
        }; 

        public string Color
        {
            get
            {
                return ColorDictionary.ContainsKey(Id) ? ColorDictionary[Id] : "#E3E3E3";
            }
        }

        public string Highlight
        {
            get
            {
                return HightlightDictionary.ContainsKey(Id) ? HightlightDictionary[Id] : "#F5F5F5";
            }
        }
    }
}