using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Data.RDB
{
    public class BEParentChildRelationDataManager : IBEParentChildRelationDataManager
    {
        #region RDB
        static string TABLE_NAME = "genericdata_BEParentChildRelation";
        static string TABLE_ALIAS = "beParentChildRelation";
        const string COL_ID = "ID";
        const string COL_RelationDefinitionID = "RelationDefinitionID";
        const string COL_ParentBEID = "ParentBEID";
        const string COL_ChildBEID = "ChildBEID";
        const string COL_BED = "BED";
        const string COL_EED = "EED";
        const string COL_CreatedTime = "CreatedTime";


        static BEParentChildRelationDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
            columns.Add(COL_RelationDefinitionID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
            columns.Add(COL_ParentBEID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_ChildBEID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar, Size = 50 });
            columns.Add(COL_BED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_EED, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "genericdata",
                DBTableName = "BEParentChildRelation",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime
            });
        }

        #endregion

        #region Mappers 
        BEParentChildRelation BEParentChildRelationMapper(IRDBDataReader reader)
        {
            BEParentChildRelation beParentChildRelation = new BEParentChildRelation
            {
                BEParentChildRelationId = reader.GetLong(COL_ID),
                RelationDefinitionId = reader.GetGuid(COL_RelationDefinitionID),
                ParentBEId = reader.GetString(COL_ParentBEID),
                ChildBEId = reader.GetString(COL_ChildBEID),
                BED = reader.GetDateTime(COL_BED),
                EED = reader.GetNullableDateTime(COL_EED)
            };
            return beParentChildRelation;
        }
        #endregion

        #region PrivateMethods
        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("VR_GenericData_BEParentChildRelation", "ConfigurationDBConnStringKey", "ConfigurationDBConnStringKey");
        }
        #endregion

        #region IBEParentChildRelationDataManager
        public List<BEParentChildRelation> GetBEParentChildRelations(Guid beParentChildRelationDefinitionId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Columns(COL_ID, COL_RelationDefinitionID, COL_ParentBEID, COL_ChildBEID, COL_BED, COL_EED);
            selectQuery.Where().EqualsCondition(COL_RelationDefinitionID).Value(beParentChildRelationDefinitionId);
            return queryContext.GetItems<BEParentChildRelation>(BEParentChildRelationMapper);
        }

        public bool AreBEParentChildRelationUpdated(Guid beParentChildRelationDefinitionId, ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public bool Insert(BEParentChildRelation BEParentChildRelationItem, out long insertedId)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(TABLE_NAME);
            insertQuery.AddSelectGeneratedId();

            insertQuery.Column(COL_RelationDefinitionID).Value(BEParentChildRelationItem.RelationDefinitionId);
            insertQuery.Column(COL_ParentBEID).Value(BEParentChildRelationItem.ParentBEId);
            insertQuery.Column(COL_ChildBEID).Value(BEParentChildRelationItem.ChildBEId);
            insertQuery.Column(COL_BED).Value(BEParentChildRelationItem.BED);
            if (BEParentChildRelationItem.EED.HasValue)
                insertQuery.Column(COL_EED).Value(BEParentChildRelationItem.EED.Value);
            else
                insertQuery.Column(COL_EED).Null();

            insertedId = queryContext.ExecuteScalar().LongWithNullHandlingValue;
            return insertedId != 0;
        }

        public bool Update(BEParentChildRelation BEParentChildRelationItem)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);

            updateQuery.Column(COL_RelationDefinitionID).Value(BEParentChildRelationItem.RelationDefinitionId);
            updateQuery.Column(COL_ParentBEID).Value(BEParentChildRelationItem.ParentBEId);
            updateQuery.Column(COL_ChildBEID).Value(BEParentChildRelationItem.ChildBEId);
            updateQuery.Column(COL_BED).Value(BEParentChildRelationItem.BED);
            if (BEParentChildRelationItem.EED.HasValue)
                updateQuery.Column(COL_EED).Value(BEParentChildRelationItem.EED.Value);
            else
                updateQuery.Column(COL_EED).Null();
            updateQuery.Where().EqualsCondition(COL_ID).Value(BEParentChildRelationItem.BEParentChildRelationId);
            return queryContext.ExecuteNonQuery() > 0;
        }
        #endregion
    }
}
