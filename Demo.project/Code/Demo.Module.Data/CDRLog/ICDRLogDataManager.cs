using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;

namespace Demo.Module.Data
{
    public interface ICDRLogDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<CDRLog> GetCDRLogData(Vanrise.Entities.DataRetrievalInput<CDRQuery> input);
    }
}
