using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class PersonViewModel : IComparable<PersonViewModel>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        [Display(Name = "Name")]
        public string Fullname { get { return string.Format("{0} {1} {2}", Title, Firstname, Lastname); } }

        public int CompareTo(PersonViewModel other)
        {
            var lastcomp = Lastname.CompareTo(other.Lastname);
            if (lastcomp != 0)
            {
                return lastcomp;
            }

            if (Firstname == null)
            {
                return -1;
            }
            return Firstname.CompareTo(other.Firstname);
        }
    }
}