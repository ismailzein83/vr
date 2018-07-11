using Demo.Module.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Demo.Module.Data
{
    public interface ICDRDataManager : IDataManager
    {
        List<CDR> GetCDR(DataRetrievalInput<CDRQuery> input);       

    }
}
