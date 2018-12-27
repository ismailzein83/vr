using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRLocalizationModuleDataManager : IVRLocalizationModuleDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "VRLocalization_Module";
		static string TABLE_ALIAS = "vrLocalizationModule";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedTime = "LastModifiedTime";

		#endregion

		#region Constructors
		static VRLocalizationModuleDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "VRLocalization",
				DBTableName = "Module",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime

			});
		}
		#endregion

		#region Public Methods
		public bool AreVRLocalizationModulesUpdated(ref object updateHandle)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			return queryContext.IsDataUpdated(TABLE_NAME, ref updateHandle);
		}

		public List<VRLocalizationModule> GetVRLocalizationModules()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			return queryContext.GetItems(VRLocalizationModuleMapper);
		}

		public bool Insert(VRLocalizationModule localizationModule)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_ID).Value(localizationModule.VRLocalizationModuleId);
			insertQuery.Column(COL_Name).Value(localizationModule.Name);
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VRLocalizationModule localizationModule)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			updateQuery.Column(COL_Name).Value(localizationModule.Name);
			updateQuery.Where().EqualsCondition(COL_ID).Value(localizationModule.VRLocalizationModuleId);
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
		private VRLocalizationModule VRLocalizationModuleMapper(IRDBDataReader reader)
		{
			return new VRLocalizationModule
			{
				VRLocalizationModuleId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name)
			};

		}
		#endregion

	}
}
