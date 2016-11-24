using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data
{
    public interface ICodeGroupDataManager : IDataManager
    {
        List<CodeGroup> GetCodeGroups();
        bool Update(CodeGroupToEdit codeGroup);
        bool Insert(CodeGroup codeGroup, out int insertedId);
        void SaveCodeGroupToDB(List<CodeGroup> codeGroups);
        bool AreCodeGroupUpdated(ref object updateHandle);
    }
}
