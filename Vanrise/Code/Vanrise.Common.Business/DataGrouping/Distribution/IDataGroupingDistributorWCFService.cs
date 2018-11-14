using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    public class DataGroupingDistributorWCFService : IDataGroupingDistributorWCFService
    {
        static DataGroupingDistributor s_dataGroupingDistributor = new DataGroupingDistributor();

        public void InitializeDataGroupingAnalysisIfNeeded(string dataAnalysisUniqueName, string serializedGroupingHandler)
        {
            s_dataGroupingDistributor.InitializeDataGroupingAnalysisIfNeeded(dataAnalysisUniqueName, serializedGroupingHandler);
        }

        public List<DataGroupingDistributionInfo> GetItemKeysDistributionInfos(string dataAnalysisUniqueName, List<string> itemKeys)
        {
            return s_dataGroupingDistributor.GetItemKeysDistributionInfos(dataAnalysisUniqueName, itemKeys);
        }

        public List<Guid> GetExecutorServiceInstanceIds(string dataAnalysisUniqueName)
        {
            return s_dataGroupingDistributor.GetExecutorServiceInstanceIds(dataAnalysisUniqueName);
        }
    }

    public interface IDataGroupingDistributorWCFService
    {
        void InitializeDataGroupingAnalysisIfNeeded(string dataAnalysisUniqueName, string serializedGroupingHandler);
        
        List<DataGroupingDistributionInfo> GetItemKeysDistributionInfos(string dataAnalysisUniqueName, List<string> itemKeys);
        
        List<Guid> GetExecutorServiceInstanceIds(string _dataAnalysisUniqueName);
    }

    public class DataGroupingDistributionInfo
    {
        public Guid ExecutorServiceInstanceId { get; set; }

        public List<string> ItemKeys { get; set; }
    }
}
