using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class CodeGroupMigrator : Migrator<SourceCodeGroup, CodeGroup>
    {
        CodeGroupDBSyncDataManager dbSyncDataManager;
        SourceCodeGroupDataManager dataManager;
        Dictionary<string, Country> allCountries;

        public CodeGroupMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CodeGroupDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeGroupDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCountry = Context.DBTables[DBTableName.Country];
            allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
        }

        public override void Migrate(MigrationInfoContext context)
        {
            base.Migrate(context);
        }

        public override void AddItems(List<CodeGroup> itemsToAdd)
        {
            dbSyncDataManager.ApplyCodeGroupsToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceCodeGroup> GetSourceItems()
        {
            return dataManager.GetSourceCodeGroups();
        }

        public override CodeGroup BuildItemFromSource(SourceCodeGroup sourceItem)
        {

            Country country = null;
            if (allCountries != null)
                allCountries.TryGetValue(CountryMigrator.ConvertCountryNameToSourceId(sourceItem.Name), out country);
            if (country != null)
                return new CodeGroup
                            {
                                Code = sourceItem.Code.Trim(),
                                CountryId = country.CountryId,
                                SourceId = sourceItem.SourceId,
                                Name = sourceItem.Name
                            };
            else
            {
                TotalRowsFailed++;
                Context.WriteWarning(string.Format("Failed migrating Code Group {0}, Source Id: {1}", sourceItem.Code, sourceItem.SourceId));
                return null;
            }
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];
            if (dbTableCodeGroup != null)
                dbTableCodeGroup.Records = dbSyncDataManager.GetCodeGroups(useTempTables);
        }

    }
}
