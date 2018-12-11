using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Branch
{
    public class MediumBranch : BranchType
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public int NumberOfBlocks { get; set; }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
