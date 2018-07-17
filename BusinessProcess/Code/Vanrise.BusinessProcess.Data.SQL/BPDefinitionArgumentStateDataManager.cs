using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.BusinessProcess.Data.SQL
{
    public class BPDefinitionArgumentStateDataManager : BaseSQLDataManager, IBPDefinitionArgumentStateDataManager
    {
        public BPDefinitionArgumentStateDataManager()
            : base(GetConnectionStringName("BusinessProcessConfigDBConnStringKey", "BusinessProcessDBConnString"))
        {

        }
        #region public methods
        public List<BPDefinitionArgumentState> GetBPDefinitionArgumentStates()
        {
            return GetItemsSP("bp.sp_BPDefinitionArgumentState_GetAll", BPDefinitionArgumentStateMapper);
        }

        public bool InsertOrUpdateBPDefinitionArgumentState(BPDefinitionArgumentState bpDefinitionArgumentState)
        {
            return ExecuteNonQuerySP("[bp].[sp_BPDefinitionArgumentState_InsertOrUpdate]", bpDefinitionArgumentState.BPDefinitionID, Vanrise.Common.Serializer.Serialize(bpDefinitionArgumentState.InputArgument)) > 0;
        }

        #endregion

        #region Mappers

        BPDefinitionArgumentState BPDefinitionArgumentStateMapper(IDataReader reader)
        {
            string argument = reader["InputArgument"] as string;

            return new BPDefinitionArgumentState
            {
                BPDefinitionID = GetReaderValue<Guid>(reader, "BPDefinitionID"),
                InputArgument = !String.IsNullOrWhiteSpace(argument) ? Serializer.Deserialize<BaseProcessInputArgument>(argument) : null
            };
        }
        #endregion
    }
}