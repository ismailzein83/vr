using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Fzero.FraudAnalysis.Aggregates;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class AggregateDataManager : BaseSQLDataManager, IAggregateDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        public AggregateDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public List<AggregateDefinitionInfo> GetAggregates()
        {
            return GetItemsSP("FraudAnalysis.sp_Aggregate_GetAll", AggregateDefinitionMapper);
        }

        public bool AreAggregatesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("FraudAnalysis.Aggregate", ref updateHandle);
        }

        #region Private Methods

        private AggregateDefinitionInfo AggregateDefinitionMapper(IDataReader reader)
        {
            AggregateDefinitionInfo aggregateDefinition = new AggregateDefinitionInfo();
            aggregateDefinition.Id = (int)reader["ID"];
            aggregateDefinition.Name = reader["Name"] as string;
            aggregateDefinition.NumberPrecision = reader["NumberPrecision"] as string;
            aggregateDefinition.OperatorTypeAllowed = (Vanrise.Fzero.Entities.OperatorType)reader["OperatorTypeAllowed"];
            return aggregateDefinition;
        }

        #endregion
    }
}
