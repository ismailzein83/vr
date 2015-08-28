using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    public class GenericAnalyticDataManager : BaseTOneDataManager, IGenericAnalyticDataManager
    {
        //private GenericAnalyticConfigManager _manager =  new GenericAnalyticConfigManager();

        public Vanrise.Entities.BigResult<AnalyticRecord> GetAnalyticSummary(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            GenericAnalyticConfigManager _manager = new GenericAnalyticConfigManager();
            StringBuilder queryBuilder = new StringBuilder(@"SELECT ");
            
            for (int i = 0; i < input.Query.MeasureFields.Count(); i++)
            {
                queryBuilder.Append(input.Query.MeasureFields[i] + ", ");
            }
            queryBuilder.Append(" FROM TrafficStatsDaily");

            for (int i = 0; i < input.Query.GroupFields.Count(); i++)
            {
                queryBuilder.Append(input.Query.MeasureFields[i] + ", ");
            }

            Dictionary<AnalyticGroupField, AnalyticGroupFieldConfig> groupFieldsConfig =  _manager.GetGroupFieldsConfig(input.Query.GroupFields);
            Dictionary<AnalyticMeasureField, AnalyticMeasureFieldConfig> measureFieldsConfig = _manager.GetMeasureFieldsConfig(input.Query.MeasureFields);

            return null;
        }
    }
}
