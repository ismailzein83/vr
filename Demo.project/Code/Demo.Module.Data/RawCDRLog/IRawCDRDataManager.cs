using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
     public interface IRawCDRDataManager:IDataManager
    {
       Vanrise.Entities.BigResult<RawCDRLog> GetRawCDRData(Vanrise.Entities.DataRetrievalInput<RawCDRQuery> input);
    }
}
