using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPBusinessRuleActionTypeDataManager : BaseSQLDataManager, IBPBusinessRuleActionTypeDataManager
    {
        public BPBusinessRuleActionTypeDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #region public methods
        public List<BPBusinessRuleActionType> GetBPBusinessRuleActionTypes()
        {
            return GetItemsSP("bp.sp_BPBusinessRuleActionType_GetAll", BPBusinessRuleActionTypeMapper);
        }

        public bool AreBPBusinessRuleActionTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[BPBusinessRuleActionType]", ref updateHandle);
        }
        #endregion

        #region Mappers

        BPBusinessRuleActionType BPBusinessRuleActionTypeMapper(IDataReader reader)
        {
            return Serializer.Deserialize<BPBusinessRuleActionType>(reader["Settings"] as string);
        }
        #endregion
    }
}