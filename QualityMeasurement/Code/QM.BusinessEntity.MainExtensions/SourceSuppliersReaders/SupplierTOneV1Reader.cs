using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SourceSuppliersReaders
{
    public class SupplierTOneV1Reader : SourceSupplierReader
    {
        public override Guid ConfigId { get { return new Guid("15fbc0b8-806c-4642-8da8-580311e16576"); } }

        public string ConnectionString { get; set; }

        public override bool UseSourceItemId
        {
            get { return false; ; }
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
                SourceSupplier sourceSupplier = new SourceSupplier()
                {
                    SourceId = arg["ID"].ToString(),
                    Name = GetCarrierAccountName(arg["ProfileName"] as string, arg["NameSuffix"] as string)
                };
                return sourceSupplier;
            }

            const string query_getUpdatedSuppliers = @"SELECT
                ca.CarrierAccountID as ID,
                cp.Name as ProfileName,
                ca.NameSuffix as NameSuffix
                FROM CarrierAccount ca
                INNER JOIN CarrierProfile cp on ca.ProfileID = cp.ProfileID
                WHERE ca.IsDeleted = 'N' AND ca.ActivationStatus = 2 and (ca.AccountType = 1 or ca.AccountType = 2)";

            internal static string GetCarrierAccountName(string carrierName, string nameSuffix)
            {
                return string.Format("{0}{1}", carrierName, string.IsNullOrEmpty(nameSuffix) ? string.Empty : " (" + nameSuffix + ")");
            }
        }
    }
}
