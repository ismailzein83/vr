using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRLocalizationLanguageDataManager : IVRLocalizationLanguageDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "VRLocalization_Language";
		static string TABLE_ALIAS = "vrLocalizationLanguage";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_ParentLanguageID = "ParentLanguageID";
		const string COL_Settings = "Settings";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRLocalizationLanguageDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_ParentLanguageID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "VRLocalization",
				DBTableName = "Language",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreVRLocalizationLanguagesUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public List<VRLocalizationLanguage> GetVRLocalizationLanguages()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(VRLocalizationLanguageMapper);
		}

		public bool Insert(VRLocalizationLanguage localizationLanguage)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_ID).Value(localizationLanguage.VRLanguageId);
			insertQuery.Column(COL_Name).Value(localizationLanguage.Name);
			if(localizationLanguage.ParentLanguageId.HasValue)
			insertQuery.Column(COL_ParentLanguageID).Value(localizationLanguage.ParentLanguageId.Value);
			else
				insertQuery.Column(COL_ParentLanguageID).Null();
			if (localizationLanguage.Settings != null)
				insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(localizationLanguage.Settings));
			else
				insertQuery.Column(COL_Settings).Null();
			return queryContext.ExecuteNonQuery() > 0;


		}

		public bool Update(VRLocalizationLanguage localizationLanguage)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_Name).Value(localizationLanguage.Name);
			if (localizationLanguage.ParentLanguageId.HasValue)
				updateQuery.Column(COL_ParentLanguageID).Value(localizationLanguage.ParentLanguageId.Value);
			else
				updateQuery.Column(COL_ParentLanguageID).Null();
			if (localizationLanguage.Settings != null)
				updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(localizationLanguage.Settings));
			else
				updateQuery.Column(COL_Settings).Null();
			updateQuery.Where().EqualsCondition(COL_ID).Value(localizationLanguage.VRLanguageId);

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
		private VRLocalizationLanguage VRLocalizationLanguageMapper(IRDBDataReader reader)
		{
			return new VRLocalizationLanguage
			{
				VRLanguageId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				ParentLanguageId = reader.GetNullableGuid(COL_ParentLanguageID),
				Settings = Vanrise.Common.Serializer.Deserialize<VRLocalizationLanguageSettings>(reader.GetString(COL_Settings))
			};

		}

		#endregion
	}
}
