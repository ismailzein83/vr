using Demo.Module.Entities;
using System;

namespace Demo.Module.MainExtension.Branch
{
    public class MediumBranch : BranchType
    {
        public override Guid ConfigId { get { return new Guid("56F72729-D48E-4EBC-812C-07FA88177DE2"); } }

        public int NumberOfBlocks { get; set; }

        public override string GetDescription()
        {
            return string.Format("Medium size branch with {0} blocks", this.NumberOfBlocks);
        }
    }
}