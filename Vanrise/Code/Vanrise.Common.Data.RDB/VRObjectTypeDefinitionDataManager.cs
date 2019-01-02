using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRObjectTypeDefinitionDataManager : IVRObjectTypeDefinitionDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRObjectTypeDefinition";
		static string TABLE_ALIAS = "vrObjectTypeDefinition";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRObjectTypeDefinitionDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRObjectTypeDefinition",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public bool AreVRObjectTypeDefinitionUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public void GenerateScript(List<VRObjectTypeDefinition> objTypeDefs, Action<string, string> addEntityScript)
		{
			throw new NotImplementedException();
		}

		public List<VRObjectTypeDefinition> GetVRObjectTypeDefinitions()
		{
			var querycontext = new RDBQueryContext(GetDataProvider());
			var selectQuery = querycontext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return querycontext.GetItems(VRObjectTypeDefinitionMapper);
		}

		public bool Insert(VRObjectTypeDefinition vrObjectTypeDefinitionItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);

			var ifNotExists = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExists.EqualsCondition(COL_Name).Value(vrObjectTypeDefinitionItem.Name);

			insertQuery.Column(COL_ID).Value(vrObjectTypeDefinitionItem.VRObjectTypeDefinitionId);
			insertQuery.Column(COL_Name).Value(vrObjectTypeDefinitionItem.Name);
			if (vrObjectTypeDefinitionItem.Settings!=null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrObjectTypeDefinitionItem.Settings));

			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VRObjectTypeDefinition vrObjectTypeDefinitionItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);

			var ifNotExists = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExists.NotEqualsCondition(COL_ID).Value(vrObjectTypeDefinitionItem.VRObjectTypeDefinitionId);
			ifNotExists.EqualsCondition(COL_Name).Value(vrObjectTypeDefinitionItem.Name);

			updateQuery.Column(COL_Name).Value(vrObjectTypeDefinitionItem.Name);
			if (vrObjectTypeDefinitionItem.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrObjectTypeDefinitionItem.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrObjectTypeDefinitionItem.VRObjectTypeDefinitionId);

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
		private VRObjectTypeDefinition VRObjectTypeDefinitionMapper(IRDBDataReader reader)
		{
			return new VRObjectTypeDefinition
			{
				VRObjectTypeDefinitionId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				Settings = Vanrise.Common.Serializer.Deserialize<VRObjectTypeDefinitionSettings>(reader.GetString(COL_Settings))
			};
		}
		#endregion

	}
}
