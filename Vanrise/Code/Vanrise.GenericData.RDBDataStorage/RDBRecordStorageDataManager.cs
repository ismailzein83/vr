﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Data.RDB;
using Vanrise.Entities;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBRecordStorageDataManager : IDataRecordDataManager, ISummaryRecordDataManager
    {
        #region ctor/Fields

        const string PARENTRECORDSTORAGE_RDBTABLEALIAS = "parentStorage";

        RDBDataStoreSettings _dataStoreSettings;
        RDBDataRecordStorageSettings _dataRecordStorageSettings;
        DataRecordStorage _dataRecordStorage;
        SummaryTransformationDefinition _summaryTransformationDefinition;
        RDBTempStorageInformation _rdbTempStorageInformation;
        DataRecordStorageManager s_dataRecordStorageManager = new DataRecordStorageManager();

        DataRecordType _dataRecordType;
        DataRecordType DataRecordType
        {
            get
            {
                if (_dataRecordType == null)
                {
                    DataRecordTypeManager manager = new DataRecordTypeManager();
                    _dataRecordType = manager.GetDataRecordType(_dataRecordStorage.DataRecordTypeId);
                    if (_dataRecordType == null)
                        throw new NullReferenceException(String.Format("_dataRecordType ID '{0}'", _dataRecordStorage.DataRecordTypeId));
                }
                return _dataRecordType;
            }
        }

        Dictionary<string, DataRecordField> _dataRecordFieldsByName;
        Dictionary<string, DataRecordField> DataRecordFieldsByName
        {
            get
            {
                if (_dataRecordFieldsByName == null)
                {
                    DataRecordTypeManager manager = new DataRecordTypeManager();
                    _dataRecordFieldsByName = manager.GetDataRecordTypeFields(_dataRecordStorage.DataRecordTypeId);
                    if (_dataRecordFieldsByName == null)
                        throw new NullReferenceException(String.Format("_dataRecordFieldsByName ID '{0}'", _dataRecordStorage.DataRecordTypeId));
                }
                return _dataRecordFieldsByName;
            }
        }

        IDynamicManager _dynamicManager;
        IDynamicManager DynamicManager
        {
            get
            {
                if (_dynamicManager == null)
                {
                    if (_dataRecordStorage == null)
                        throw new NullReferenceException("_dataRecordStorage");
                    DynamicTypeGenerator dynamicTypeGenerator = new DynamicTypeGenerator();
                    _dynamicManager = dynamicTypeGenerator.GetDynamicManager(_dataRecordStorage, _dataRecordStorageSettings);
                    if (_dynamicManager == null)
                        throw new NullReferenceException("_dynamicManager");
                }
                return _dynamicManager;
            }
        }

        internal RDBRecordStorageDataManager(RDBDataStoreSettings dataStoreSettings, RDBDataRecordStorageSettings dataRecordStorageSettings, DataRecordStorage dataRecordStorage, RDBTempStorageInformation sqlTempStorageInformation)
        {
            this._dataStoreSettings = dataStoreSettings;
            this._dataRecordStorageSettings = dataRecordStorageSettings;
            this._dataRecordStorage = dataRecordStorage;
            this._rdbTempStorageInformation = sqlTempStorageInformation;
        }

        internal RDBRecordStorageDataManager(RDBDataStoreSettings dataStoreSettings, RDBDataRecordStorageSettings dataRecordStorageSettings, DataRecordStorage dataRecordStorage, SummaryTransformationDefinition summaryTransformationDefinition, RDBTempStorageInformation sqlTempStorageInformation)
            : this(dataStoreSettings, dataRecordStorageSettings, dataRecordStorage, sqlTempStorageInformation)
        {
            this._summaryTransformationDefinition = summaryTransformationDefinition;
        }

        #endregion

        #region IDataRecordDataManager

        public void ApplyStreamToDB(object stream)
        {
            stream.CastWithValidate<RDBBulkInsertQueryContext>("stream").Apply();
        }

        public bool AreDataRecordsUpdated(ref object updateHandle)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            return new RDBQueryContext(GetDataProvider()).IsDataUpdated(rdbRegistrationInfo.SchemaManager, rdbRegistrationInfo.RDBTableUniqueName, ref updateHandle);
        }

        public bool Delete(List<object> recordFieldIds)
        {
            return DeleteRecords_Private(null, null, null, null, recordFieldIds) > 0;
        }

        public void DeleteRecords(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup)
        {
            DeleteRecords_Private(from, to, null, recordFilterGroup, null);
        }

        public void DeleteRecords(DateTime dateTime, RecordFilterGroup recordFilterGroup)
        {
            DeleteRecords_Private(null, null, dateTime, recordFilterGroup, null);
        }

        public void DeleteRecords(DateTime fromDate, DateTime toDate, List<long> idsToDelete)
        {
            DeleteRecords_Private(fromDate, toDate, null, null, idsToDelete.Cast<object>().ToList());
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }
        public DataRecord GetDataRecord(Object dataRecordId, List<string> fieldNames)
        {
            var dynamicManager = this.DynamicManager;
            DataRecord dataRecord = null;
            SelectRecords(fieldNames, null, null, null, false, null, null, null,dataRecordId,
                (reader) =>
                {
                    if (reader.Read())
                        dataRecord = dynamicManager.GetDataRecordFromReader(reader, fieldNames);
                });
            return dataRecord;
        }
        public List<DataRecord> GetAllDataRecords(List<string> columns)
        {
            List<DataRecord> dataRecords = new List<DataRecord>();
            var dynamicManager = this.DynamicManager;
            SelectRecords(columns, null, null, null, false, null, null, null,null,
                (reader) =>
                {
                    while (reader.Read())
                    {
                        dataRecords.Add(dynamicManager.GetDataRecordFromReader(reader, columns));
                    }
                });
            return dataRecords;
        }

        public void GetDataRecords(DateTime? from, DateTime? to, RecordFilterGroup recordFilterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady, string orderColumnName = null, bool isOrderAscending = false)
        {
            var dynamicManager = this.DynamicManager;
            SelectRecords(null, null, from, to, false, recordFilterGroup, isOrderAscending ? OrderDirection.Ascending : OrderDirection.Descending, orderColumnName,null, (reader) =>
            {
                while (reader.Read())
                {
                    onItemReady(dynamicManager.GetDynamicRecordFromReader(reader));
                    if (shouldStop != null && shouldStop())
                        break;
                }
            });
        }
        
        public int GetDBQueryMaxParameterNumber()
        {
            return new RDBQueryContext(GetDataProvider()).GetDBQueryMaxParameterNumber();
        }

        public List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context)
        {
            List<DataRecord> dataRecords = new List<DataRecord>();
            var dynamicManager = this.DynamicManager;
            SelectRecords(context.FieldNames, context.LimitResult, context.FromTime, context.ToTime, true, context.FilterGroup, context.Direction, null,null,
                (reader) =>
                {
                    while (reader.Read())
                    {
                        dataRecords.Add(dynamicManager.GetDataRecordFromReader(reader, context.FieldNames));
                    }
                });
            return dataRecords;
        }

        public long? GetMaxId(out DateTime? maxDate, out DateTime? minDate)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(rdbRegistrationInfo.RDBTableQuerySource, "rec", null, true);

            var selectAggregates = selectQuery.SelectAggregates();
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, GetIdFieldNameWithValidate(), "MaxId");
            string dateTimeFieldName = GetDateTimeFieldNameWithValidate();
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, dateTimeFieldName, "MaxDate");
            selectAggregates.Aggregate(RDBNonCountAggregateType.MIN, dateTimeFieldName, "MinDate");

            long? maxIdFromReader = null;
            DateTime? maxDateFromReader = null;
            DateTime? minDateFromReader = null;

            queryContext.ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    maxIdFromReader = reader.GetNullableLong("MaxId");
                    maxDateFromReader =  reader.GetNullableDateTime("MaxDate");
                    minDateFromReader = reader.GetNullableDateTime("MinDate");
                }
            }, 0);

            maxDate = maxDateFromReader;
            minDate = minDateFromReader;
            return maxIdFromReader;
        }

        public DateTime? GetMinDateTimeWithMaxIdAfterId(long id, out long? maxId)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(rdbRegistrationInfo.RDBTableQuerySource, "rec", null, true);

            var selectAggregates = selectQuery.SelectAggregates();
            string dateTimeFieldName = GetDateTimeFieldNameWithValidate();
            selectAggregates.Aggregate(RDBNonCountAggregateType.MIN, dateTimeFieldName, "MinDate");
            string idFieldName = GetIdFieldNameWithValidate();
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, idFieldName, "MaxId");

            selectQuery.Where().GreaterThanCondition(idFieldName).Value(id);

            DateTime? minDateFromReader = null;
            long? maxIdFromReader = null;

            queryContext.ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    minDateFromReader = reader.GetNullableDateTime("MinDate");
                    maxIdFromReader = reader.GetNullableLong("MaxId");
                }
            });

            maxId = maxIdFromReader;
            return minDateFromReader;
        }

        public DateTime? GetMinDateTimeWithMaxIdByFilter(RecordFilterGroup filterGroup, out long? maxId)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(rdbRegistrationInfo.RDBTableQuerySource, "rec", null, true);

            var selectAggregates = selectQuery.SelectAggregates();
            string dateTimeFieldName = GetDateTimeFieldNameWithValidate();
            selectAggregates.Aggregate(RDBNonCountAggregateType.MIN, dateTimeFieldName, "MinDate");
            string idFieldName = GetIdFieldNameWithValidate();
            selectAggregates.Aggregate(RDBNonCountAggregateType.MAX, idFieldName, "MaxId");

            var where = selectQuery.Where();

            if (filterGroup != null)
                AddFilterGroupCondition(filterGroup, where);

            DateTime? minDateFromReader = null;
            long? maxIdFromReader = null;

            queryContext.ExecuteReader((reader) =>
            {
                if (reader.Read())
                {
                    minDateFromReader = reader.GetNullableDateTime("MinDate");
                    maxIdFromReader = reader.GetNullableLong("MaxId");
                }
            });

            maxId = maxIdFromReader;
            return minDateFromReader;
        }

        public object InitialiazeStreamForDBApply()
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var streamForBulkInsert = queryContext.StartBulkInsert();
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            streamForBulkInsert.IntoTable(rdbRegistrationInfo.SchemaManager, rdbRegistrationInfo.RDBTableUniqueName, '^', rdbRegistrationInfo.FieldNamesForBulkInsert);
            return streamForBulkInsert;
        }

        public bool Insert(Dictionary<string, object> fieldValues, int? createdUserId, int? modifiedUserId, out object insertedId)
        {
            if (fieldValues == null || fieldValues.Count == 0)
                throw new Exception("fieldValues should not be null or empty.");

            string idFieldName = GetIdFieldNameWithValidate();
            var rdbRegistrationInfo = GetRDBRegistrationInfo();

            Dictionary<string, object> currentStorageFieldValues;
            Dictionary<string, object> parentStorageFieldValues;

            insertedId = null;
            if(_dataRecordStorageSettings.ParentRecordStorageId.HasValue)
            {
                SplitFieldValuesForCurrentAndParentStorages(fieldValues, rdbRegistrationInfo, out currentStorageFieldValues, out parentStorageFieldValues);
                var parentStorageDataManager = GetParentStorageDataManagerWithValidate();
                if (parentStorageDataManager.Insert(parentStorageFieldValues, createdUserId, modifiedUserId, out insertedId))
                {
                    if (insertedId != null)
                    {
                        if (currentStorageFieldValues.ContainsKey(idFieldName))
                            currentStorageFieldValues[idFieldName] = insertedId;
                        else
                            currentStorageFieldValues.Add(idFieldName, insertedId);
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                currentStorageFieldValues = fieldValues;
                parentStorageFieldValues = null;
            }

            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(rdbRegistrationInfo.RDBTableQuerySource);

            if (rdbRegistrationInfo.IsIdIdentity)
                insertQuery.AddSelectGeneratedId();

            SetRDBWriteQueryColumnsFromDict(insertQuery, false, null, currentStorageFieldValues, createdUserId, modifiedUserId, rdbRegistrationInfo);

            if(rdbRegistrationInfo.IsIdIdentity)
            {
                var executeScalarResult = queryContext.ExecuteScalar();
                switch (rdbRegistrationInfo.IdentityColumnType)
                {
                    case IdentityColumnType.Int:
                        var nullableIntId = executeScalarResult.NullableIntValue;
                        if(nullableIntId.HasValue)
                        {
                            insertedId = nullableIntId.Value;
                            return true;
                        }
                        else
                        {
                            insertedId = null;
                            return false;
                        }
                    case IdentityColumnType.Long:
                        var nullableLongId = executeScalarResult.NullableLongValue;
                        if (nullableLongId.HasValue)
                        {
                            insertedId = nullableLongId.Value;
                            return true;
                        }
                        else
                        {
                            insertedId = null;
                            return false;
                        }
                    default: throw new NotSupportedException($"rdbRegistrationInfo.IdentityColumnType '{rdbRegistrationInfo.IdentityColumnType.ToString()}'");
                }
            }
            else
            {
                if(queryContext.ExecuteNonQuery() > 0)
                {
                    if (!_dataRecordStorageSettings.ParentRecordStorageId.HasValue)//insertedId should be set if inserted to parent record storage
                        fieldValues.TryGetValue(idFieldName, out insertedId);
                    return true;
                }
                else
                {
                    insertedId = null;
                    return false;
                }
            }
        }

        public void InsertRecords(IEnumerable<dynamic> records)
        {
            var dbApplyStream = this.InitialiazeStreamForDBApply();
            foreach (var record in records)
            {
                this.WriteRecordToStream(record, dbApplyStream);
            }
            var readyStream = this.FinishDBApplyStream(dbApplyStream);
            this.ApplyStreamToDB(readyStream);
        }
        
        public bool Update(Dictionary<string, object> fieldValues, int? modifiedUserId, RecordFilterGroup filterGroup)
        {
            if (fieldValues == null || fieldValues.Count == 0)
                throw new Exception("fieldValues should not be null or empty.");

            string idFieldName = GetIdFieldNameWithValidate();

            var rdbRegistrationInfo = GetRDBRegistrationInfo();


            Dictionary<string, object> currentStorageFieldValues;
            Dictionary<string, object> parentStorageFieldValues;

            if (_dataRecordStorageSettings.ParentRecordStorageId.HasValue)
            {
                SplitFieldValuesForCurrentAndParentStorages(fieldValues, rdbRegistrationInfo, out currentStorageFieldValues, out parentStorageFieldValues);                
            }
            else
            {
                currentStorageFieldValues = fieldValues;
                parentStorageFieldValues = null;
            }

            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(rdbRegistrationInfo.RDBTableQuerySource);

            if (_dataRecordStorageSettings.ParentRecordStorageId.HasValue)
                AddJoinParentRecordStorage(updateQuery.Join("rec"), "rec");
            
            object idValue;
            if (!fieldValues.TryGetValue(idFieldName, out idValue) || idValue == null)
                throw new NullReferenceException("idValue");

            SetRDBWriteQueryColumnsFromDict(updateQuery, true, idValue, currentStorageFieldValues, null, modifiedUserId, rdbRegistrationInfo);

            var where = updateQuery.Where();
            where.EqualsCondition(idFieldName).ObjectValue(idValue);

            if (filterGroup != null)
                AddFilterGroupCondition(filterGroup, where);

            if(queryContext.ExecuteNonQuery() > 0)
            {
                if (parentStorageFieldValues != null)
                {
                    var parentStorageDataManager = GetParentStorageDataManagerWithValidate();
                    parentStorageDataManager.Update(parentStorageFieldValues, modifiedUserId, null);//no need to pass the FilterGroup because it is already matched on the child entity
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void UpdateRecords(IEnumerable<dynamic> records, List<string> fieldsToJoin, List<string> fieldsToUpdate)
        {
            if (records == null)
                throw new NullReferenceException("records");
            if (fieldsToJoin == null || fieldsToJoin.Count == 0)
                throw new NullReferenceException("fieldsToJoin");
            if (fieldsToUpdate == null || fieldsToUpdate.Count == 0)
                throw new NullReferenceException("fieldsToUpdate");

            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider(), true);
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumnsFromTable(rdbRegistrationInfo.RDBTableUniqueName, rdbRegistrationInfo.SchemaManager);

            var dynamicManager = this.DynamicManager;

            var insertRecordsToTempTableQuery = queryContext.AddInsertMultipleRowsQuery();
            insertRecordsToTempTableQuery.IntoTable(tempTableQuery);

            foreach (var record in records)
            {
                var newRowContext = insertRecordsToTempTableQuery.AddRow();
                dynamicManager.SetRDBInsertColumnsToTempTableFromRecord(record, newRowContext);
            }

            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(rdbRegistrationInfo.RDBTableQuerySource);

            var joinContext = updateQuery.Join("rec");
            var joinCondition = joinContext.Join(tempTableQuery, "updatedTable").On();
            foreach (var fldToJoin in fieldsToJoin)
            {
                if (!rdbRegistrationInfo.NullableFieldNames.Contains(fldToJoin))
                    joinCondition.EqualsCondition(fldToJoin).Column("updatedTable", fldToJoin);
            }

            foreach (var fldtoUpdate in fieldsToUpdate)
            {
                if (!rdbRegistrationInfo.NullableFieldNames.Contains(fldtoUpdate))
                    updateQuery.Column(fldtoUpdate).Column("updatedTable", fldtoUpdate);
            }
            queryContext.ExecuteNonQuery();
        }

        public void WriteRecordToStream(object record, object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            var recordContext = bulkInsertContext.WriteRecord();
            this.DynamicManager.WriteFieldsToRecordStream(record, recordContext);
        }

        #endregion

        #region ISummaryRecordDataManager

        public IEnumerable<dynamic> GetExistingSummaryRecords(DateTime batchStart)
        {
            _summaryTransformationDefinition.ThrowIfNull("_summaryTransformationDefinition");
            _summaryTransformationDefinition.SummaryBatchStartFieldName.ThrowIfNull("_summaryTransformationDefinition.SummaryBatchStartFieldName", _summaryTransformationDefinition.SummaryTransformationDefinitionId);

            var rdbRegistrationInfo = GetRDBRegistrationInfo();

            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();

            selectQuery.From(rdbRegistrationInfo.RDBTableQuerySource, "rec");
            selectQuery.SelectColumns().AllTableColumns("rec");

            selectQuery.Where().EqualsCondition(_summaryTransformationDefinition.SummaryBatchStartFieldName).Value(batchStart);

            var dynamicManager = this.DynamicManager;
            return queryContext.GetItems(dynamicManager.GetDynamicRecordFromReader);
        }

        #endregion

        #region Internal Methods

        internal void CreateRDBRecordStorageTable()
        {
            CreateDBTable(_dataRecordStorageSettings.TableName);
        }

        internal RDBTempStorageInformation CreateTempRDBRecordStorageTable(long processId)
        {
            string tableName = string.Format("{0}_Temp_{1}", _dataRecordStorageSettings.TableName, processId);
            CreateDBTable(tableName);
            return new RDBTempStorageInformation()
            {
                Schema = _dataRecordStorageSettings.TableSchema,
                TableName = tableName
            };
        }

        private void CreateDBTable(string tableName)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(_dataRecordStorageSettings.TableSchema, tableName);
            string idFieldName = this.DataRecordType.Settings.IdField;
            foreach (var col in _dataRecordStorageSettings.Columns)
            {
                createTableQuery.AddColumn(col.FieldName, col.ColumnName, col.DataType, col.Size, col.Precision, null, col.FieldName == idFieldName, col.IsIdentity);
            }
            if (_dataRecordStorageSettings.IncludeQueueItemId)
            {
                createTableQuery.AddColumn("QueueItemId", RDBDataType.BigInt);
            }
            queryContext.ExecuteNonQuery();
        }

        internal void AlterRDBRecordStorageTable(RDBDataRecordStorageSettings existingRecordStorageSettings)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());

            if(_dataRecordStorageSettings.TableSchema != existingRecordStorageSettings.TableSchema || _dataRecordStorageSettings.TableName != existingRecordStorageSettings.TableName)
            {
                var renameTableQuery = queryContext.AddRenameTableQuery();
                renameTableQuery.ExistingDBTableName(existingRecordStorageSettings.TableSchema, existingRecordStorageSettings.TableName);
                renameTableQuery.NewDBTableName(_dataRecordStorageSettings.TableSchema, _dataRecordStorageSettings.TableName);
            }

            RDBAddColumnsQuery addColumnsQuery = null;
            RDBAlterColumnsQuery alterColumnsQuery = null;

            foreach (var column in _dataRecordStorageSettings.Columns)
            {
                var existingMatch = existingRecordStorageSettings.Columns.FirstOrDefault(itm => itm.ColumnName == column.ColumnName);
                if (existingMatch == null)
                {
                    if (addColumnsQuery == null)
                    {
                        addColumnsQuery = queryContext.AddAddColumnsQuery();
                        addColumnsQuery.DBTableName(_dataRecordStorageSettings.TableSchema, _dataRecordStorageSettings.TableName);
                    }
                    addColumnsQuery.AddColumn(column.FieldName, column.ColumnName, column.DataType, column.Size, column.Precision, null, column.IsIdentity);
                }
                else if (column.DataType != existingMatch.DataType 
                    || column.Size != existingMatch.Size 
                    || column.Precision != existingMatch.Precision)
                {
                    if(alterColumnsQuery == null)
                    {
                        alterColumnsQuery = queryContext.AddAlterColumnsQuery();
                        alterColumnsQuery.DBTableName(_dataRecordStorageSettings.TableSchema, _dataRecordStorageSettings.TableName);
                    }
                    alterColumnsQuery.AddColumn(column.FieldName, column.ColumnName, column.DataType, column.Size, column.Precision, null);
                }
            }

            if(_dataRecordStorageSettings.IncludeQueueItemId && !existingRecordStorageSettings.IncludeQueueItemId)
            {
                if (addColumnsQuery == null)
                {
                    addColumnsQuery = queryContext.AddAddColumnsQuery();
                    addColumnsQuery.DBTableName(_dataRecordStorageSettings.TableSchema, _dataRecordStorageSettings.TableName);
                }
                addColumnsQuery.AddColumn("QueueItemId", RDBDataType.BigInt);
            }

            queryContext.ExecuteNonQuery(true);
        }

        internal void DropStorage()
        {
            _rdbTempStorageInformation.ThrowIfNull("_rdbTempStorageInformation");
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var dropTableQuery = queryContext.AddDropTableQuery();
            dropTableQuery.TableName(rdbRegistrationInfo.RDBTableUniqueName, rdbRegistrationInfo.SchemaManager);            
            queryContext.ExecuteNonQuery();
        }

        internal void FillDataRecordStorageFromTempStorage(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup)
        {
            _rdbTempStorageInformation.ThrowIfNull("_rdbTempStorageInformation");
            var tempStorageRDBRegistrationInfo = GetRDBRegistrationInfo();
            var mainStorageRDBRegistrationInfo = GetMainStorageRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());

            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(mainStorageRDBRegistrationInfo.RDBTableQuerySource);
            var deleteWhere = deleteQuery.Where();
            string dateTimeFieldName = GetDateTimeFieldNameWithValidate();
            deleteWhere.GreaterOrEqualCondition(dateTimeFieldName).Value(from);
            deleteWhere.LessThanCondition(dateTimeFieldName).Value(to);

            if (recordFilterGroup != null)
            {
                AddFilterGroupCondition(recordFilterGroup, deleteWhere);
            }

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(mainStorageRDBRegistrationInfo.RDBTableQuerySource);
            var selectFromTempQuery = insertQuery.FromSelect();
            selectFromTempQuery.From(tempStorageRDBRegistrationInfo.RDBTableQuerySource, "tmp", null, true);
            selectFromTempQuery.SelectColumns().AllTableColumns("tmp");

            var selectWhere = selectFromTempQuery.Where();
            selectWhere.GreaterOrEqualCondition(dateTimeFieldName).Value(from);
            selectWhere.LessThanCondition(dateTimeFieldName).Value(to);

            if (recordFilterGroup != null)
            {
                AddFilterGroupCondition(recordFilterGroup, selectWhere);
            }
            queryContext.ExecuteNonQuery(true);
        }

        private string GetDateTimeFieldNameWithValidate()
        {
            _dataRecordStorageSettings.DateTimeField.ThrowIfNull("_dataRecordStorageSettings.DateTimeField", _dataRecordStorage.DataRecordStorageId);
            return _dataRecordStorageSettings.DateTimeField;
        }

        private string GetIdFieldNameWithValidate()
        {
            this.DataRecordType.Settings.IdField.ThrowIfNull("DataRecordType.Settings.IdField", this.DataRecordType.DataRecordTypeId);
            return this.DataRecordType.Settings.IdField;
        }

        internal int GetStorageRowCount()
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectRowsCountQuery = queryContext.AddSelectTableRowsCountQuery();
            selectRowsCountQuery.TableName(rdbRegistrationInfo.RDBTableUniqueName, rdbRegistrationInfo.SchemaManager);
            return queryContext.ExecuteScalar().IntValue;
        }

        internal string GetMainStorageRDBTableName()
        {
            return GetMainStorageRegistrationInfo().RDBTableUniqueName;
        }

        #endregion

        #region Private Methods

        BaseRDBDataProvider GetDataProvider()
        {
            _dataStoreSettings.ThrowIfNull("_dataStoreSettings");
            //_dataStoreSettings.ModuleName.ThrowIfNull("_dataStoreSettings.ModuleName");
            if (!String.IsNullOrEmpty(_dataStoreSettings.ConnectionString))
            {
                return RDBDataProviderFactory.CreateProviderFromConnString(_dataStoreSettings.ModuleName, _dataStoreSettings.ConnectionString);
            }
            else
            {
                return RDBDataProviderFactory.CreateProvider(_dataStoreSettings.ModuleName, _dataStoreSettings.ConnectionStringAppSettingName, _dataStoreSettings.ConnectionStringName);
            }
        }

        private RDBRegistrationInfo GetRDBRegistrationInfo()
        {
            var mainStorageRegistrationInfo = GetMainStorageRegistrationInfo();
            if (_rdbTempStorageInformation == null)
            {
                return mainStorageRegistrationInfo;
            }
            else
            {
                string cacheName = string.Concat("RDBStorageTempStorage_", (_rdbTempStorageInformation.Schema != null ? _rdbTempStorageInformation.Schema : ""), "_", _rdbTempStorageInformation.TableName);
                return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordStorageManager.CacheManager>().GetOrCreateObject(
                    cacheName, new Vanrise.Caching.SlidingWindowCacheExpirationChecker(TimeSpan.FromMinutes(5)),
                    () =>
                    {
                        var schemaManager = new RDBSchemaManager();
                        var mainStorageRDBTableDefinition = mainStorageRegistrationInfo.RDBTableDefinition;
                        var tempStorageRDBTableDefinition = new RDBTableDefinition
                        {
                            DBSchemaName = _rdbTempStorageInformation.Schema,
                            DBTableName = _rdbTempStorageInformation.TableName,
                            Columns = mainStorageRDBTableDefinition.Columns,
                            IdColumnName = mainStorageRDBTableDefinition.IdColumnName,
                            CreatedTimeColumnName = mainStorageRDBTableDefinition.CreatedTimeColumnName,
                            ModifiedTimeColumnName = mainStorageRDBTableDefinition.ModifiedTimeColumnName
                        };
                        schemaManager.RegisterDefaultTableDefinition(mainStorageRegistrationInfo.RDBTableUniqueName, tempStorageRDBTableDefinition);
                        return new RDBRegistrationInfo
                        {
                            SchemaManager = schemaManager,
                            RDBTableUniqueName = mainStorageRegistrationInfo.RDBTableUniqueName,
                            RDBTableDefinition = tempStorageRDBTableDefinition,
                            FieldInfosByFieldName = mainStorageRegistrationInfo.FieldInfosByFieldName,
                            FieldNamesForBulkInsert = mainStorageRegistrationInfo.FieldNamesForBulkInsert,
                            IsIdIdentity = mainStorageRegistrationInfo.IsIdIdentity,
                            IdentityColumnType = mainStorageRegistrationInfo.IdentityColumnType,
                            UniqueFieldNames = mainStorageRegistrationInfo.UniqueFieldNames,
                            NullableFieldNames = mainStorageRegistrationInfo.NullableFieldNames,
                            ParentRDBRegistrationInfo = mainStorageRegistrationInfo.ParentRDBRegistrationInfo
                        };
                    });
            }
        }

        private RDBRegistrationInfo GetMainStorageRegistrationInfo()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordStorageManager.CacheManager>().GetOrCreateObject(
                                string.Concat("RDBRecordStorageDataManager_GetRDBRegistrationInfo_", _dataRecordStorage.DataRecordStorageId),
                                () =>
                                {
                                    var tableSchema = _dataRecordStorageSettings.TableSchema;
                                    var tableName = _dataRecordStorageSettings.TableName;
                                    string rdbTableName = string.Concat(tableSchema, "_", tableName, Guid.NewGuid());

                                    var rdbTableDefinition = new RDBTableDefinition
                                    {
                                        DBSchemaName = tableSchema,
                                        DBTableName = tableName,
                                        Columns = new Dictionary<string, RDBTableColumnDefinition>()
                                    };
                                    List<string> fieldNamesForBulkInsert = new List<string>();
                                    Dictionary<string, RDBStorageFieldInfo> fieldInfosByFieldName = new Dictionary<string, RDBStorageFieldInfo>();
                                    string idFieldName = this.DataRecordType.Settings.IdField;
                                    string createdTimeFieldName = _dataRecordStorageSettings.CreatedTimeField;
                                    string lastModifiedTimeFieldName = _dataRecordStorageSettings.LastModifiedTimeField;
                                    bool isIdIdentity = false;
                                    IdentityColumnType identityColumnType = default(IdentityColumnType);
                                    List<string> uniqueFieldNames = new List<string>();
                                    foreach (var col in _dataRecordStorageSettings.Columns)
                                    {
                                        var rdbColDef = new RDBTableColumnDefinition
                                        {
                                            DBColumnName = col.ColumnName,
                                            DataType = col.DataType,
                                            Size = col.Size,
                                            Precision = col.Precision
                                        };
                                        rdbTableDefinition.Columns.Add(col.FieldName, rdbColDef);

                                        if (!col.IsIdentity)
                                            fieldNamesForBulkInsert.Add(col.FieldName);

                                        if (col.FieldName == idFieldName)
                                            rdbTableDefinition.IdColumnName = col.FieldName;

                                        if (col.FieldName == createdTimeFieldName)
                                            rdbTableDefinition.CreatedTimeColumnName = col.FieldName;

                                        if (col.FieldName == lastModifiedTimeFieldName)
                                            rdbTableDefinition.ModifiedTimeColumnName = col.FieldName;

                                        if (col.IsIdentity)
                                        {
                                            isIdIdentity = true;
                                            switch (col.DataType)
                                            {
                                                case RDBDataType.Int:
                                                    identityColumnType = IdentityColumnType.Int;
                                                    break;
                                                case RDBDataType.BigInt:
                                                    identityColumnType = IdentityColumnType.Long;
                                                    break;
                                                default: throw new NotSupportedException($"col.DataType '{col.DataType.ToString()}'");
                                            }
                                        }

                                        if (col.IsUnique)
                                            uniqueFieldNames.Add(col.FieldName);

                                        DataRecordField field;
                                        if (!this.DataRecordFieldsByName.TryGetValue(col.FieldName, out field))
                                            throw new NullReferenceException($"field '{col.FieldName}'");
                                        fieldInfosByFieldName.Add(col.FieldName, new RDBStorageFieldInfo
                                        {
                                            Column = col,
                                            FieldType = field.Type
                                        });
                                    }

                                    if (_dataRecordStorageSettings.IncludeQueueItemId)
                                    {
                                        string fieldName = "QueueItemId";
                                        var rdbColumnDef = new RDBTableColumnDefinition
                                        {
                                            DBColumnName = fieldName,
                                            DataType = RDBDataType.BigInt
                                        };
                                        rdbTableDefinition.Columns.Add(fieldName, rdbColumnDef);
                                        fieldNamesForBulkInsert.Add(fieldName);
                                    }

                                    RDBRegistrationInfo parentRDBRegistrationInfo;
                                    if (_dataRecordStorageSettings.ParentRecordStorageId.HasValue)
                                    {
                                        var parentRDBDataManager = GetParentStorageDataManagerWithValidate();
                                        parentRDBRegistrationInfo = parentRDBDataManager.GetRDBRegistrationInfo();
                                    }
                                    else
                                    {
                                        parentRDBRegistrationInfo = null;
                                    }
                                    
                                    HashSet<string> nullableFieldNames = _dataRecordStorageSettings.NullableFields != null ? new HashSet<string>(_dataRecordStorageSettings.NullableFields.Select(itm => itm.Name)) : new HashSet<string>();

                                    RDBSchemaManager.Current.RegisterDefaultTableDefinition(rdbTableName, rdbTableDefinition);
                                    RDBRegistrationInfo registrationInfo = new RDBRegistrationInfo
                                    {
                                        SchemaManager = RDBSchemaManager.Current,
                                        RDBTableUniqueName = rdbTableName,
                                        RDBTableDefinition = rdbTableDefinition,
                                        FieldNamesForBulkInsert = fieldNamesForBulkInsert.ToArray(),
                                        FieldInfosByFieldName = fieldInfosByFieldName,
                                        IsIdIdentity = isIdIdentity,
                                        IdentityColumnType = identityColumnType,
                                        UniqueFieldNames = uniqueFieldNames,
                                        NullableFieldNames = nullableFieldNames,
                                        ParentRDBRegistrationInfo = parentRDBRegistrationInfo
                                    };
                                    return registrationInfo;
                                });
        }

        void SelectRecords(List<string> fieldNames, int? numberOfRecords,
            DateTime? fromtime, DateTime? toTime, bool toTimeLessOrEqual, RecordFilterGroup filterGroup,
            OrderDirection? orderDirection, string orderByFieldName,Object dataRecordId,
            Action<IRDBDataReader> onReaderReady)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(rdbRegistrationInfo.RDBTableQuerySource, "rec", numberOfRecords, true);

            if (_dataRecordStorageSettings.ParentRecordStorageId.HasValue)
                AddJoinParentRecordStorage(selectQuery.Join(), "rec");

            var selectColumns = selectQuery.SelectColumns();
            if (fieldNames != null)
            {
                foreach (var fieldName in fieldNames)
                {
                    if(rdbRegistrationInfo.FieldInfosByFieldName.ContainsKey(fieldName))
                    {
                        selectColumns.Column(fieldName);
                    }
                    else if(rdbRegistrationInfo.ParentRDBRegistrationInfo != null && rdbRegistrationInfo.ParentRDBRegistrationInfo.FieldInfosByFieldName.ContainsKey(fieldName))
                    {
                        selectColumns.Column(PARENTRECORDSTORAGE_RDBTABLEALIAS, fieldName, fieldName);
                    }                        
                }
            }
            else
            {
                selectColumns.AllTableColumns("rec");
                if (rdbRegistrationInfo.ParentRDBRegistrationInfo != null)
                {
                    foreach(var parentFieldName in rdbRegistrationInfo.ParentRDBRegistrationInfo.FieldInfosByFieldName.Keys)
                    {
                        if (!rdbRegistrationInfo.FieldInfosByFieldName.ContainsKey(parentFieldName))
                            selectColumns.Column(PARENTRECORDSTORAGE_RDBTABLEALIAS, parentFieldName, parentFieldName);
                    }
                }
            }

            var where = selectQuery.Where();
            if (fromtime.HasValue && fromtime != default(DateTime))
            {
                where.GreaterOrEqualCondition(GetDateTimeFieldNameWithValidate()).Value(fromtime.Value);
            }
            if (toTime.HasValue && toTime != default(DateTime))
            {
                string dateTimeFieldName = GetDateTimeFieldNameWithValidate();
                if (toTimeLessOrEqual)
                    where.LessOrEqualCondition(dateTimeFieldName).Value(toTime.Value);
                else
                    where.LessThanCondition(dateTimeFieldName).Value(toTime.Value);
            }

            if (filterGroup != null)
            {
                AddFilterGroupCondition(filterGroup, where);
            }

            if (orderDirection.HasValue)
            {
                if (orderByFieldName == null)
                    orderByFieldName = GetDateTimeFieldNameWithValidate();
                orderByFieldName.ThrowIfNull("orderByFieldName", _dataRecordStorage.DataRecordStorageId);
                if (!rdbRegistrationInfo.NullableFieldNames.Contains(orderByFieldName))
                    selectQuery.Sort().ByColumn(orderByFieldName, orderDirection.Value == OrderDirection.Ascending ? RDBSortDirection.ASC : RDBSortDirection.DESC);
            }
            if(dataRecordId != null)
            {
                where.EqualsCondition(GetIdFieldNameWithValidate()).ObjectValue(dataRecordId);
            }
                
            queryContext.ExecuteReader(onReaderReady);
        }

        private void AddFilterGroupCondition(RecordFilterGroup filterGroup, RDBConditionContext conditionContext)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var recordFilterRDBBuilder = new RecordFilterRDBBuilder(
                (fieldName, expressionContext) =>
                {
                    if(rdbRegistrationInfo.FieldInfosByFieldName.ContainsKey(fieldName))
                        expressionContext.Column(fieldName);
                    else if (rdbRegistrationInfo.ParentRDBRegistrationInfo != null && rdbRegistrationInfo.ParentRDBRegistrationInfo.FieldInfosByFieldName.ContainsKey(fieldName))
                        expressionContext.Column(PARENTRECORDSTORAGE_RDBTABLEALIAS, fieldName);
                    else if (rdbRegistrationInfo.NullableFieldNames.Contains(fieldName))
                        expressionContext.Null();
                    else if (rdbRegistrationInfo.ParentRDBRegistrationInfo != null && rdbRegistrationInfo.ParentRDBRegistrationInfo.NullableFieldNames.Contains(fieldName))
                        expressionContext.Null();
                    else
                        throw new Exception($"Unsupported Field Name '{fieldName}");
                });
            recordFilterRDBBuilder.RecordFilterGroupCondition(conditionContext, filterGroup);
        }

        int DeleteRecords_Private(DateTime? fromtime, DateTime? toTime, DateTime? time, RecordFilterGroup filterGroup, List<object> ids)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(rdbRegistrationInfo.RDBTableQuerySource);
            var where = deleteQuery.Where();
            if (fromtime.HasValue)
            {
                where.GreaterOrEqualCondition(GetDateTimeFieldNameWithValidate()).Value(fromtime.Value);
            }
            if (toTime.HasValue)
            {
                where.LessThanCondition(GetDateTimeFieldNameWithValidate()).Value(toTime.Value);
            }
            if (time.HasValue)
            {
                where.EqualsCondition(GetDateTimeFieldNameWithValidate()).Value(time.Value);
            }
            if (filterGroup != null)
            {
                AddFilterGroupCondition(filterGroup, where);
            }
            if (ids != null)
            {
                var idsExpressions = new List<BaseRDBExpression>();
                foreach (var id in ids)
                {
                    where.CreateExpressionContext((expression) => idsExpressions.Add(expression)).ObjectValue(id);
                }
                where.ListCondition(GetIdFieldNameWithValidate(), RDBListConditionOperator.IN, idsExpressions);
            }
            return queryContext.ExecuteNonQuery(0);
        }

        void SetRDBWriteQueryColumnsFromDict(Vanrise.Data.RDB.BaseRDBWriteDataQuery writeQuery, bool isUpdateQuery, object idValue, Dictionary<string, object> fieldValues, int? createdUserId, int? modifiedUserId, RDBRegistrationInfo rdbRegistrationInfo)
        {
            var fieldInfos = rdbRegistrationInfo.FieldInfosByFieldName;
            string idFieldName = GetIdFieldNameWithValidate();
            List<WriteQueryUniqueFieldValue> uniqueFieldsValues = new List<WriteQueryUniqueFieldValue>();
            foreach (var fieldValueEntry in fieldValues)
            {
                string fieldName = fieldValueEntry.Key;
                object fieldValue = fieldValueEntry.Value;

                if (rdbRegistrationInfo.NullableFieldNames.Contains(fieldName))
                    continue;

                if (fieldName == idFieldName)
                {
                    if (isUpdateQuery || rdbRegistrationInfo.IsIdIdentity)
                        continue;
                }
                RDBStorageFieldInfo fieldInfo;
                if (!fieldInfos.TryGetValue(fieldName, out fieldInfo))
                    throw new NullReferenceException($"fieldInfo '{fieldName}'");
                SetFieldRDBValue(writeQuery.Column(fieldName), fieldValue, fieldInfo.FieldType, isUpdateQuery);
                if (fieldInfo.Column.IsUnique)
                {
                    uniqueFieldsValues.Add(new WriteQueryUniqueFieldValue
                    {
                        FieldName = fieldName,
                        FieldType = fieldInfo.FieldType,
                        FieldValue = fieldValue
                    });
                }
            }
            if (createdUserId.HasValue)
            {
                string createdUserFieldName = this._dataRecordStorage.Settings.CreatedByField;
                if (!string.IsNullOrEmpty(createdUserFieldName))
                    writeQuery.Column(createdUserFieldName).Value(createdUserId.Value);
            }
            if (modifiedUserId.HasValue)
            {
                string modifiedUserFieldName = this._dataRecordStorage.Settings.LastModifiedByField;
                if (!string.IsNullOrEmpty(modifiedUserFieldName))
                    writeQuery.Column(modifiedUserFieldName).Value(modifiedUserId.Value);
            }
            if (uniqueFieldsValues.Count > 0 && uniqueFieldsValues.Count == rdbRegistrationInfo.UniqueFieldNames.Count)
            {
                var ifNotExistsCondition = writeQuery.IfNotExists("rec");
                if (isUpdateQuery)
                {
                    ifNotExistsCondition.NotEqualsCondition(idFieldName).ObjectValue(idValue);
                }
                foreach (var fieldValueEntry in uniqueFieldsValues)
                {
                    SetFieldRDBValue(ifNotExistsCondition.EqualsCondition(fieldValueEntry.FieldName), fieldValueEntry.FieldValue, fieldValueEntry.FieldType, true);
                }
            }
        }

        private void SetFieldRDBValue(RDBExpressionContext fieldRDBExpressionContext, object fieldValue, DataRecordFieldType fieldType, bool setNullIfValueNull)
        {
            if (fieldValue == null)
            {
                if (setNullIfValueNull)
                    fieldRDBExpressionContext.Null();
            }
            else
            {
                if (fieldType.StoreValueSerialized)
                {
                    var serializeValueContext = new Vanrise.GenericData.Entities.SerializeDataRecordFieldValueContext() { Object = fieldValue };
                    fieldRDBExpressionContext.Value(fieldType.SerializeValue(serializeValueContext));
                }
                else
                {
                    fieldRDBExpressionContext.ObjectValue(fieldValue);
                }
            }
        }

        void AddJoinParentRecordStorage(RDBJoinContext joinContext, string tableAlias)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            rdbRegistrationInfo.ParentRDBRegistrationInfo.ThrowIfNull("rdbRegistrationInfo.ParentRDBRegistrationInfo", _dataRecordStorage.DataRecordStorageId);

            var joinStatement = joinContext.Join(rdbRegistrationInfo.ParentRDBRegistrationInfo.RDBTableQuerySource, PARENTRECORDSTORAGE_RDBTABLEALIAS);
            joinStatement.On().EqualsCondition(tableAlias, GetIdFieldNameWithValidate()).Column(PARENTRECORDSTORAGE_RDBTABLEALIAS, GetParentStorageDataManagerWithValidate().GetIdFieldNameWithValidate());
        }

        private RDBRecordStorageDataManager GetParentStorageDataManagerWithValidate()
        {
            return GetStorageDataManagerWithValidate(_dataRecordStorageSettings.ParentRecordStorageId.Value);
        }

        private RDBRecordStorageDataManager GetStorageDataManagerWithValidate(Guid recordStorageId)
        {
            var recordStorageDataManager = s_dataRecordStorageManager.GetStorageDataManager(recordStorageId);
            recordStorageDataManager.ThrowIfNull("recordStorageDataManager", recordStorageId);
            return recordStorageDataManager.CastWithValidate<RDBRecordStorageDataManager>("recordStorageDataManager", recordStorageId);
        }

        private void SplitFieldValuesForCurrentAndParentStorages(Dictionary<string, object> fieldValues, RDBRegistrationInfo rdbRegistrationInfo, out Dictionary<string, object> currentStorageFieldValues, out Dictionary<string, object> parentStorageFieldValues)
        {
            rdbRegistrationInfo.ParentRDBRegistrationInfo.ThrowIfNull("rdbRegistrationInfo.ParentRDBRegistrationInfo", _dataRecordStorage.DataRecordStorageId);
            currentStorageFieldValues = new Dictionary<string, object>();
            parentStorageFieldValues = new Dictionary<string, object>();
            foreach(var fieldValueEntry in fieldValues)
            {
                if (rdbRegistrationInfo.FieldInfosByFieldName.ContainsKey(fieldValueEntry.Key))
                    currentStorageFieldValues.Add(fieldValueEntry.Key, fieldValueEntry.Value);
                if (rdbRegistrationInfo.ParentRDBRegistrationInfo.FieldInfosByFieldName.ContainsKey(fieldValueEntry.Key))
                    parentStorageFieldValues.Add(fieldValueEntry.Key, fieldValueEntry.Value);
            }
        }

        #endregion

        #region Private Classes

        private class RDBRegistrationInfo
        {
            public RDBSchemaManager SchemaManager { get; set; }

            public string RDBTableUniqueName { get; set; }

            public RDBTableDefinitionQuerySource RDBTableQuerySource
            {
                get
                {
                    return new RDBTableDefinitionQuerySource(this.RDBTableUniqueName, this.SchemaManager);
                }
            }

            public string[] FieldNamesForBulkInsert { get; set; }
            
            public Dictionary<string, RDBStorageFieldInfo> FieldInfosByFieldName { get; set; }

            public RDBTableDefinition RDBTableDefinition { get; set; }
            
            public bool IsIdIdentity { get; set; }

            public IdentityColumnType IdentityColumnType { get; set; }

            public List<string> UniqueFieldNames { get; set; }

            public HashSet<string> NullableFieldNames { get; set; }
            
            public RDBRegistrationInfo ParentRDBRegistrationInfo { get; set; }
        }

        private class RDBStorageFieldInfo
        {
            public RDBDataRecordStorageColumn Column { get; set; }

            public DataRecordFieldType FieldType { get; set; }
        }

        private enum IdentityColumnType { Int = 0, Long = 1 }

        private class WriteQueryUniqueFieldValue
        {
            public string FieldName { get; set; }

            public DataRecordFieldType FieldType { get; set; }

            public Object FieldValue { get; set; }
        }

        #endregion
    }
}