﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;
using Vanrise.Entities;
namespace Vanrise.GenericData.Data.RDB
{
    public class BusinessEntityDefinitionDataManager : IBusinessEntityDefinitionDataManager
    {
        #region RDB

        public static string TABLE_NAME = "genericdata_BusinessEntityDefinition";
        static string TABLE_ALIAS = "beDefinition";
        public const string COL_ID = "ID";
        public const string COL_DevProjectID = "DevProjectID";
        public const string COL_Name = "Name";
        public const string COL_Title = "Title";
        public const string COL_Settings = "Settings";
        public const string COL_CreatedTime = "CreatedTime";
        public const string COL_LastModifiedTime = "LastModifiedTime";

        static BusinessEntityDefinitionDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_DevProjectID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 900 });
            columns.Add(COL_Title, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "BusinessEntityDefinition",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime
            });
        }

        #endregion
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #region PrivateMethods
        BusinessEntityDefinition BusinessEntityDefinitionMapper(IRDBDataReader reader)
        {
            return new BusinessEntityDefinition()
            {
                BusinessEntityDefinitionId = reader.GetGuid(COL_ID),
                DevProjectId = reader.GetNullableGuid(COL_DevProjectID),
                Name = reader.GetString(COL_Name),
                Title = reader.GetString(COL_Title),
                Settings = Vanrise.Common.Serializer.Deserialize<BusinessEntityDefinitionSettings>(reader.GetString(COL_Settings))
            };
        }

        #endregion

        #region Mappers

        #endregion
        #region IBusinessEntityDefinitionDataManager
        public bool AddBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(businessEntityDefinition.Name);
            insertQuery.Column(COL_ID).Value(businessEntityDefinition.BusinessEntityDefinitionId);
            insertQuery.Column(COL_Name).Value(businessEntityDefinition.Name);
            insertQuery.Column(COL_Title).Value(businessEntityDefinition.Title);

            if (businessEntityDefinition.DevProjectId.HasValue)
            insertQuery.Column(COL_DevProjectID).Value(businessEntityDefinition.DevProjectId.Value);

            if (businessEntityDefinition.Settings != null)
                insertQuery.Column(COL_Settings).Value(Serializer.Serialize(businessEntityDefinition.Settings));
            return queryContext.ExecuteNonQuery() > 0;

        }

        public bool AreGenericRuleDefinitionsUpdated(ref object updateHandle)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

        public void GenerateScript(List<BusinessEntityDefinition> beDefinitions, Action<string, string> addEntityScript)
        {
            StringBuilder scriptBuilder = new StringBuilder();
            foreach (var beDefinition in beDefinitions)
            {
                if (scriptBuilder.Length > 0)
                {
                    scriptBuilder.Append(",");
                    scriptBuilder.AppendLine();
                }
                scriptBuilder.AppendFormat(@"('{0}','{1}','{2}','{3}')", beDefinition.BusinessEntityDefinitionId, beDefinition.Name, beDefinition.Title, Serializer.Serialize(beDefinition.Settings));
            }
            string script = String.Format(@"set nocount on;
;with cte_data([ID],[Name],[Title],[Settings])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
{0}
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([ID],[Name],[Title],[Settings]))
merge	[genericdata].[BusinessEntityDefinition] as t
using	cte_data as s
on		1=1 and t.[ID] = s.[ID]
when matched then
	update set
	[Name] = s.[Name],[Title] = s.[Title],[Settings] = s.[Settings]
when not matched by target then
	insert([ID],[Name],[Title],[Settings])
	values(s.[ID],s.[Name],s.[Title],s.[Settings]);", scriptBuilder);
            addEntityScript("[genericdata].[BusinessEntityDefinition]", script);
        }

        public IEnumerable<BusinessEntityDefinition> GetBusinessEntityDefinitions()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
            selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
            return queryContext.GetItems(BusinessEntityDefinitionMapper);
        }

        public bool UpdateBusinessEntityDefinition(BusinessEntityDefinition businessEntityDefinition)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
            ifNotExists.EqualsCondition(COL_Name).Value(businessEntityDefinition.Name);
            ifNotExists.NotEqualsCondition(COL_ID).Value(businessEntityDefinition.BusinessEntityDefinitionId);
            updateQuery.Column(COL_Name).Value(businessEntityDefinition.Name);
            updateQuery.Column(COL_Title).Value(businessEntityDefinition.Title);
            if (businessEntityDefinition.DevProjectId.HasValue)
                updateQuery.Column(COL_DevProjectID).Value(businessEntityDefinition.DevProjectId.Value);
            else
                updateQuery.Column(COL_DevProjectID).Null();
            if (businessEntityDefinition.Settings != null)
                updateQuery.Column(COL_Settings).Value(Serializer.Serialize(businessEntityDefinition.Settings));
            else
                updateQuery.Column(COL_Settings).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(businessEntityDefinition.BusinessEntityDefinitionId);
            return queryContext.ExecuteNonQuery() > 0;

        }
        #endregion
    }
}
