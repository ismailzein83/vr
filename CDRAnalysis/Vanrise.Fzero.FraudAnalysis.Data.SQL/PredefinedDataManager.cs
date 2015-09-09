using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.CDRImport.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class PredefinedDataManager : BaseSQLDataManager, IPredefinedDataManager
    {

        public PredefinedDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public List<CallClass> GetCallClasses()
        {
            return GetItemsSP("FraudAnalysis.sp_CallClass_GetAll", CallClassMapper);
        }

        public List<Period> GetPeriods()
        {

            var enumerationType = typeof(PeriodEnum);
            List<Period> periods = new List<Period>();

            foreach (int value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                periods.Add(new Period() { Id = value, Name = name });
            }

            return periods;
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
