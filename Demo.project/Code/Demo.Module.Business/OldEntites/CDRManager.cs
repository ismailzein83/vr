using Demo.Module.Data;
using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Demo.Module.Business
{
    public class CDRManager
    {

        #region Public Methods
        public List<CDR> GetCDR(DataRetrievalInput<CDRQuery> input)
        {
            ICDRDataManager cdrDataManager = DemoModuleFactory.GetDataManager<ICDRDataManager>();
            return cdrDataManager.GetCDR(input);
        }

        #endregion
    }
}
