using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class SaleCodeDataManager : ISaleCodeDataManager
    {
        #region RDB

        static string TABLE_NAME = "TOneWhS_BE_SaleCode";
        static string TABLE_ALIAS = "sc";
        const string COL_ID = "ID";
        const string COL_Code = "Code";
        const string COL_ZoneID = "ZoneID";
        const string COL_CodeGroupID = "CodeGroupID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_SourceID = "SourceID";
        const string COL_ProcessInstanceID = "ProcessInstanceID";



        static SaleCodeDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>
            {
                {COL_ID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_Code, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 20}},
                {COL_ZoneID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}},
                {COL_CodeGroupID, new RDBTableColumnDefinition {DataType = RDBDataType.Int}},
                {COL_BED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_EED, new RDBTableColumnDefinition {DataType = RDBDataType.DateTime}},
                {COL_SourceID, new RDBTableColumnDefinition {DataType = RDBDataType.Varchar, Size = 50}},
                {COL_ProcessInstanceID, new RDBTableColumnDefinition {DataType = RDBDataType.BigInt}}
            };
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "SaleCode",
                Columns = columns
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE_SaleCode", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }


        #endregion

        #region ISaleCodeDataManager Members
        public IEnumerable<SaleCode> GetSaleCodesByCode(string codeNumber)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.StartsWithCondition(COL_Code, codeNumber);
            AddEffectiveOnDateCondition(where, DateTime.Now);
            selectQuery.Sort().ByColumn(COL_Code, RDBSortDirection.ASC);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public IEnumerable<SaleCode> GetAllSaleCodes()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public IEnumerable<SaleCode> GetFilteredSaleCodes(SaleCodeQuery query) // to Test again
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, "sz", TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();

            if (query.GetEffectiveAfter)
                AddEffectiveAfterDateCondition(where, query.EffectiveOn);
            else
                AddEffectiveOnDateCondition(where, query.EffectiveOn);

            if (query.Code != null)
                where.StartsWithCondition(COL_Code, query.Code);

            if (query.SellingNumberPlanId.HasValue)
                where.EqualsCondition("sz", SaleZoneDataManager.COL_SellingNumberPlanID).Value(query.SellingNumberPlanId.Value);

            if (query.ZonesIds != null && query.ZonesIds.Any())
                where.ListCondition(COL_ZoneID, RDBListConditionOperator.IN, query.ZonesIds);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public IEnumerable<SaleCode> GetSaleCodesByZone(SaleCodeQueryByZone query)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ZoneID).Value(query.ZoneId);

            if (query.EffectiveOn.HasValue)
                AddEffectiveOnDateCondition(where, query.EffectiveOn.Value);
            else
                where.FalseCondition();

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByZoneID(long zoneID, DateTime effectiveDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ZoneID).Value(zoneID);

            AddEffectiveOnDateCondition(where, effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public IEnumerable<SaleCode> GetParentsByPlan(int sellingNumberPlan, string codeNumber)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, "sz", TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            where.StartsWithCondition(COL_Code, codeNumber);
            where.EqualsCondition("sz", SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlan);

            AddEffectiveOnDateCondition(where, DateTime.Today);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByCodeGroups(List<int> codeGroupsIds) //To test
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.ListCondition(COL_CodeGroupID, RDBListConditionOperator.IN, codeGroupsIds);

            return queryContext.GetItems(SaleCodeMapper);

        }

        public List<SaleCode> GetSaleCodesByCodeId(IEnumerable<long> codeIds)  //To Test.. Changed but always same result
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.ListCondition(COL_Code, RDBListConditionOperator.IN, codeIds);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesEffectiveByZoneID(long zoneID, DateTime effectiveDate)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            where.EqualsCondition(COL_ZoneID).Value(zoneID);
            AddEffectiveOnDateCondition(where, effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodes(DateTime effectiveOn)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var where = selectQuery.Where();
            AddEffectiveOnDateCondition(where, effectiveOn);

            return queryContext.GetItems(SaleCodeMapper);
        }



        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, DateTime effectiveOn, long? processInstanceId)
        {
            var saleZoneDataManager = new SaleZoneDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, "sz", TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            where.EqualsCondition("sz", SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);

            AddEffectiveAfterDateCondition(where, effectiveOn);

            if (processInstanceId != null)
            {
                var processCondition = where.ChildConditionGroup(RDBConditionGroupOperator.OR);
                processCondition.NullCondition(TABLE_ALIAS, COL_ProcessInstanceID);
                processCondition.LessThanCondition(TABLE_ALIAS, COL_ProcessInstanceID).Value(processInstanceId.Value);
            }
            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByCountry(int countryId, DateTime effectiveDate)
        {
            var saleZoneDataManager = new SaleZoneDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, "sz", TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            where.EqualsCondition("sz", SaleZoneDataManager.COL_CountryID).Value(countryId);

            AddEffectiveOnDateCondition(where, effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);

        }

        public List<SaleCode> GetSaleCodesByPrefix(string codePrefix, DateTime? effectiveOn, bool isFuture, bool getChildCodes, bool getParentCodes) //to test // need revising: and or to if
        {
            var saleZoneDataManager = new SaleZoneDataManager();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, "sz", TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            var codeCondition = where.ChildConditionGroup(RDBConditionGroupOperator.OR);

            var childCodesCondition = codeCondition.ChildConditionGroup(RDBConditionGroupOperator.AND);
            childCodesCondition.CompareCondition(RDBCompareConditionOperator.Eq).Expression1().Value(getChildCodes);
            childCodesCondition.CompareCondition(RDBCompareConditionOperator.Eq).Expression2().Value(1);
            childCodesCondition.StartsWithCondition(COL_Code, codePrefix);

            var parentCodesCondition = codeCondition.ChildConditionGroup(RDBConditionGroupOperator.AND);
            //  parentCodesCondition.CompareCondition(fieldDatePartExpression, RDBCompareConditionOperator.GEq, startDateExpression);
            parentCodesCondition.StartsWithCondition(codePrefix, COL_Code);

            if (isFuture == false && effectiveOn.HasValue) // in SQL effectiveOn check doesnt exist
                AddEffectiveOnDateCondition(where, effectiveOn.Value);
            if (isFuture == true)
                AddEffectiveAfterDateCondition(where, DateTime.Today);


            return queryContext.GetItems(SaleCodeMapper);

        }

        public IEnumerable<CodePrefixInfo> GetDistinctCodeByPrefixes(int prefixLength, DateTime? effectiveOn, bool isFuture)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CodePrefixInfo> GetSpecificCodeByPrefixes(int prefixLength, IEnumerable<string> codePrefixes, DateTime? effectiveOn, bool isFuture)
        {
            throw new NotImplementedException();
        }

        public List<SaleCode> GetSaleCodesByZoneName(int sellingNumberPlanId, string zoneName, DateTime effectiveDate)
        {
            var saleZoneDataManager = new SaleZoneDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, "sz", TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();
            where.EqualsCondition("sz", SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);
            where.EqualsCondition("sz", SaleZoneDataManager.COL_Name).Value(zoneName);

            AddEffectiveOnDateCondition(where, effectiveDate);

            return queryContext.GetItems(SaleCodeMapper);
        }

        public bool AreZonesUpdated(ref object updateHandle) //TBR
        {
            throw new NotImplementedException();
        }

        public List<SaleCode> GetSaleCodesEffectiveAfter(int sellingNumberPlanId, int countryId, DateTime minimumDate)
        {
            SaleZoneDataManager saleZoneDataManager = new SaleZoneDataManager();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);

            var join = selectQuery.Join();
            saleZoneDataManager.JoinSaleZone(join, "sz", TABLE_ALIAS, COL_ZoneID);

            var where = selectQuery.Where();

            AddEffectiveAfterDateCondition(where, minimumDate);

            where.EqualsCondition("sz", SaleZoneDataManager.COL_SellingNumberPlanID).Value(sellingNumberPlanId);
            where.EqualsCondition("sz", SaleZoneDataManager.COL_CountryID).Value(countryId);



            return queryContext.GetItems(SaleCodeMapper);
        }

        public List<SaleCode> GetSaleCodesByZoneIDs(List<long> zoneIds, DateTime effectiveDate)
        {
            throw new NotImplementedException();
        }

        public bool AreSaleCodesUpdated(ref object updateHandle)//TBR
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods
        private void AddEffectiveOnDateCondition(RDBConditionContext context, DateTime effectiveOn)
        {
            context.LessOrEqualCondition(TABLE_ALIAS, COL_BED).Value(effectiveOn);

            var orCondition = context.ChildConditionGroup(RDBConditionGroupOperator.OR);
            orCondition.NullCondition(TABLE_ALIAS, COL_EED);
            orCondition.GreaterThanCondition(TABLE_ALIAS, COL_EED).Value(effectiveOn);
        }

        private void AddEffectiveAfterDateCondition(RDBConditionContext context, DateTime effectiveAfter)
        {
            var effectiveCondition = context.ChildConditionGroup(RDBConditionGroupOperator.OR);
            effectiveCondition.NullCondition(TABLE_ALIAS, COL_EED);
            var andCondition = effectiveCondition.ChildConditionGroup();
            andCondition.GreaterThanCondition(TABLE_ALIAS, COL_EED).Value(effectiveAfter);
            andCondition.NotEqualsCondition(TABLE_ALIAS, COL_EED).Column(COL_BED);
        }
        #endregion

        #region Mappers
        SaleCode SaleCodeMapper(IRDBDataReader reader)
        {
            SaleCode saleCode = new SaleCode
            {
                SaleCodeId = reader.GetLong(COL_ID),
                Code = reader.GetString(COL_Code),
                ZoneId = reader.GetLongWithNullHandling(COL_ZoneID),
                BED = reader.GetDateTimeWithNullHandling(COL_BED),
                EED = reader.GetDateTimeWithNullHandling(COL_EED),
                CodeGroupId = reader.GetIntWithNullHandling(COL_CodeGroupID),
                SourceId = reader.GetString(COL_SourceID)
            };
            return saleCode;
        }

        //CodePrefixInfo CodePrefixMapper(IDataReader reader)
        //{
        //    return new CodePrefixInfo()
        //    {
        //        CodePrefix = reader["CodePrefix"] as string,
        //        Count = GetReaderValue<int>(reader, "codeCount")
        //    };
        //}

        #endregion
    }
}
