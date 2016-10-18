using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using System;

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

        public bool AreBPBusinessRuleActionsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[bp].[BPBusinessRuleAction]", ref updateHandle);
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