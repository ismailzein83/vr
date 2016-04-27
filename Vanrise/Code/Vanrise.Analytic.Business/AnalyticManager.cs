using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace Vanrise.Analytic.Business
{
    public class AnalyticManager
    {
        public Vanrise.Entities.IDataRetrievalResult<AnalyticRecord> GetFilteredRecords(Vanrise.Entities.DataRetrievalInput<AnalyticQuery> input)
        {
            IAnalyticDataManager dataManager = AnalyticDataManagerFactory.GetDataManager<IAnalyticDataManager>();
            AnalyticTableManager analyticTableManager = new AnalyticTableManager();
            AnalyticItemConfigManager analyticItemConfigManager = new AnalyticItemConfigManager();
            var table = analyticTableManager.GetAnalyticTableById(input.Query.TableId);
            if (table == null)
                throw new NullReferenceException(String.Format("table.ID '{0}'", input.Query.TableId));
            if (table.Settings == null)
                throw new NullReferenceException(String.Format("table.Settings.ID '{0}'", table.AnalyticTableId));
            dataManager.Table = table;
            dataManager.Dimensions = analyticItemConfigManager.GetDimensions(table.AnalyticTableId);
            dataManager.Measures = analyticItemConfigManager.GetMeasures(table.AnalyticTableId);
            dataManager.Joins = analyticItemConfigManager.GetJoins(table.AnalyticTableId);
            var analyticRecords = BigDataManager.Instance.RetrieveData(input, new AnalyticRecordRequestHandler(dataManager));
            AnalyticSummaryBigResult<AnalyticRecord> result = analyticRecords as AnalyticSummaryBigResult<AnalyticRecord>;
            if (input.Query.WithSummary)
                result.Summary = dataManager.GetAnalyticSummary(input);

            return result;
        }

        private class AnalyticRecordRequestHandler : BigDataRequestHandler<AnalyticQuery, AnalyticRecord, AnalyticRecord>
        {
            IAnalyticDataManager _dataManager;
            public AnalyticRecordRequestHandler(IAnalyticDataManager dataManager)
            {
                _dataManager = dataManager;
            }
            public override AnalyticRecord EntityDetailMapper(AnalyticRecord entity)
            {
                return entity;
            }

            public override IEnumerable<AnalyticRecord> RetrieveAllData(DataRetrievalInput<AnalyticQuery> input)
            {
                return _dataManager.GetAnalyticRecords(input);
            }
        }
    }
}
