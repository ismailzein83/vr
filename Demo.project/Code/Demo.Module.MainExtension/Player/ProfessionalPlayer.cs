using Demo.Module.Entities;
using System;

namespace Demo.Module.MainExtension.Player
{
    public class ProfessionalPlayer : PlayerType
    {
        public override Guid ConfigID { get { return new Guid("92089C36-B820-4619-9060-98A503ED452B"); } }

        public int YearsOfExperience { get; set; }

        public int Salary { get; set; }
    }
}