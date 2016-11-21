using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Data.SQL
{
    public class CodeGroupDataManager : BaseSQLDataManager, ICodeGroupDataManager
    {
        #region ctor/Local Variables
        public CodeGroupDataManager()
            : base(GetConnectionStringName("NumberingPlanDBConnStringKey", "NumberingPlanDBConnString"))
        {
        }

        #endregion

        #region Public Methods
        public List<CodeGroup> GetCodeGroups()
        {
            return GetItemsSP("VR_NumberingPlan.sp_CodeGroup_GetAll", CodeGroupMapper);
        }
        public bool AreCodeGroupUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("VR_NumberingPlan.CodeGroup", ref updateHandle);
        }
       
        #endregion

       
        #region  Mappers
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
        #endregion
    }
}
