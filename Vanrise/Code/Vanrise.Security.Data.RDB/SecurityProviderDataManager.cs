using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Security.Entities;
using Vanrise.Entities;
namespace Vanrise.Security.Data.RDB
{
    public class SecurityProviderDataManager : ISecurityProviderDataManager
    {
        #region RDB

        static string TABLE_NAME = "sec_SecurityProvider";
        static string TABLE_ALIAS = "provider";
        const string COL_ID = "ID";
        const string COL_Name = "Name";
        const string COL_Settings = "Settings";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_LastModifiedBy = "LastModifiedBy";
        const string COL_IsEnabled = "IsEnabled";
        const string COL_IsDefault = "IsDefault";


        static SecurityProviderDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_IsEnabled, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            columns.Add(COL_IsDefault, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "sec",
                DBTableName = "SecurityProvider",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }


        #endregion

        #region Mappers
        public SecurityProvider SecurityProviderMapper(IRDBDataReader reader)
        {
            return new SecurityProvider()
            {
                SecurityProviderId = reader.GetGuid(COL_ID),
                Name = reader.GetString(COL_Name),
                IsEnabled = reader.GetBoolean(COL_IsEnabled),
                Settings = Common.Serializer.Deserialize<SecurityProviderSettings>(reader.GetString(COL_Settings))
            };
        }
        #endregion

        #region Private Methods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_Sec", "SecurityDBConnStringKey", "SecurityDBConnString");
        }
        #endregion
        #region ISecurityProviderDataManager
        public SecurityProvider GetDefaultSecurityProvider()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Where().EqualsCondition(COL_IsDefault).Value(true);
            return queryContext.GetItem(SecurityProviderMapper);
        }

        public bool SetDefaultSecurityProvider(Guid securityProviderId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var caseExpression = updateQuery.Column(COL_IsDefault).CaseExpression();
            var caseWhenContext = caseExpression.AddCase();
            caseWhenContext.When().EqualsCondition(COL_ID).Value(securityProviderId);
            caseWhenContext.Then().Value(true);
            caseExpression.Else().Value(false);
            var whereQuery = updateQuery.Where(RDBConditionGroupOperator.OR);
            whereQuery.EqualsCondition(COL_IsDefault).Value(true);
            whereQuery.EqualsCondition(COL_ID).Value(securityProviderId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
