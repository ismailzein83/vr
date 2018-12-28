using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.Common.Data.RDB
{
	public class VRCommentDataManager : IVRCommentDataManager
	{
		#region Local Variables
		static string TABLE_NAME = "common_VRComment";
		static string TABLE_ALIAS = "vrComment";
		const string COL_ID = "ID";
		const string COL_DefinitionId = "DefinitionId";
		const string COL_ObjectId = "ObjectId";
		const string COL_Content = "Content";
		const string COL_CreatedBy = "CreatedBy";
		const string COL_CreatedTime = "CreatedTime";
		const string COL_LastModifiedBy = "LastModifiedBy";
		const string COL_LastModifiedTime = "LastModifiedTime";
		#endregion

		#region Constructors
		static VRCommentDataManager()
		{
			var columns = new Dictionary<string, RDBTableColumnDefinition>();
			columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
			columns.Add(COL_DefinitionId, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
			columns.Add(COL_ObjectId, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
			columns.Add(COL_Content, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
			columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
			columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
			RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
			{
				DBSchemaName = "common",
				DBTableName = "VRComment",
				Columns = columns,
				IdColumnName = COL_ID,
				CreatedTimeColumnName = COL_CreatedTime,
				ModifiedTimeColumnName = COL_LastModifiedTime
			});
		}
		#endregion

		#region Public Methods
		public IEnumerable<VRComment> GetFilteredVRComments(DataRetrievalInput<VRCommentQuery> input)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Where().EqualsCondition(COL_DefinitionId).Value(input.Query.DefinitionId);
			selectQuery.Where().EqualsCondition(COL_ObjectId).Value(input.Query.ObjectId);
			selectQuery.Sort().ByColumn(COL_CreatedTime, RDBSortDirection.DESC);
			return queryContext.GetItems(VRCommentMapper);
		}
		public VRComment GetVRCommentById(long vRCommentId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var selectQuery = queryContext.AddSelectQuery();
			selectQuery.From(TABLE_NAME, TABLE_ALIAS);
			selectQuery.SelectColumns().AllTableColumns(TABLE_ALIAS);
			selectQuery.Where().EqualsCondition(COL_ID).Value(vRCommentId);
			return queryContext.GetItem(VRCommentMapper);
		}
		public bool Insert(VRComment vrComment, out long reportId)
		{
			var queryContext = new RDBQueryContext(GetDataProvider());
			var insertQuery = queryContext.AddInsertQuery();
			insertQuery.IntoTable(TABLE_NAME);
			insertQuery.Column(COL_DefinitionId).Value(vrComment.DefinitionId);
			insertQuery.Column(COL_ObjectId).Value(vrComment.ObjectId);
			insertQuery.Column(COL_Content).Value(vrComment.Content);
			insertQuery.Column(COL_CreatedBy).Value(vrComment.CreatedBy);
			insertQuery.Column(COL_LastModifiedBy).Value(vrComment.LastModifiedBy);
			insertQuery.AddSelectGeneratedId();
			var insertedId = queryContext.ExecuteScalar().NullableLongValue;
			if (insertedId.HasValue)
			{
				reportId = insertedId.Value;
				return true;
			}
			reportId = -1;
			return false;
		}
		#endregion

		#region Private Methods
		private BaseRDBDataProvider GetDataProvider()
		{
			return RDBDataProviderFactory.CreateProvider("VR_Common", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
		}
		#endregion

		#region Mappers
		private VRComment VRCommentMapper(IRDBDataReader reader)
		{
			return new VRComment
			{
				VRCommentId = reader.GetLong(COL_ID),
				DefinitionId =reader.GetGuid(COL_DefinitionId),
				ObjectId = reader.GetString(COL_ObjectId),
				Content = reader.GetString(COL_Content),
				CreatedBy = reader.GetInt(COL_CreatedBy),
				CreatedTime = reader.GetDateTime(COL_CreatedTime),
				LastModifiedTime = reader.GetDateTime(COL_LastModifiedTime),
				LastModifiedBy = reader.GetInt(COL_LastModifiedBy)
			};
		}
		#endregion

	}
}
