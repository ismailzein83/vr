using System;

namespace Demo.Module.Entities
{
    public class Team
    {
        public int TeamID { get; set; } 
        public string Name { get; set; }
        public int LeagueID { get; set; }
        public TeamSettings Settings { get; set; }
    }

    public abstract class TeamSettings
    {
        public abstract Guid ConfigID { get; }

        public string City { get; set; }

        public string Stadium { get; set; }

        public abstract string GetDescription();
    }
}