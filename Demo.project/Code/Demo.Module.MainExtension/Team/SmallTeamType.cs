using Demo.Module.Entities;
using System;

namespace Demo.Module.MainExtension.Team
{
    public class SmallTeamType : TeamSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public int NumberOfPlayers { get; set; }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}