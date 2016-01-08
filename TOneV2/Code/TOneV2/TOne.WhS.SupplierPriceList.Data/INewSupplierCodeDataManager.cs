using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.Data;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface INewSupplierCodeDataManager : IDataManager, IBulkApplyDataManager<NewCode>
    {
        long ProcessInstanceId { set; }

        void ApplyNewCodesToDB(object preparedCodes);
    }
}
