using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;


namespace TOne.WhS.BusinessEntity.Data
{
    public interface ICodeGroupDataManager : IDataManager
    {
        List<CodeGroup> GetCodeGroups();
        bool Update(CodeGroup codeGroup);
        bool Insert(CodeGroup codeGroup, out int insertedId);
        bool AreCodeGroupUpdated(ref object updateHandle);
       
    }
}
