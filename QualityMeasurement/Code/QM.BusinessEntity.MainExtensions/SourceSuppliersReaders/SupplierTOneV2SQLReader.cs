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
            TOneV2DataManager dataManager = new TOneV2DataManager(this.ConnectionString);
            return dataManager.GetUpdatedSuppliers(ref updatedHandle);
        }

        private class TOneV2DataManager : BaseTOneDataManager
        {
            public TOneV2DataManager(string connectionString)
                : base(connectionString)
            { 
            }

            const string query_getUpdatedSuppliers = @"SELECT
			                                                    ca.ID,
			                                                    ca.Name
	                                                    FROM TOneWhS_BE.CarrierAccount ca";

            protected override string GetQuery()
            {
                return query_getUpdatedSuppliers;
            }
        }
    }
}
