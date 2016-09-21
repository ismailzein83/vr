
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;


namespace TONEAPI.FakreManagers
{
    public class CodeGroupDataManager : ICodeGroupDataManager
    {
        public bool AreCodeGroupUpdated(ref object updateHandle)
        {
            return false;
        }

        public List<TOne.WhS.BusinessEntity.Entities.CodeGroup> GetCodeGroups()
        {
            connect con = new connect();
            List<TOne.WhS.BusinessEntity.Entities.CodeGroup> codeGroups = con.getcodegroup("SELECT  [ID] ,[CountryID],[Code] FROM [MvtsProDemoV2].[TOneWhS_BE].[CodeGroup]");


            //TOne.WhS.BusinessEntity.Entities.CodeGroup c1 = new TOne.WhS.BusinessEntity.Entities.CodeGroup() { Code = "961", CodeGroupId = 1, CountryId = 1 };
            //List<TOne.WhS.BusinessEntity.Entities.CodeGroup> codeGroups = new List<TOne.WhS.BusinessEntity.Entities.CodeGroup>();
            //codeGroups.Add(c1);

            return codeGroups;
        }

        public bool Insert(TOne.WhS.BusinessEntity.Entities.CodeGroup codeGroup, out int insertedId)
        {
            throw new NotImplementedException();
        }

        public void SaveCodeGroupToDB(List<TOne.WhS.BusinessEntity.Entities.CodeGroup> codeGroups)
        {
            throw new NotImplementedException();
        }

        public bool Update(TOne.WhS.BusinessEntity.Entities.CodeGroupToEdit codeGroup)
        {
            throw new NotImplementedException();
        }
    }
}
