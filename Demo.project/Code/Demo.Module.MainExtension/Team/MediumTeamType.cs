using Demo.Module.Entities;
using System;
using System.Collections.Generic;

namespace Demo.Module.MainExtension.Team
{
    public class MediumTeamType : TeamSettings
    {
        public override Guid ConfigID { get { return new Guid("74DE94A0-008C-4558-B243-7153B21BA796"); } }

        public List<Demo.Module.Entities.Player> Players { get; set; }

        public override string GetDescription()
        {
            return "Medium Team, City: " + City + ", Stadium: " + Stadium + ", Number Of Players: " + Players.Count;
        }
    }
}