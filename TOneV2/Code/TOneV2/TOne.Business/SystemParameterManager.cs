using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data;

namespace TOne.Business
{
    public class SystemParameterManager
    {
        public T GetSystemParameterValue<T>(Entities.SystemParameter systemParameter)
        {
            ISystemParameterDataManger dataManager = DataManagerFactory.GetDataManager<ISystemParameterDataManger>();
            return dataManager.GetSystemParameterValue<T>(systemParameter);
        }
    }
}
