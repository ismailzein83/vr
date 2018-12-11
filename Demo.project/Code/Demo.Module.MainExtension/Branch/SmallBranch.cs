using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.MainExtension.Branch
{
    public class SmallBranch : BranchType
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public int NumberOfRooms { get; set; }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
