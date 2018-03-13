using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface IBranchDataManager : IDataManager
    {
       List<Branch> GetBranches();       
       bool Insert(Branch branch, out int insertedId);
       bool Update(Branch branch);
       bool AreBranchesUpdated(ref object updateHandle);

    }
}
