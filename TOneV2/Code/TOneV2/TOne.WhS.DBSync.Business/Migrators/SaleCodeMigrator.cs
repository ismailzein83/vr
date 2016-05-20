using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SaleCodeMigrator : Migrator<SourceCode, SaleCode>
    {
        SaleCodeDBSyncDataManager dbSyncDataManager;
        SourceCodeDataManager dataManager;

        public SaleCodeMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new SaleCodeDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<SaleCode> itemsToAdd)
        {
            long startingId;
            ReserveIDRange(itemsToAdd.Count(), out startingId);
            dbSyncDataManager.ApplySaleCodesToTemp(itemsToAdd, startingId);
            DBTable dbTableSaleCode = Context.DBTables[DBTableName.SaleCode];
            if (dbTableSaleCode != null)
                dbTableSaleCode.Records = dbSyncDataManager.GetSaleCodes();
        }

        public override IEnumerable<SourceCode> GetSourceItems()
        {
            return dataManager.GetSourceCodes(false);
        }

        public override SaleCode BuildItemFromSource(SourceCode sourceItem)
        {
            DBTable dbTableSaleZone = Context.DBTables[DBTableName.SaleZone];
            DBTable dbTableCountry = Context.DBTables[DBTableName.Country];
            DBTable dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];

            if (dbTableCountry != null && dbTableSaleZone != null && dbTableCodeGroup != null)
            {
                Dictionary<string, SaleZone> allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZone.Records;
                Dictionary<string, Country> allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
                Dictionary<string, CodeGroup> allCodeGroups = (Dictionary<string, CodeGroup>)dbTableCodeGroup.Records;

                SaleZone saleZone = null;
                if (allSaleZones != null)
                    allSaleZones.TryGetValue(sourceItem.ZoneId.ToString(), out saleZone);

                Country country = null;
                if (allCountries != null)
                    allCountries.TryGetValue(sourceItem.CodeGroup, out country);

                CodeGroup codeGroup = null;
                if (allCodeGroups != null & country != null)
                    allCodeGroups.Values.Where(x => x.SourceId == sourceItem.CodeGroup);


                if (codeGroup != null & saleZone != null)
                    return new SaleCode
                        {
                            Code = sourceItem.Code,
                            BED = sourceItem.BeginEffectiveDate,
                            CodeGroupId = codeGroup.CodeGroupId,
                            EED = sourceItem.EndEffectiveDate,
                            ZoneId = saleZone.SaleZoneId,
                            SourceId = sourceItem.SourceId
                        };
                else
                    return null;

            }
            else
                return null;
        }

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            IDManager.Instance.ReserveIDRange(typeof(SaleCodeManager), nbOfIds, out startingId);
        }
    }
}
