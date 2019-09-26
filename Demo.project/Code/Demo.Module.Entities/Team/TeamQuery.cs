using System.Collections.Generic;

namespace Demo.Module.Entities
{
    public class TeamQuery
    {
        public string Name { get; set; }

        public List<int> LeagueIDs { get; set; }
    }
}