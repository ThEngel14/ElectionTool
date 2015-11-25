using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class PersonWithPartyViewModel : IComparable<PersonWithPartyViewModel>
    {
        public PersonViewModel Person { get; set; }

        public PartyViewModel Party { get; set; }

        public int CompareTo(PersonWithPartyViewModel other)
        {
            var personcomp = Person.CompareTo(other.Person);
            return personcomp != 0 ? personcomp : Party.CompareTo(other.Party);
        }
    }
}