using Demo.Module.Entities;
using System;

namespace Demo.Module.MainExtension.Player
{
    public class ProfessionalPlayer : PlayerType
    {
        public override Guid ConfigID => throw new NotImplementedException();

        public int YearsOfExperience { get; set; }

        public int Salary { get; set; }
    }
}