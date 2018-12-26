using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRLocalizationTextResourceDataManager : IVRLocalizationTextResourceDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "VRLocalization_TextResource";
		static string TABLE_ALIAS = "vrLocalizationTextResource";
		const string COL_ID = "ID";
		const string COL_ResourceKey = "ResourceKey";
		const string COL_ModuleID = "ModuleID";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRLocalizationTextResourceDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_ResourceKey, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 255 });
			columns.Add(COL_ModuleID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "VRLocalization",
				DBTableName = "TextResource",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreVRLocalizationTextResourcesUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public List<VRLocalizationTextResource> GetVRLocalizationTextResources()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(VRLocalizationTextResourceMapper);
		}

		public bool Insert(VRLocalizationTextResource localizationTextResource)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_ID).Value(localizationTextResource.VRLocalizationTextResourceId);
			insertQuery.Column(COL_ResourceKey).Value(localizationTextResource.ResourceKey);
			insertQuery.Column(COL_ModuleID).Value(localizationTextResource.ModuleId);
			if (localizationTextResource.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(localizationTextResource.Settings));
			else
				insertQuery.Column(COL_Settings).Null();
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VRLocalizationTextResource localizationTextResource)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_ResourceKey).Value(localizationTextResource.ResourceKey);
			updateQuery.Column(COL_ModuleID).Value(localizationTextResource.ModuleId);
			if (localizationTextResource.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(localizationTextResource.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(localizationTextResource.VRLocalizationTextResourceId);

			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "VRLocalizationDBConnStringKey", "VRLocalizationDBConnString");
		}

		#endregion

		#region Mappers
		private VRLocalizationTextResource VRLocalizationTextResourceMapper(IRDBDataReader reader)
		{
			return new VRLocalizationTextResource
			{
				VRLocalizationTextResourceId = reader.GetGuid(COL_ID),
				ResourceKey = reader.GetString(COL_ResourceKey),
				ModuleId = reader.GetGuid(COL_ModuleID),
				Settings = Vanrise.Common.Serializer.Deserialize<VRLocalizationTextResourceSettings>(reader.GetString(COL_Settings)),
			};
		}
		#endregion

	}
}
