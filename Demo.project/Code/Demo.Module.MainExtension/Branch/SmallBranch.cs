using Demo.Module.Entities;
using System;

namespace Demo.Module.MainExtension.Branch
{
    public class SmallBranch : BranchType
    {
        public override Guid ConfigId { get { return new Guid("BCE471CE-F9AC-4F01-A10A-E63B08DAB86B"); } }

        public int NumberOfRooms { get; set; }

        public override string GetDescription()
        {
           return string.Format("Small size branch with {0} rooms", this.NumberOfRooms);
        }
    }
}