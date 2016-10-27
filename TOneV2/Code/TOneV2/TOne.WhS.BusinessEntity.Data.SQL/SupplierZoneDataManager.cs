using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierZoneDataManager : BaseSQLDataManager, ISupplierZoneDataManager
    {

        #region ctor/Local Variables
        public SupplierZoneDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetBySupplierId", SupplierZoneMapper, supplierId, effectiveDate);
        }

        public List<SupplierZone> GetSupplierZonesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetByDate", SupplierZoneMapper, supplierId, minimumDate);
        }
        public List<SupplierZone> GetSupplierZones()
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierZone_GetAll", SupplierZoneMapper);
        }
        public bool AreSupplierZonesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierZone", ref updateHandle);
        }
        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        SupplierZone SupplierZoneMapper(IDataReader reader)
        {
            SupplierZone supplierZone = new SupplierZone
            {
                SupplierId = (int)reader["SupplierID"],
                CountryId = (int)reader["CountryID"],
                SupplierZoneId = (long)reader["ID"],
                Name = reader["Name"] as string,
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                SourceId = reader["SourceID"] as string
            };
            return supplierZone;
        }
       
        #endregion


        #region State Backup Methods

        public string BackupAllDataBySupplierId(long stateBackupId, string backupDatabase, int supplierId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SupplierZone] WITH (TABLOCK)
                                            SELECT [ID] ,[CountryID], [Name], [SupplierID], [BED], [EED], [SourceID],  {1} AS StateBackupID FROM [TOneWhS_BE].[SupplierZone]
                                            WITH (NOLOCK) Where SupplierID = {2}", backupDatabase, stateBackupId, supplierId);
        }


        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SupplierZone] ([ID], [CountryID], [Name], [SupplierID], [BED], [EED],[SourceID])
                                            SELECT [ID], [CountryID], [Name], [SupplierID], [BED], [EED],[SourceID] FROM [{0}].[TOneWhS_BE_Bkup].[SupplierZone]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }

        public string GetDeleteCommandsBySupplierId(int supplierId)
        {
            return String.Format(@"DELETE FROM [TOneWhS_BE].[SupplierZone] Where SupplierID = {0}", supplierId);
        }

        #endregion

    }
}
