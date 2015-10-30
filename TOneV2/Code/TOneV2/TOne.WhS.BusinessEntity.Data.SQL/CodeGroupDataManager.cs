using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CodeGroupDataManager : BaseSQLDataManager, ICodeGroupDataManager
    {

        public CodeGroupDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        private CodeGroup CodeGroupMapper(IDataReader reader)
        {
            CodeGroup codeGroup = new CodeGroup
            {
                CodeGroupId = (int)reader["ID"],
                Code = reader["Code"] as string,
                CountryId = (int)reader["CountryID"] 
                 
            };

            return codeGroup;
        }

        
        public List<CodeGroup> GetCodeGroups()
        {
            return GetItemsSP("TOneWhS_BE.sp_CodeGroup_GetAll", CodeGroupMapper);
        }
        public bool Update(CodeGroup codeGroup)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CodeGroup_Update", codeGroup.CodeGroupId, codeGroup.CountryId, codeGroup.Code);
            return (recordsEffected > 0);
        }

        public bool Insert(CodeGroup codeGroup, out int insertedId)
        {
            object codeGroupId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CodeGroup_Insert", out codeGroupId, codeGroup.Code, codeGroup.CountryId);
            insertedId = (int)codeGroupId;
            return (recordsEffected > 0);
        }

        public bool AreCodeGroupUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CodeGroup", ref updateHandle);
        }


    }
}
