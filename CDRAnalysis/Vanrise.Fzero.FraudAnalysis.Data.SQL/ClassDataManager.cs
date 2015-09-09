using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class ClassDataManager : BaseSQLDataManager, IClassDataManager
    {

        public ClassDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public List<CallClass> GetCallClasses()
        {
            return GetItemsSP("FraudAnalysis.sp_CallClass_GetAll", CallClassMapper);
        }

        #region Private Methods

        private CallClass CallClassMapper(IDataReader reader)
        {
            var callClass = new CallClass();
            callClass.Id = (int)reader["Id"];
            callClass.Description = reader["Description"] as string;
            callClass.NetType = (NetTypeEnum)Enum.ToObject(typeof(NetTypeEnum), GetReaderValue<int>(reader, "NetType"));
            return callClass;
        }

        #endregion

    }
}
