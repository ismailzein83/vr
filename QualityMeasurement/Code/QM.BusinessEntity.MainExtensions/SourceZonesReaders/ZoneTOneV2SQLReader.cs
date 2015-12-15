using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SourceZonesReaders
{
    public class ZoneTOneV2SQLReader : SourceZoneReader 
    {
        public string ConnectionString { get; set; }

        public override bool UseSourceItemId
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<SourceZone> GetChangedItems(ref object updatedHandle)
        {
            throw new NotImplementedException();
        }

        private class DataManager : Vanrise.Data.SQL.BaseSQLDataManager
        {
//            public DataManager(string connectionString)
//                : base(connectionString, false)
//            { 
//            }

//            public List<SourceSupplier> GetUpdatedSuppliers(ref object updateHandle)
//            {
//                return GetItemsText(query_getUpdatedSuppliers, SourceSupplierMapper, null);
//            }

//            private SourceSupplier SourceSupplierMapper(System.Data.IDataReader arg)
//            {
//                SourceSupplier sourceSupplier = new SourceSupplier()
//                {
//                    SourceId = arg["ID"].ToString(),
//                    Name = arg["Name"] as string
//                };
//                return sourceSupplier;
//            }

//            const string query_getUpdatedSuppliers = @"SELECT
//	        ca.ID,
//	        ca.Name
//	        FROM TOneWhS_BE.CarrierAccount ca  
//	        where ca.AccountType = 1 or ca.AccountType = 2";
        }
    }
}
