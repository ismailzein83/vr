using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.Common;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class SaleRateMigrator : Migrator<SourceCodeGroup, CodeGroup>
    {
        CodeGroupDBSyncDataManager dbSyncDataManager;
        SourceCodeGroupDataManager dataManager;

        public SaleRateMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CodeGroupDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeGroupDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
        }

        public override void Migrate()
        {
            base.Migrate();
        }

        public override void AddItems(List<CodeGroup> itemsToAdd)
        {
            dbSyncDataManager.ApplyCodeGroupsToTemp(itemsToAdd);
            DBTable dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];
            if (dbTableCodeGroup != null)
                dbTableCodeGroup.Records = dbSyncDataManager.GetCodeGroups();
        }

        public override IEnumerable<SourceCodeGroup> GetSourceItems()
        {
            return dataManager.GetSourceCodeGroups();
        }

        public override CodeGroup BuildItemFromSource(SourceCodeGroup sourceItem)
        {
            DBTable dbTableCountry = Context.DBTables[DBTableName.Country];
            if (dbTableCountry != null)
            {
                Dictionary<string, Country> allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
                Country country = null;
                if (allCountries != null)
                    allCountries.TryGetValue(sourceItem.SourceId, out country);
                if (country != null)
                    return new CodeGroup
                    {
                        Code = sourceItem.Code,
                        CountryId = country.CountryId,
                        SourceId = sourceItem.SourceId
                    };
                else
                    return null;

            }
            else
                return null;
        }

    }
}
