using Demo.Module.Entities;
using System;

namespace Demo.Module.MainExtension.Team
{
    public class SmallTeamType : TeamSettings
    {
        public override Guid ConfigID { get { return new Guid("721D8101-8064-42D2-8DE9-8D34D096359B"); } }

        public int NumberOfPlayers { get; set; }

        public override string GetDescription()
        {
            return "Small Team, City: " + City + ", Stadium: " + Stadium + ", Number Of Players: " + NumberOfPlayers.ToString();
        }
    }
}