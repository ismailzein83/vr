﻿using System;
using System.Collections.Generic;
using Vanrise.Data.RDB;
using Vanrise.Entities;
using TOne.WhS.SMSBusinessEntity.Data;
using TOne.WhS.SMSBusinessEntity.Entities;
using Vanrise.Common;


namespace TOne.WhS.SMSBusinessEntity.Data.RDB
{
    public class ProcessDraftDataManager : IProcessDraftDataManager
    {
        static string TABLE_NAME = "TOneWhS_SMSBE_ProcessDraft";
        static string TABLE_ALIAS = "SMSBEProcessDraft";
        const string COL_ID = "ID";
        const string COL_ProcessType = "ProcessType";
        const string COL_EntityID = "EntityID";
        const string COL_Changes = "Changes";
        const string COL_Status = "Status";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_CreatedBy = "CreatedBy";
        const string COL_LastModifiedTime = "LastModifiedTime";
        const string COL_LastModifiedBy = "LastModifiedBy";


        static ProcessDraftDataManager()
        {
            var columns = new Dictionary<string, RDBTableColumnDefinition>();
            columns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_ProcessType, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_EntityID, new RDBTableColumnDefinition { DataType = RDBDataType.Varchar });
            columns.Add(COL_Changes, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
            columns.Add(COL_Status, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_CreatedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            columns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
            columns.Add(COL_LastModifiedBy, new RDBTableColumnDefinition { DataType = RDBDataType.Int });
            RDBSchemaManager.Current.RegisterDefaultTableDefinition(TABLE_NAME, new RDBTableDefinition
            {
                DBSchemaName = "TOneWhS_SMSBE",
                DBTableName = "ProcessDraft",
                Columns = columns,
                IdColumnName = COL_ID,
                CreatedTimeColumnName = COL_CreatedTime,
                ModifiedTimeColumnName = COL_LastModifiedTime

            });
        }

        BaseRDBDataProvider GetDataProvider()
        {
            return RDBDataProviderFactory.CreateProvider("TOneWhS_SMSBuisenessEntity", "TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString");
        }

        public ProcessDraft GetChanges(ProcessEntityType processType, string entityID)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS, null, true);
            selectQuery.SelectColumns().Column(COL_ID);
            selectQuery.SelectColumns().Column(COL_Changes);
            var where = selectQuery.Where();
            where.EqualsCondition(COL_ProcessType).Value((int)processType);
            where.EqualsCondition(COL_EntityID).Value(entityID);
            where.EqualsCondition(COL_Status).Value((int)ProcessStatus.Draft);

            return queryContext.GetItem(ChangesMapper);
        }

        public bool InsertOrUpdateChanges(ProcessEntityType processType, string serializedChanges, string entityID, int userID, out int? processDraftID)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Changes).Value(serializedChanges);
            updateQuery.Column(COL_LastModifiedBy).Value(userID);
            var where = updateQuery.Where();
            where.EqualsCondition(COL_ProcessType).Value((int)processType);
            where.EqualsCondition(COL_EntityID).Value(entityID);
            where.EqualsCondition(COL_Status).Value((int)ProcessStatus.Draft);

            updateQuery.AddSelectColumn(COL_ID);
            processDraftID = queryContext.ExecuteScalar().NullableIntValue;

            if (!processDraftID.HasValue)
            {
                var query2Context = new RDBQueryContext(GetDataProvider());
                var insertQuery = query2Context.AddInsertQuery();
                insertQuery.IntoTable(TABLE_NAME);
                insertQuery.Column(COL_ProcessType).Value((int)processType);
                insertQuery.Column(COL_EntityID).Value(entityID);
                insertQuery.Column(COL_Changes).Value(serializedChanges);
                insertQuery.Column(COL_Status).Value((int)ProcessStatus.Draft);
                insertQuery.Column(COL_CreatedBy).Value(userID);
                insertQuery.AddSelectGeneratedId();

                processDraftID  = query2Context.ExecuteScalar().NullableIntValue;
            }

            return processDraftID.HasValue;
        }

        public bool UpdateProcessStatus(ProcessEntityType processType, int processID, ProcessStatus newStatus, int userID)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(TABLE_NAME);
            updateQuery.Column(COL_Status).Value((int)newStatus);
            updateQuery.Column(COL_LastModifiedBy).Value(userID);

            var where = updateQuery.Where();
            where.EqualsCondition(COL_ProcessType).Value((int)processType);
            where.EqualsCondition(COL_ID).Value(processID);

            return queryContext.ExecuteNonQuery() > 0;
        }

        public DraftStateResult CheckIfDraftExist(ProcessEntityType processType, string entityID)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(TABLE_NAME, TABLE_ALIAS);
            selectQuery.SelectColumns().Column(COL_ID);
            var where = selectQuery.Where();
            where.EqualsCondition(COL_ProcessType).Value((int)processType);
            where.EqualsCondition(COL_EntityID).Value(entityID);
            where.EqualsCondition(COL_Status).Value((int)ProcessStatus.Draft);

            int? executeScalarValue = queryContext.ExecuteScalar().IntWithNullHandlingValue;
            return executeScalarValue.HasValue ? new DraftStateResult() { ProcessDraftID = executeScalarValue.Value } : null;
        }

        private ProcessDraft ChangesMapper(IRDBDataReader dataReader)
        {
            return new ProcessDraft()
            {
                ID = dataReader.GetInt(COL_ID),
                Changes = dataReader.GetString(COL_Changes)
            };
        }

    }
}