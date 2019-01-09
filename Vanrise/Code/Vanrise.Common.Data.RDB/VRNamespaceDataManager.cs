using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRNamespaceDataManager : IVRNamespaceDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRNamespace";
		static string TABLE_ALIAS = "vrNamespace";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_LastModifiedTime = "LastModifiedTime";
		const string COL_CreatedTime = "CreatedTime";
		#endregion

		#region Constructors
		static VRNamespaceDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRNamespace",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreVRNamespaceUpdated(ref object updateHandle)
		{
            var queryContext = new RDBQueryContext(GetDataProvider());
            return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
        }

		public List<VRNamespace> GetVRNamespaces()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS,null,true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name,RDBSortDirection.ASC);
			return queryContext.GetItems(VRNamespaceMapper);
		}

        public bool Insert(VRNamespace vrNamespaceItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
            ifNotExist.EqualsCondition(COL_Name).Value(vrNamespaceItem.Name);
            insertQuery.Column(COL_ID).Value(vrNamespaceItem.VRNamespaceId);
            insertQuery.Column(COL_Name).Value(vrNamespaceItem.Name);
         
            return queryContext.ExecuteNonQuery() > 0;
        }

		public bool Update(VRNamespace vrNamespaceItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.NotEqualsCondition(COL_ID).Value(vrNamespaceItem.VRNamespaceId);
			ifNotExist.EqualsCondition(COL_Name).Value(vrNamespaceItem.Name);
			updateQuery.Column(COL_Name).Value(vrNamespaceItem.Name);

            updateQuery.Where().EqualsCondition(COL_ID).Value(vrNamespaceItem.VRNamespaceId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		VRNamespace VRNamespaceMapper(IRDBDataReader reader)
		{
			return  new VRNamespace
			{
				VRNamespaceId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
			};
		}
		#endregion

	}
}
