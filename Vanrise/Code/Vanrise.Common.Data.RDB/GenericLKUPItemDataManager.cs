using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class GenericLKUPItemDataManager : IGenericLKUPItemDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_GenericLKUP";
		static string TABLE_ALIAS = "genericLKUP";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_BusinessEntityDefinitionID = "BusinessEntityDefinitionID";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";
        #endregion
        #region Contructors
        static GenericLKUPItemDataManager()
		{
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
            columns.Add(COL_BusinessEntityDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "common",
                DBTableName = "GenericLKUP",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }
		#endregion
		#region Public Methods

		public bool AreGenericLKUPItemUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

		public List<GenericLKUPItem> GetGenericLKUPItem()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(GenericLKUPItemMapper);
		}

		public bool Insert(GenericLKUPItem genericLKUPItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_Name).Value(genericLKUPItem.Name);
			ifNotExist.EqualsCondition(COL_BusinessEntityDefinitionID).Value(genericLKUPItem.BusinessEntityDefinitionId);
			insertQuery.Column(COL_ID).Value(genericLKUPItem.GenericLKUPItemId);
			insertQuery.Column(COL_Name).Value(genericLKUPItem.Name);
			insertQuery.Column(COL_BusinessEntityDefinitionID).Value(genericLKUPItem.BusinessEntityDefinitionId);
            if(genericLKUPItem.Settings != null)
			  insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(genericLKUPItem.Settings));
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(GenericLKUPItem genericLKUPItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(genericLKUPItem.GenericLKUPItemId);
			ifNotExist.EqualsCondition(COL_Name).Value(genericLKUPItem.Name);
			ifNotExist.EqualsCondition(COL_BusinessEntityDefinitionID).Value(genericLKUPItem.BusinessEntityDefinitionId);
			updateQuery.Column(COL_Name).Value(genericLKUPItem.Name);
            if (genericLKUPItem.Settings != null)
            {
                updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(genericLKUPItem.Settings));
            }
            else
            {
                updateQuery.Column(COL_Settings).Null();
            }

            updateQuery.Column(COL_BusinessEntityDefinitionID).Value(genericLKUPItem.BusinessEntityDefinitionId);
			updateQuery.Where().EqualsCondition(COL_ID).Value(genericLKUPItem.GenericLKUPItemId);
			return queryContext.ExecuteNonQuery() > 0;
		}

		#endregion
		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_GenericLKUP", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		GenericLKUPItem GenericLKUPItemMapper(IRDBDataReader reader)
		{
            return new GenericLKUPItem
			{
				GenericLKUPItemId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				BusinessEntityDefinitionId = reader.GetGuid(COL_BusinessEntityDefinitionID),
				Settings = Vanrise.Common.Serializer.Deserialize<GenericLKUPItemSettings>(reader.GetString(COL_Settings)),
			};
		}

		#endregion

	}
}
