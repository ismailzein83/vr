using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SourceSuppliersReaders
{
    public class SupplierTOneV2SQLReader : SourceSupplierReader 
    {
        public string ConnectionString { get; set; }

        public override bool UseSourceItemId
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<SourceSupplier> GetChangedItems(ref object updatedHandle)
        {
            DataManager dataManager = new DataManager(this.ConnectionString);
            return dataManager.GetUpdatedSuppliers(ref updatedHandle);
        }

        private class DataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
            public DataManager(string connectionString)
                : base(connectionString, false)
            { 
            }

            public List<SourceSupplier> GetUpdatedSuppliers(ref object updateHandle)
            {
                return GetItemsText(query_getUpdatedSuppliers, SourceSupplierMapper, null);
            }

            private SourceSupplier SourceSupplierMapper(System.Data.IDataReader arg)
            {
                throw new NotImplementedException();
            }

            const string query_getUpdatedSuppliers = @"SELECT
			ca.ID,
			ca.Name
	FROM TOneWhS_BE.CarrierAccount ca   ";
        }
    }
}
