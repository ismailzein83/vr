using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class SupplierCodeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISupplierCodeDataManager
    {

        #region ctor/Local Variables
        public SupplierCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }

        #endregion

        #region Public Methods
        public List<SupplierCode> GetSupplierCodesEffectiveAfter(int supplierId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetByDate", SupplierCodeMapper, supplierId, minimumDate);
        }
        public List<SupplierCode> GetSupplierCodes(DateTime from, DateTime to)
        {
            return GetItemsSP("[TOneWhS_BE].[sp_SupplierCode_GetByEffective]", SupplierCodeMapper, from, to);
        }
        public IEnumerable<SupplierCode> GetSupplierCodesByCode(string codeNumber)
        {
            return GetItemsSP("TOneWhS_BE.[sp_SupplierCode_GetByCode]", SupplierCodeMapper, codeNumber);
        }
        
        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetDistinctCodePrefixes", CodePrefixMapper, prefixLength, effectiveOn, isFuture);
        }
        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture) 
        {
            string _codePrefixes = null;
            if (codePrefixes != null && codePrefixes.Count() > 0)
                _codePrefixes = string.Join<string>(",", codePrefixes);

            return GetItemsSP("TOneWhS_BE.sp_SupplierCode_GetSpecificCodePrefixes", CodePrefixMapper, prefixLength, _codePrefixes, effectiveOn, isFuture);    
        }


        public IEnumerable<SupplierCode> GetFilteredSupplierCodes(SupplierCodeQuery query)
        {
            string zoneIds = null;
            if (query.ZoneIds != null && query.ZoneIds.Count() > 0)
                zoneIds = string.Join<int>(",", query.ZoneIds);

            return GetItemsSP("[TOneWhS_BE].[sp_SupplierCode_GetFiltered]", SupplierCodeMapper, query.Code, query.SupplierId, zoneIds, query.EffectiveOn);
        }

        public bool AreSupplierCodesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SupplierCode", ref updateHandle);
        }

        #endregion

        #region Private Methods
        #endregion

        #region Mappers
        SupplierCode SupplierCodeMapper(IDataReader reader)
        {
            SupplierCode supplierCode = new SupplierCode
            {
                Code = GetReaderValue<string>(reader, "Code"),
                SupplierCodeId = GetReaderValue<long>(reader, "ID"),
                ZoneId = (long)reader["ZoneID"],
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                CodeGroupId = GetReaderValue<int>(reader, "CodeGroupId"),
                SourceId = reader["SourceId"] as string

            };
            return supplierCode;
        }
        CodePrefixInfo CodePrefixMapper(IDataReader reader)
        {
            return new CodePrefixInfo()
            {
                CodePrefix = reader["CodePrefix"] as string,
                Count = GetReaderValue<int>(reader, "codeCount")
            };
        }

        #endregion


        public void LoadSupplierCodes(IEnumerable<RoutingSupplierInfo> activeSupplierInfo, string codePrefix, DateTime? effectiveOn, bool isFuture, Func<bool> shouldStop, Action<SupplierCode> onCodeLoaded)
        {
            DataTable dtActiveSuppliers = CarrierAccountDataManager.BuildRoutingSupplierInfoTable(activeSupplierInfo);
            ExecuteReaderSPCmd("[TOneWhS_BE].[sp_SupplierCode_GetActiveCodesBySuppliers]",
                (reader) =>
                {
                    while(reader.Read())
                    {
                        if (shouldStop != null && shouldStop())
                            break;

                        onCodeLoaded(SupplierCodeMapper(reader));
                    }
                },
                (cmd) =>
                {
                    var dtPrm = new SqlParameter("@ActiveSuppliersInfo", SqlDbType.Structured);
                    dtPrm.Value = dtActiveSuppliers;
                    cmd.Parameters.Add(dtPrm);

                    cmd.Parameters.Add(new SqlParameter("@CodePrefix", codePrefix));
                    cmd.Parameters.Add(new SqlParameter("@EffectiveOn", effectiveOn));
                    cmd.Parameters.Add(new SqlParameter("@IsFuture", isFuture));
                });
        }

        #region State Backup Methods

        public string BackupAllDataBySupplierId(long stateBackupId, string backupDatabase, int supplierId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SupplierCode] WITH (TABLOCK)
                                            SELECT sc.[ID], sc.[Code], sc.[ZoneID], sc.[CodeGroupID], sc.[BED], sc.[EED], sc.[SourceID],  {1} AS StateBackupID  FROM [TOneWhS_BE].[SupplierCode] sc
                                            WITH (NOLOCK)  Inner Join [TOneWhS_BE].[SupplierZone] sz  WITH (NOLOCK)  on sz.ID = sc.ZoneID
                                            Where sz.SupplierID = {2}", backupDatabase, stateBackupId, supplierId);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SupplierCode] ([ID], [Code], [ZoneID], [CodeGroupID], [BED], [EED], [SourceID])
                                            SELECT [ID], [Code], [ZoneID], [CodeGroupID], [BED], [EED], [SourceID] FROM [{0}].[TOneWhS_BE_Bkup].[SupplierCode]
                                            WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }

        public string GetDeleteCommandsBySupplierId(int supplierId)
        {
            return String.Format(@"DELETE sc FROM [TOneWhS_BE].[SupplierCode] sc Inner Join [TOneWhS_BE].[SupplierZone] sz on sz.ID = sc.ZoneID
                                            Where sz.SupplierID = {0}", supplierId);
        }

        #endregion
    
    }
}
