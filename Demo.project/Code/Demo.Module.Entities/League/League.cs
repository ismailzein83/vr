using System;

namespace Demo.Module.Entities
{
    public class League
    {
        public int LeagueID { get; set; }
        public string Name { get; set; }
        public LeagueSettings Settings { get; set; }
    }

    public class LeagueSettings
    {
        public string Country { get; set; }

        public string GetDescription()
        {
            return "Country: " + Country;
        }
    }
}