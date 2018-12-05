using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPBusinessRuleActionDataManager : BaseSQLDataManager, IBPBusinessRuleActionDataManager
    {
        public BPBusinessRuleActionDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "ConfigurationDBConnString"))
        {

        }
        #region public methods
        public List<BPBusinessRuleAction> GetBPBusinessRuleActions()
        {
            return GetItemsSP("bp.sp_BPBusinessRuleAction_GetAll", BPBusinessRuleActionMapper);
        }

        #endregion

        #region Mappers

        BPBusinessRuleAction BPBusinessRuleActionMapper(IDataReader reader)
        {
            BPBusinessRuleAction BPBusinessRuleAction = new BPBusinessRuleAction()
            {
                BPBusinessRuleActionId = (int)reader["ID"],
                Details = new BPBusinessRuleActionDetails()
                {
                    BPBusinessRuleDefinitionId = GetReaderValue<Guid>(reader,"BusinessRuleDefinitionId"),
                    Settings = Serializer.Deserialize<BPBusinessRuleActionSettings>(reader["Settings"] as string)
                }
            };
            return BPBusinessRuleAction;
        }
        #endregion
    }
}