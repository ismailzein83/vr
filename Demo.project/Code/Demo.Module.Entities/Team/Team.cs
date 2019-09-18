using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int LeagueId { get; set; }
        public TeamSettings Settings { get; set; }
    }

    public class TeamSettings
    {
        public string City { get; set; }

        public string Stadium { get; set; }

        public TeamType TeamType { get; set; }
    }

    public abstract class TeamType
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetDescription();
    }

    public abstract class Player
    {

    }
}