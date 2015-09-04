using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Data
{
    public interface ISupplierCommissionDataManager:IDataManager
    {
        Vanrise.Entities.BigResult<string> GetSupplierCommissions(Vanrise.Entities.DataRetrievalInput<string> input);
    }
}
