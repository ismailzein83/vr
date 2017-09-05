using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;
namespace Vanrise.Common.Business
{
    public class UtilityManager 
    {
        public DateTimeRange GetDateTimeRange()
        {
            IUtilityDataManager dataManager = CommonDataManagerFactory.GetDataManager<IUtilityDataManager>();
            return dataManager.GetDateTimeRange();
        }
    }
}
