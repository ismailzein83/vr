using Demo.Module.Entities;
using System;
using System.Collections.Generic;

namespace Demo.Module.MainExtension.Team
{
    public class MediumTeamType : TeamSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public List<Player> Players { get; set; }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}