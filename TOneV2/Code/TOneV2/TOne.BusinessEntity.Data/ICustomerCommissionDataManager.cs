using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Data
{
   public interface ICustomerCommissionDataManager:IDataManager
    {
       Vanrise.Entities.BigResult<string> GetCustomerCommissions(Vanrise.Entities.DataRetrievalInput<string> input);
    }
}
