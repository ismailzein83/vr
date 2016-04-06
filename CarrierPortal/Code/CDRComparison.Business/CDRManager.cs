using CDRComparison.Data;
using CDRComparison.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
namespace CDRComparison.Business
{
    public class CDRManager
    {
        public int GetAllCDRsCount(string tableKey)
        {
            ICDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.TableNameKey = tableKey;
            return dataManager.GetAllCDRsCount();
        }
        public IDataRetrievalResult<CDR> GetFilteredCDRs(DataRetrievalInput<CDRQuery> input)
        {
            ICDRDataManager dataManager = CDRComparisonDataManagerFactory.GetDataManager<ICDRDataManager>();
            dataManager.TableNameKey = input.Query.TableKey;
            return DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredCDRs(input));
        }
    }
}
