using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class League
    {
        public int LeagueId { get; set; }
        public string Name { get; set; }
        public LeagueSettings Settings { get; set; }
    }

    public class LeagueSettings
    {
        public string Country { get; set; }
    }
}