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
    public class SaleCodeDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISaleCodeDataManager
    {

        #region ctor/Local Variables

        public SaleCodeDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public IEnumerable<SaleCode> GetFilteredSaleCodes(SaleCodeQuery query)
        {
            string zoneIdsAsString = null;
            if (query.ZonesIds != null && query.ZonesIds.Count() > 0)
                zoneIdsAsString = string.Join<long>(",", query.ZonesIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetFiltered", SaleCodeMapper, query.SellingNumberPlanId, zoneIdsAsString, query.Code, query.EffectiveOn, query.GetEffectiveAfter);
        }

        public IEnumerable<SaleCode> GetSaleCodesByCode(string codeNumber)
        {
            return GetItemsSP("TOneWhS_BE.[sp_SaleCode_GetByCode]", SaleCodeMapper, codeNumber);
        }
        public IEnumerable<SaleCode> GetSaleCodesByZone(SaleCodeQueryByZone query)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetFilteredByZone", SaleCodeMapper, query.ZoneId, query.EffectiveOn);
        }

        public IEnumerable<SaleCode> GetAllSaleCodes()
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetAll", SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByZoneId", SaleCodeMapper, zoneID, effectiveDate);
        }

        public List<SaleCode> GetSaleCodesByCodeGroups(List<int> codeGroupsIds)
        {
            string codegroupslist = null;
            if (codeGroupsIds != null && codeGroupsIds.Count() > 0)
                codegroupslist = string.Join<int>(",", codeGroupsIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByCodeGroupIds", SaleCodeMapper, codegroupslist);
        }

        public List<SaleCode> GetSaleCodesByCodeId(IEnumerable<long> codeIds)
        {
            string codeslist = null;
            if (codeIds != null && codeIds.Any())
                codeslist = string.Join(",", codeIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetCodeIds", SaleCodeMapper, codeslist);
        }

        public List<SaleCode> GetSaleCodesEffectiveByZoneID(long zoneID, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetEffectiveByZoneId", SaleCodeMapper, zoneID, effectiveDate);
        }

        public List<SaleCode> GetSaleCodes(DateTime effectiveOn)
        {
            return GetItemsSP("[TOneWhS_BE].[sp_SaleCode_GetByEffective]", SaleCodeMapper, effectiveOn);
        }
        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn, long? processInstanceId)
        {
            /*
             * If processInstanceId is null then we get all sale codes effective After
             * Else we get the sale Codes effectives at the time => processInstanceId<= param.processInstanceId and effective in param.effectiveOn
             * */
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetSaleCodesEffectiveAfter", SaleCodeMapper, sellingNumberPlanId, effectiveOn, processInstanceId);
        }
        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByCodePrefix", SaleCodeMapper, codePrefix, effectiveOn, isFuture, getChildCodes, getParentCodes);
        }

        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetDistinctCodePrefixes", CodePrefixMapper, prefixLength, effectiveOn, isFuture);
        }

        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            string _codePrefixes = null;
            if (codePrefixes != null && codePrefixes.Count() > 0)
                _codePrefixes = string.Join<string>(",", codePrefixes);

            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetSpecificCodePrefixes", CodePrefixMapper, prefixLength, _codePrefixes, effectiveOn, isFuture);
        }

        public bool AreZonesUpdated(ref object lastReceivedDataInfo)
        {
            return IsDataUpdated("TOneWhS_BE.SaleCode", ref lastReceivedDataInfo);
        }
        public List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByZoneName", SaleCodeMapper, sellingNumberPlanId, zoneName, effectiveDate);
        }
        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByDate", SaleCodeMapper, sellingNumberPlanId, countryId, minimumDate);
        }
        public List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate)
        {
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByCountry", SaleCodeMapper, countryId, effectiveDate);
        }

        public List<SaleCode> GetSaleCodesByZoneIDs(List<long> zoneIds, DateTime effectiveDate)
        {
            string allZoneIds = null;
            if (zoneIds != null && zoneIds.Count() > 0)
                allZoneIds = string.Join<long>(",", zoneIds);
            return GetItemsSP("TOneWhS_BE.sp_SaleCode_GetByZoneIds", SaleCodeMapper, allZoneIds, effectiveDate);
        }

        public bool AreSaleCodesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.SaleCode", ref updateHandle);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Mappers
        SaleCode SaleCodeMapper(IDataReader reader)
        {
            SaleCode saleCode = new SaleCode
            {
                SaleCodeId = (long)reader["ID"],
                Code = reader["Code"] as string,
                ZoneId = GetReaderValue<long>(reader, "ZoneID"),
                BED = GetReaderValue<DateTime>(reader, "BED"),
                EED = GetReaderValue<DateTime?>(reader, "EED"),
                CodeGroupId = GetReaderValue<int>(reader, "CodeGroupId"),
                SourceId = reader["SourceId"] as string
            };
            return saleCode;
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


        #region State Backup Methods

        public string BackupAllDataBySellingNumberingPlanId(long stateBackupId, string backupDatabase, int sellingNumberPlanId)
        {
            return String.Format(@"INSERT INTO [{0}].[TOneWhS_BE_Bkup].[SaleCode] WITH (TABLOCK)
                                                      ( [ID]
                                                       ,[Code]
                                                       ,[ZoneID]
                                                       ,[CodeGroupID]
                                                       ,[BED]
                                                       ,[EED]
                                                       ,[SourceID]
                                                       ,[StateBackupID]
                                                       ,[ProcessInstanceID])
                                                SELECT
                                                          sc.[ID]
                                                        , sc.[Code]
                                                        , sc.[ZoneID]
                                                        , sc.[CodeGroupID]
                                                        , sc.[BED]
                                                        , sc.[EED]
                                                        , sc.[SourceID]
                                                        , {1} AS StateBackupID 
                                                        , sc.[ProcessInstanceID] 
                                            FROM [TOneWhS_BE].[SaleCode]
                                            sc WITH (NOLOCK) Inner Join [TOneWhS_BE].SaleZone sz WITH (NOLOCK) on sc.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {2}", backupDatabase, stateBackupId, sellingNumberPlanId);
        }

        public string GetRestoreCommands(long stateBackupId, string backupDatabase)
        {
            return String.Format(@"INSERT INTO [TOneWhS_BE].[SaleCode] 
                                            ( [ID]
                                            , [Code]
                                            , [ZoneID]
                                            , [CodeGroupID]
                                            , [BED]
                                            , [EED]
                                            , [SourceID]
                                            , [ProcessInstanceID] )
                                                                                           
                                        SELECT 
                                              [ID]
                                            , [Code]
                                            , [ZoneID]
                                            , [CodeGroupID]
                                            , [BED]
                                            , [EED]
                                            , [SourceID]  
                                            , [ProcessInstanceID]
                                        FROM [{0}].[TOneWhS_BE_Bkup].[SaleCode]
                                        WITH (NOLOCK) Where StateBackupID = {1} ", backupDatabase, stateBackupId);
        }

        public string GetDeleteCommandsBySellingNumberPlanId(long sellingNumberPlanId)
        {
            return String.Format(@"DELETE sc FROM [TOneWhS_BE].[SaleCode] sc  Inner Join [TOneWhS_BE].SaleZone sz on sc.ZoneID = sz.ID
                                            Where sz.SellingNumberPlanID = {0}", sellingNumberPlanId);
        }

        #endregion

    }
}
