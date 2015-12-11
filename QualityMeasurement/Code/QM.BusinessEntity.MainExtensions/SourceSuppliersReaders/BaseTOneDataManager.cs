using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SourceSuppliersReaders
{
    public abstract class BaseTOneDataManager : Vanrise.Data.SQL.BaseSQLDataManager
    {
        public BaseTOneDataManager(string connectionString)
                : base(connectionString, false)
            { 
            }

            public List<SourceSupplier> GetUpdatedSuppliers(ref object updateHandle)
            {
                return GetItemsText(GetQuery(), SourceSupplierMapper, null);
            }

            private SourceSupplier SourceSupplierMapper(System.Data.IDataReader arg)
            {
                throw new NotImplementedException();
            }

            protected abstract string GetQuery();
    }
}
