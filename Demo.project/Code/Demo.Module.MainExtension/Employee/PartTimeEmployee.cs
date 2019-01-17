using System;

namespace Demo.Module.MainExtension.Employee
{
    public class PartTimeEmployee : Demo.Module.Entities.Employee
    {
        public override Guid ConfigId { get { return new Guid("D4EAF33C-37E4-4DF6-8F43-884A88CB60D7"); } }

        public int NumberOfHourPerMonth { get; set; }

        public int HourRate { get; set; }

    }
}