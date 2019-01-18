using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.RDB;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public class CodeGroupDataManager : ICodeGroupDataManager
    {
        #region RDB

        static string TABLE_NAME = "TOneWhS_BE_CodeGroup";
        static string TABLE_ALIAS = "cg";

        const string COL_ID = "ID";
        const string COL_CountryID = "CountryID";
        const string COL_Name = "Name";
        internal const string COL_Code = "Code";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_SourceID = "SourceID";
        const string COL_LastModifiedTime = "LastModifiedTime";


        static CodeGroupDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CountryID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 200 });
            columns.Add(COL_Code, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 20 });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_SourceID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_BE",
                DBTableName = "CodeGroup",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_BE", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        #endregion

        #region ICodeGroupDataManager Members

        public List<CodeGroup> GetCodeGroups()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            return queryContext.GetItems(CodeGroupMapper);
        }

        public bool Update(CodeGroupToEdit codeGroup)
        {
            codeGroup.Code = codeGroup.Code.Trim();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            var notExistsCondition = updateQuery.IfNotExists(TABLE_ALIAS);
            notExistsCondition.NotEqualsCondition(COL_ID).Value(codeGroup.CodeGroupId);
            notExistsCondition.EqualsCondition(COL_Code).Value(codeGroup.Code);

            updateQuery.Column(COL_Code).Value(codeGroup.Code);
            updateQuery.Column(COL_CountryID).Value(codeGroup.CountryId);
            updateQuery.Column(COL_Name).Value(codeGroup.Name);

            updateQuery.Where().EqualsCondition(COL_ID).Value(codeGroup.CodeGroupId);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public bool Insert(CodeGroup codeGroup, out int insertedId)
        {
            codeGroup.Code = codeGroup.Code.Trim();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            var notExistsCondition = insertQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.OR);
            notExistsCondition.EqualsCondition(COL_Code).Value(codeGroup.Code);
            notExistsCondition.EqualsCondition(COL_Name).Value(codeGroup.Name);

            insertQuery.Column(COL_Code).Value(codeGroup.Code);
            insertQuery.Column(COL_Name).Value(codeGroup.Name);
            insertQuery.Column(COL_CountryID).Value(codeGroup.CountryId);

            var returnedValue = queryContext.ExecuteScalar().NullableIntValue;
            if (returnedValue.HasValue)
            {
                insertedId = returnedValue.Value;
                return true;
            }
            else
            {
                insertedId = 0;
                return false;
            }
        }

        public void SaveCodeGroupToDB(List<CodeGroup> codeGroups)
        {
            if (codeGroups == null || !codeGroups.Any())
                return;

            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertMultipleRowsContext = queryContext.AddInsertMultipleRowsQuery();
            insertMultipleRowsContext.IntoTable(TABLE_NAME);

            foreach (CodeGroup codeGroup in codeGroups)
            {
                codeGroup.Code = codeGroup.Code.Trim();
                var rowContext = insertMultipleRowsContext.AddRow();
                rowContext.Column(COL_Code).Value(codeGroup.Code);
                rowContext.Column(COL_Name).Value(codeGroup.Name);
                rowContext.Column(COL_CountryID).Value(codeGroup.CountryId);
            }

            queryContext.ExecuteNonQuery();
        }

        public bool AreCodeGroupUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public bool CheckIfCodeGroupHasRelatedCodes(int codeGroupId)
        {
            SaleCodeDataManager saleCodeDataManager = new SaleCodeDataManager();
            var saleCode = saleCodeDataManager.GetSaleCodeByCodeGroup(codeGroupId);

            if (saleCode != null)
                return true;

            SupplierCodeDataManager supplierCodeDataManager = new SupplierCodeDataManager();
            var supplierCode = supplierCodeDataManager.GetSupplierCodeByCodeGroup(codeGroupId);

            return supplierCode != null;
        }

        public void JoinCodeGroup(RDBJoinContext joinContext, string codeTableAlias, string originalTableAlias, string originalTableCodeIdCol)
        {
            var joinStatement = joinContext.Join(TABLE_NAME, codeTableAlias);
            joinStatement.JoinType(RDBJoinType.Inner);
            var joinCondition = joinStatement.On();
            joinCondition.EqualsCondition(originalTableAlias, originalTableCodeIdCol, codeTableAlias, COL_ID);
        }
        #endregion

        #region Mappers
        private CodeGroup CodeGroupMapper(IRDBDataReader reader)
        {
            CodeGroup codeGroup = new CodeGroup
            {
                CodeGroupId = reader.GetInt(COL_ID),
                CountryId = reader.GetInt(COL_CountryID),
                Name = reader.GetString(COL_Name),
                Code = reader.GetString(COL_Code).Trim()
            };

            return codeGroup;
        }
        #endregion
    }
}
