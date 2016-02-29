using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class FilterDataManager : BaseSQLDataManager, IFilterDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        public FilterDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public List<Filter> GetFilters()
        {
            return GetItemsSP("FraudAnalysis.sp_Filter_GetAll", FilterDefinitionMapper);
        }

        public bool AreFiltersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("FraudAnalysis.Filter", ref updateHandle);
        }

        #region Private Methods

        private Filter FilterDefinitionMapper(IDataReader reader)
        {
            Filter filterDefinition = new Filter();
            filterDefinition.FilterId = (int)reader["ID"];
            filterDefinition.Abbreviation =  reader["Abbreviation"] as string;
            filterDefinition.OperatorTypeAllowed = (Vanrise.Fzero.Entities.OperatorType)reader["OperatorTypeAllowed"];
            filterDefinition.Description = reader["Description"] as string;
            filterDefinition.Label = reader["Label"] as string;
            filterDefinition.ToolTip = reader["ToolTip"] as string;
            filterDefinition.ExcludeHourly = (bool)reader["ExcludeHourly"];
            filterDefinition.CompareOperator = (CriteriaCompareOperator)reader["CompareOperator"];
            filterDefinition.MinValue = (decimal)reader["MinValue"];
            filterDefinition.MaxValue = (decimal)reader["MaxValue"];
            filterDefinition.DecimalPrecision = (int)reader["DecimalPrecision"];
            return filterDefinition;
        }



        #endregion
    }
}
