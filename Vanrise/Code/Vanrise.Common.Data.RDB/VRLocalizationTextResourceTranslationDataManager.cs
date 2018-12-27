using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRLocalizationTextResourceTranslationDataManager : IVRLocalizationTextResourceTranslationDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "VRLocalization_TextResourceTranslation";
		static string TABLE_ALIAS = "vrLocalizationTextResourceTranslation";
		const string COL_ID = "ID";
		const string COL_TextResourceID = "TextResourceID";
		const string COL_LanguageID = "LanguageID";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRLocalizationTextResourceTranslationDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_TextResourceID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_LanguageID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "VRLocalization",
				DBTableName = "TextResourceTranslation",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AddVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation localizationTextResource)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_TextResourceID).Value(localizationTextResource.ResourceId);
			ifNotExist.EqualsCondition(COL_LanguageID).Value(localizationTextResource.LanguageId);
			insertQuery.Column(COL_ID).Value(localizationTextResource.VRLocalizationTextResourceTranslationId);
			insertQuery.Column(COL_TextResourceID).Value(localizationTextResource.ResourceId);
			insertQuery.Column(COL_LanguageID).Value(localizationTextResource.LanguageId);
			if (localizationTextResource.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(localizationTextResource.Settings));

			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool AreVRLocalizationTextResourcesTranslationUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public List<VRLocalizationTextResourceTranslation> GetVRLocalizationTextResourcesTranslation()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(VRLocalizationTextResourceTranslationMapper);
		}

		public bool UpdateVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation localizationTextResource)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery= queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS);
			ifNotExist.EqualsCondition(COL_TextResourceID).Value(localizationTextResource.ResourceId);
			ifNotExist.EqualsCondition(COL_LanguageID).Value(localizationTextResource.LanguageId);
			ifNotExist.NotEqualsCondition(COL_ID).Value(localizationTextResource.VRLocalizationTextResourceTranslationId);
			updateQuery.Column(COL_TextResourceID).Value(localizationTextResource.ResourceId);
			updateQuery.Column(COL_LanguageID).Value(localizationTextResource.LanguageId);
			if (localizationTextResource.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(localizationTextResource.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(localizationTextResource.VRLocalizationTextResourceTranslationId);

			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion
		 
		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "VRLocalizationDBConnStringKey", "VRLocalizationDBConnString");
		}

		#endregion

		#region Mappers
		private VRLocalizationTextResourceTranslation VRLocalizationTextResourceTranslationMapper(IRDBDataReader reader)
		{
			return new VRLocalizationTextResourceTranslation
			{
				VRLocalizationTextResourceTranslationId = reader.GetGuid(COL_ID),
				ResourceId = reader.GetGuid(COL_TextResourceID),
				LanguageId = reader.GetGuid(COL_LanguageID),
				Settings = Vanrise.Common.Serializer.Deserialize<VRLocalizationTextResourceTranslationSettings>(reader.GetString(COL_Settings)),
			};


		}
		#endregion

	}
}
