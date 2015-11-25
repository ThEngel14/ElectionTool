using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElectionTool.Models
{
    public class AllMemberOfBundestagViewModel
    {
        public int ElectionId { get; set; }

        public IEnumerable<MemberOfBundestagViewModel> Members { get; set; } 
    }
}