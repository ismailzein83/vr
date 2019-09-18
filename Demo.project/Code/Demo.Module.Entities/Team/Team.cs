using System;

namespace Demo.Module.Entities
{
    public class Team
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int LeagueId { get; set; }
        public TeamSettings Settings { get; set; }
    }

    public abstract class TeamSettings
    {
        public abstract Guid ConfigId { get; }

        public string City { get; set; }

        public string Stadium { get; set; }

        public abstract string GetDescription();
    }

    public class Player
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public PlayerType Type { get; set; }
    }

    public abstract class PlayerType
    {
        public abstract Guid ConfigId { get; }

        public string Nationality { get; set; } 
    }

    public class ProfessionalPlayer : PlayerType
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public int YearsOfExperience { get; set; }

        public int Salary { get; set; }
    }

    public class BeginnerPlayer : PlayerType
    {
        public override Guid ConfigId => throw new NotImplementedException();
    }
}