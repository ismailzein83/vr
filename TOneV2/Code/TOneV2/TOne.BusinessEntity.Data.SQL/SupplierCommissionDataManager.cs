using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Data.SQL;

namespace TOne.BusinessEntity.Data.SQL
{
    public class SupplierCommissionDataManager : BaseTOneDataManager, ISupplierCommissionDataManager
    {
       public Vanrise.Entities.BigResult<string> GetSupplierCommissions(Vanrise.Entities.DataRetrievalInput<string> input)
       {
           Vanrise.Entities.BigResult<string> data = new Vanrise.Entities.BigResult<string>();
           return data;
       }
    }
}
