using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class DataGroupingAnalysisInfoManager
    {
        public  List<string> GetDataAnalysisNames(string dataAnalysisNamePrefix)
        {
            var dataManager = CommonDataManagerFactory.GetDataManager<IDataGroupingAnalysisInfoDataManager>();
            return dataManager.GetDataAnalysisNames(dataAnalysisNamePrefix);
        }
    }
}
