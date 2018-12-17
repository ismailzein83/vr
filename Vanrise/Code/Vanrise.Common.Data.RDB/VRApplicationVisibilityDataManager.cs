using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRApplicationVisibilityDataManager : IVRApplicationVisibilityDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRAppVisibility";
		static string TABLE_ALIAS = "vrAppVisibility";
		const string COL_ID = "ID";
		const string COL_Name = "Name";
		const string COL_Settings = "Settings";
		const string COL_IsCurrent = "IsCurrent";
		const string COL_CreatedTime = "CreatedTime";
		#endregion

		#region Constructors
		static VRApplicationVisibilityDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_Name, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
			columns.Add(COL_Settings, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_IsCurrent, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRAppVisibility",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime
			});
		}
		#endregion

		#region Public Methods
		public bool AreVRApplicationVisibilityUpdated(ref object updateHandle)
		{

			throw new NotImplementedException();
		}

		public List<VRApplicationVisibility> GetVRApplicationVisibilities()
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
			selectQuery.SelectColumns().Columns(COL_ID, COL_Name, COL_IsCurrent, COL_Settings);
			selectQuery.Sort().ByColumn(COL_Name, RDBSortDirection.ASC);
			return queryContext.GetItems(VRApplicationVisibilityMapper);
		}

		public bool Insert(VRApplicationVisibility vrApplicationVisibilityItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			var ifNotExist = insertQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.EqualsCondition(COL_Name).Value(vrApplicationVisibilityItem.Name);
			insertQuery.Column(COL_ID).Value(vrApplicationVisibilityItem.VRApplicationVisibilityId);
			insertQuery.Column(COL_Name).Value(vrApplicationVisibilityItem.Name);
			insertQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrApplicationVisibilityItem.Settings));
			return queryContext.ExecuteNonQuery() > 0;
		}

		public bool Update(VRApplicationVisibility vrApplicationVisibilityItem)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var updateQuery = queryContext.AddUpdateQuery();
			updateQuery.FromTable(TABLE_NAME);
			var ifNotExist = updateQuery.IfNotExists(TABLE_ALIAS, RDBConditionGroupOperator.AND);
			ifNotExist.NotEqualsCondition(COL_ID).Value(vrApplicationVisibilityItem.VRApplicationVisibilityId);
			ifNotExist.EqualsCondition(COL_Name).Value(vrApplicationVisibilityItem.Name);
			updateQuery.Column(COL_Name).Value(vrApplicationVisibilityItem.Name);
			updateQuery.Column(COL_Settings).Value(Vanrise.Common.Serializer.Serialize(vrApplicationVisibilityItem.Settings));
			updateQuery.Where().EqualsCondition(COL_ID).Value(vrApplicationVisibilityItem.VRApplicationVisibilityId);
			return queryContext.ExecuteNonQuery() > 0;
		}
		#endregion

		#region Private Methods
		BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common_VRAppVisibility", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}

		#endregion

		#region Mappers
		VRApplicationVisibility VRApplicationVisibilityMapper(IRDBDataReader reader)
		{
			return  new VRApplicationVisibility
			{
				VRApplicationVisibilityId = reader.GetGuid(COL_ID),
				Name = reader.GetString(COL_Name),
				IsCurrent = reader.GetNullableBoolean(COL_IsCurrent),
				Settings = Vanrise.Common.Serializer.Deserialize<VRApplicationVisibilitySettings>(reader.GetString(COL_Settings))
			};
		}
		#endregion

	}
}
