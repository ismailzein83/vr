using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class CallClassDataManager : BaseSQLDataManager, ICallClassDataManager
    {

        public CallClassDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public List<CallClass> GetCallClasses()
        {
            return GetItemsSP("FraudAnalysis.sp_CallClass_GetAll", CallClassMapper);
        }

        public bool AreCallClassesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("FraudAnalysis.CallClass", ref updateHandle);
        }

        #region Private Methods

        private CallClass CallClassMapper(IDataReader reader)
        {
            var callClass = new CallClass();
            callClass.Id = (int)reader["Id"];
            callClass.Description = reader["Description"] as string;
            callClass.NetType = (NetType)reader["NetType"];
            return callClass;
        }

        #endregion

    }
}
