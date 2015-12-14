using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Analytics.Entities;

namespace TOne.WhS.Analytics.Data
{
     public interface IRawCDRDataManager:IDataManager
    {
       Vanrise.Entities.BigResult<RawCDRLog> GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRInput> input);
    }
}
