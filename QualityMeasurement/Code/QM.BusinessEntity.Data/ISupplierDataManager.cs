using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Data
{
    public interface ISupplierDataManager : IDataManager
    {
        bool AreSuppliersUpdated(ref object _updateHandle);

        bool Insert(Supplier supplier);

        bool Update(Supplier supplier);
    }
}
