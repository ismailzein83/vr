using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Data;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;

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
            if (input.SortByColumnName.Contains("MeasureValues"))
            {
                string[] measureProperty = input.SortByColumnName.Split('.');
                input.SortByColumnName = string.Format(@"{0}[""{1}""]", measureProperty[0], measureProperty[1]);
            }
            var analyticRecords = BigDataManager.Instance.RetrieveData(input, new AnalyticRecordRequestHandler(dataManager));
            if (analyticRecords != null)
            {
                BigResult<AnalyticRecord> bigResult = analyticRecords as BigResult<AnalyticRecord>;
                if (bigResult != null)
                {
                    var rslt = new AnalyticSummaryBigResult<AnalyticRecord>()
                    {
                        ResultKey = bigResult.ResultKey,
                        Data = bigResult.Data,
                        TotalCount = bigResult.TotalCount
                    };
                    if (input.Query.WithSummary)
                        rslt.Summary = dataManager.GetAnalyticSummary(input);
                    return rslt;
                }
                else
                    return analyticRecords;
            }
            else
                return null;
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
