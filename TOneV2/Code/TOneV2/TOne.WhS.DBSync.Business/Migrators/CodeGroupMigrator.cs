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
        Dictionary<string, CodeGroup> allCodeGroups;
        Dictionary<string, CodeGroup> _codesByValue = new Dictionary<string, CodeGroup>();
        int _minLength = int.MaxValue;
        int _maxLength = 0;

        public CodeGroupMigrator(MigrationContext context)
            : base(context)
        {
            dbSyncDataManager = new CodeGroupDBSyncDataManager(Context.UseTempTables);
            dataManager = new SourceCodeGroupDataManager(Context.ConnectionString);
            TableName = dbSyncDataManager.GetTableName();
            var dbTableCountry = Context.DBTables[DBTableName.Country];
            allCountries = (Dictionary<string, Country>)dbTableCountry.Records;
            var dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];
            allCodeGroups = (Dictionary<string, CodeGroup>)dbTableCodeGroup.Records;
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
                                Code = sourceItem.Code,
                                CountryId = country.CountryId,
                                SourceId = sourceItem.SourceId
                            };
            else
            {
                TotalRowsFailed++;
                return null;
            }
        }

        public override void FillTableInfo(bool useTempTables)
        {
            DBTable dbTableCodeGroup = Context.DBTables[DBTableName.CodeGroup];
            if (dbTableCodeGroup != null)
                dbTableCodeGroup.Records = dbSyncDataManager.GetCodeGroups(useTempTables);
        }

        public CodeGroup GetMatchCodeGroup(string itemCode)
        {
            IEnumerable<CodeGroup> codeObjects = allCodeGroups.Values;
            foreach (var codeObj in codeObjects)
            {
                var codes = this.GetCodes(codeObj);
                if (codes != null)
                {
                    foreach (var code in codes)
                    {
                        if (!_codesByValue.ContainsKey(code))
                        {
                            int codeLength = code.Length;
                            if (codeLength < _minLength)
                                _minLength = codeLength;
                            if (codeLength > _maxLength)
                                _maxLength = codeLength;
                            _codesByValue.Add(code, codeObj);
                        }
                    }
                }
            }

            return this.GetLongestMatch(itemCode);
        }


        private List<string> GetCodes(CodeGroup codeObject)
        {
            if (!String.IsNullOrEmpty(codeObject.Code))
                return new List<string> { codeObject.Code };
            else
                return null;
        }

        private CodeGroup GetLongestMatch(string phoneNumber)
        {
            if (phoneNumber == null)
                return default(CodeGroup);

            string prefix = phoneNumber.Substring(0, Math.Min(_maxLength, phoneNumber.Length));
            while (prefix.Length >= _minLength)
            {
                CodeGroup matchCode;
                if (_codesByValue.TryGetValue(prefix, out matchCode))
                    return matchCode;
                prefix = prefix.Substring(0, prefix.Length - 1);
            }
            return default(CodeGroup);
        }

    }
}
