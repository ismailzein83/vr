using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common.Business
{
    public class DataGroupingExecutorWCFService : IDataGroupingExecutorWCFService
    {
        public void InitializeDataGroupingAnalysis(string dataAnalysisUniqueName, string serializedGroupingHandler)
        {
            DataGroupingHandler handler = Utilities.DeserializeAndValidate<DataGroupingHandler>(serializedGroupingHandler);
            DataGroupingExecutor.Current.InitializeDataGroupingAnalysis(dataAnalysisUniqueName, handler);
        }

        public void AddItemsToGrouping(string dataAnalysisUniqueName, string serializedItems)
        {
            DataGroupingExecutor.Current.AddItemsToGrouping(dataAnalysisUniqueName, serializedItems);
        }

        public DataGroupingExecutorFinalResult GetFinalResults(string dataAnalysisUniqueName, int nbOfItems)
        {
            return DataGroupingExecutor.Current.GetFinalResults(dataAnalysisUniqueName, nbOfItems);
        }
    }

     [ServiceContract(Namespace = "http://runtime.vanrise.com/IDataGroupingExecutorWCFService")]
    public interface IDataGroupingExecutorWCFService
    {
         [OperationContract]
         void InitializeDataGroupingAnalysis(string dataAnalysisUniqueName, string serializedGroupingHandler);

         [OperationContract]
         void AddItemsToGrouping(string dataAnalysisUniqueName, string serializedItems);

         [OperationContract]
         DataGroupingExecutorFinalResult GetFinalResults(string dataAnalysisUniqueName, int nbOfItems);
    }

    public class DataGroupingExecutorFinalResult
    {
        public string SerializedItems { get; set; }

        public bool HasMoreResult { get; set; }
    }
}
