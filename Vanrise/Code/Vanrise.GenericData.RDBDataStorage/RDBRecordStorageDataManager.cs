using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Data.RDB;

namespace Vanrise.GenericData.RDBDataStorage
{
    public class RDBRecordStorageDataManager : IDataRecordDataManager, ISummaryRecordDataManager
    {
        #region ctor/Fields

        RDBDataStoreSettings _dataStoreSettings;
        RDBDataRecordStorageSettings _dataRecordStorageSettings;
        DataRecordStorage _dataRecordStorage;
        SummaryTransformationDefinition _summaryTransformationDefinition;
        RDBTempStorageInformation _rdbTempStorageInformation;

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
            throw new NotImplementedException();
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

        public void DeleteRecords(DateTime fromDate, DateTime toDate, List<long> idsToDelete, string idFieldName, string dateTimeFieldName)
        {
            throw new NotImplementedException();
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            RDBBulkInsertQueryContext bulkInsertContext = dbApplyStream.CastWithValidate<RDBBulkInsertQueryContext>("dbApplyStream");
            bulkInsertContext.CloseStream();
            return bulkInsertContext;
        }

        public List<DataRecord> GetAllDataRecords(List<string> columns)
        {
            List<DataRecord> dataRecords = new List<DataRecord>();
            var dynamicManager = this.DynamicManager;
            SelectRecords(columns, null, null, null, false, null, null, null,
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
            SelectRecords(null, null, from, to, false, recordFilterGroup, isOrderAscending ? OrderDirection.Ascending : OrderDirection.Descending, orderColumnName,
                (reader) =>
                {
                    while(reader.Read())
                    {
                        onItemReady(dynamicManager.GetDynamicRecordFromReader(reader));
                        if (shouldStop())
                            break;
                    }
                });
        }

        public int GetDBQueryMaxParameterNumber()
        {
            throw new NotImplementedException();
        }

        public List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context)
        {
            List<DataRecord> dataRecords = new List<DataRecord>();
            var dynamicManager = this.DynamicManager;
            SelectRecords(context.FieldNames, context.LimitResult, context.FromTime, context.ToTime, true, context.FilterGroup, context.Direction, null,
                (reader) =>
                {
                    while(reader.Read())
                    {
                        dataRecords.Add(dynamicManager.GetDataRecordFromReader(reader, context.FieldNames));
                    }
                });
            return dataRecords;
        }

        public long? GetMaxId(string idFieldName, string dateTimeFieldName, out DateTime? maxDate, out DateTime? minDate)
        {
            throw new NotImplementedException();
        }

        public DateTime? GetMinDateTimeWithMaxIdAfterId(long id, string idFieldName, string dateTimeFieldName, out long? maxId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            var queryContext = new RDBQueryContext(GetDataProvider());
            var tempTableQuery = queryContext.CreateTempTable();
            tempTableQuery.AddColumnsFromTable(rdbRegistrationInfo.RDBTableUniqueName);

            var dynamicManager = this.DynamicManager;

            var insertRecordsToTempTableQuery = queryContext.AddInsertMultipleRowsQuery();
            insertRecordsToTempTableQuery.IntoTable(tempTableQuery);

            foreach(var record in records)
            {
                var newRowContext = insertRecordsToTempTableQuery.AddRow();
                dynamicManager.SetRDBInsertColumnsToTempTableFromRecord(record, newRowContext);
            }
            
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(rdbRegistrationInfo.RDBTableQuerySource);

            var joinContext = updateQuery.Join("rec");
            var joinCondition = joinContext.Join(tempTableQuery, "updatedTable").On();
            foreach(var fldToJoin in fieldsToJoin)
            {
                joinCondition.EqualsCondition(fldToJoin).Column("updatedTable", fldToJoin);
            }
            
            foreach(var fldtoUpdate in fieldsToUpdate)
            {
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

        internal RDBTempStorageInformation CreateTempRDBRecordStorageTable(long processId)
        {
            string tableName = string.Format("{0}_Temp_{1}", _dataRecordStorageSettings.TableName, processId);
            var queryContext = new RDBQueryContext(GetDataProvider());
            var createTableQuery = queryContext.AddCreateTableQuery();
            createTableQuery.DBTableName(_dataRecordStorageSettings.TableSchema, tableName);
            foreach (var col in _dataRecordStorageSettings.Columns)
            {
                createTableQuery.AddColumn(col.FieldName, col.ColumnName, col.DataType, col.Size, col.Precision, false, false, false);
            }
            if (_dataRecordStorageSettings.IncludeQueueItemId)
            {
                createTableQuery.AddColumn("QueueItemId", RDBDataType.BigInt);
            }
            queryContext.ExecuteNonQuery();

            return new RDBTempStorageInformation()
            {
                Schema = _dataRecordStorageSettings.TableSchema,
                TableName = tableName
            };
        }

        internal void DropStorage()
        {
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
            _dataRecordStorageSettings.DateTimeField.ThrowIfNull("_dataRecordStorageSettings.CreatedTimeField", _dataRecordStorage.DataRecordStorageId);
            deleteWhere.GreaterOrEqualCondition(_dataRecordStorageSettings.DateTimeField).Value(from);
            deleteWhere.LessThanCondition(_dataRecordStorageSettings.DateTimeField).Value(to);

            if (recordFilterGroup != null)
            {
                var recordFilterRDBBuilder = new RecordFilterRDBBuilder((fieldName, expressionContext) => expressionContext.Column(fieldName));
                recordFilterRDBBuilder.RecordFilterGroupCondition(deleteWhere, recordFilterGroup);
            }

            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(mainStorageRDBRegistrationInfo.RDBTableQuerySource);
            var selectFromTempQuery = insertQuery.FromSelect();
            selectFromTempQuery.From(tempStorageRDBRegistrationInfo.RDBTableQuerySource, "tmp");
            selectFromTempQuery.SelectColumns().AllTableColumns("tmp");

            var selectWhere = selectFromTempQuery.Where();
            selectWhere.GreaterOrEqualCondition(_dataRecordStorageSettings.DateTimeField).Value(from);
            selectWhere.LessThanCondition(_dataRecordStorageSettings.DateTimeField).Value(to);

            if (recordFilterGroup != null)
            {
                var recordFilterRDBBuilder = new RecordFilterRDBBuilder((fieldName, expressionContext) => expressionContext.Column(fieldName));
                recordFilterRDBBuilder.RecordFilterGroupCondition(selectWhere, recordFilterGroup);
            }
            queryContext.ExecuteNonQuery(true);
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
                    ColumnsByFieldName = mainStorageRegistrationInfo.ColumnsByFieldName,
                    FieldNamesForBulkInsert = mainStorageRegistrationInfo.FieldNamesForBulkInsert
                };
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
                                    Dictionary<string, RDBDataRecordStorageColumn> columnsByFieldName = new Dictionary<string, RDBDataRecordStorageColumn>();
                                    string idFieldName = this.DataRecordType.Settings.IdField;
                                    string createdTimeFieldName = _dataRecordStorageSettings.CreatedTimeField;
                                    string lastModifiedTimeFieldName = _dataRecordStorageSettings.LastModifiedTimeField;

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
                                        fieldNamesForBulkInsert.Add(col.FieldName);
                                        columnsByFieldName.Add(col.FieldName, col);
                                        if (col.FieldName == idFieldName)
                                            rdbTableDefinition.IdColumnName = col.FieldName;
                                        if (col.FieldName == createdTimeFieldName)
                                            rdbTableDefinition.CreatedTimeColumnName = col.FieldName;
                                        if (col.FieldName == lastModifiedTimeFieldName)
                                            rdbTableDefinition.ModifiedTimeColumnName = col.FieldName;
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

                                    RDBSchemaManager.Current.RegisterDefaultTableDefinition(rdbTableName, rdbTableDefinition);
                                    RDBRegistrationInfo registrationInfo = new RDBRegistrationInfo
                                    {
                                        SchemaManager = RDBSchemaManager.Current,
                                        RDBTableUniqueName = rdbTableName,
                                        RDBTableDefinition = rdbTableDefinition,
                                        FieldNamesForBulkInsert = fieldNamesForBulkInsert.ToArray(),
                                        ColumnsByFieldName = columnsByFieldName
                                    };
                                    return registrationInfo;
                                });
        }
        
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

            public Dictionary<string, RDBDataRecordStorageColumn> ColumnsByFieldName { get; set; }

            public RDBTableDefinition RDBTableDefinition { get; set; }
        }

        void SelectRecords(List<string> fieldNames, int? numberOfRecords, 
            DateTime? fromtime, DateTime? toTime, bool toTimeLessOrEqual, RecordFilterGroup filterGroup, 
            OrderDirection? orderDirection, string orderByFieldName,
            Action<IRDBDataReader> onReaderReady)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(rdbRegistrationInfo.RDBTableQuerySource, "rec", numberOfRecords, true);
            var selectColumns = selectQuery.SelectColumns();
            if (fieldNames != null)
            {                
                foreach(var fieldName in fieldNames)
                {
                    selectColumns.Column(fieldName);
                }
            }
            else
            {
                selectColumns.AllTableColumns("rec");
            }

            var where = selectQuery.Where();
            if (fromtime.HasValue)
            {
                _dataRecordStorageSettings.DateTimeField.ThrowIfNull("_dataRecordStorageSettings.CreatedTimeField", _dataRecordStorage.DataRecordStorageId);
                where.GreaterOrEqualCondition(_dataRecordStorageSettings.DateTimeField).Value(fromtime.Value);
            }
            if(toTime.HasValue)
            {
                _dataRecordStorageSettings.DateTimeField.ThrowIfNull("_dataRecordStorageSettings.CreatedTimeField", _dataRecordStorage.DataRecordStorageId);
                if (toTimeLessOrEqual)
                    where.LessOrEqualCondition(_dataRecordStorageSettings.DateTimeField).Value(toTime.Value);
                else
                    where.LessThanCondition(_dataRecordStorageSettings.DateTimeField).Value(toTime.Value);
            }

            if(filterGroup != null)
            {
                var recordFilterRDBBuilder = new RecordFilterRDBBuilder((fieldName, expressionContext) => expressionContext.Column(fieldName));
                recordFilterRDBBuilder.RecordFilterGroupCondition(where, filterGroup);
            }

            if(orderDirection.HasValue)
            {
                if (orderByFieldName == null)
                    orderByFieldName = _dataRecordStorageSettings.DateTimeField;
                orderByFieldName.ThrowIfNull("orderByFieldName", _dataRecordStorage.DataRecordStorageId);
                selectQuery.Sort().ByColumn(orderByFieldName, orderDirection.Value == OrderDirection.Ascending ? RDBSortDirection.ASC : RDBSortDirection.DESC);
            }

            queryContext.ExecuteReader(onReaderReady);
        }

        int DeleteRecords_Private(DateTime? fromtime, DateTime? toTime, DateTime? time, RecordFilterGroup filterGroup, List<object> ids)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var deleteQuery = queryContext.AddDeleteQuery();
            deleteQuery.FromTable(rdbRegistrationInfo.RDBTableQuerySource);
            var where = deleteQuery.Where();
            if(fromtime.HasValue)
            {
                _dataRecordStorageSettings.DateTimeField.ThrowIfNull("_dataRecordStorageSettings.CreatedTimeField", _dataRecordStorage.DataRecordStorageId);
                where.GreaterOrEqualCondition(_dataRecordStorageSettings.DateTimeField).Value(fromtime.Value);
            }
            if (toTime.HasValue)
            {
                _dataRecordStorageSettings.DateTimeField.ThrowIfNull("_dataRecordStorageSettings.CreatedTimeField", _dataRecordStorage.DataRecordStorageId);
                where.LessThanCondition(_dataRecordStorageSettings.DateTimeField).Value(toTime.Value);
            }
            if(time.HasValue)
            {
                _dataRecordStorageSettings.DateTimeField.ThrowIfNull("_dataRecordStorageSettings.CreatedTimeField", _dataRecordStorage.DataRecordStorageId);
                where.EqualsCondition(_dataRecordStorageSettings.DateTimeField).Value(time.Value);
            }
            if (filterGroup != null)
            {
                var recordFilterRDBBuilder = new RecordFilterRDBBuilder((fieldName, expressionContext) => expressionContext.Column(fieldName));
                recordFilterRDBBuilder.RecordFilterGroupCondition(where, filterGroup);
            }
            if(ids != null)
            {
                this.DataRecordType.Settings.IdField.ThrowIfNull("this.DataRecordType.Settings.IdField", this.DataRecordType.DataRecordTypeId);
                var idsExpressions = new List<BaseRDBExpression>();
                foreach (var id in ids)
                {
                    where.CreateExpressionContext((expression) => idsExpressions.Add(expression)).ObjectValue(id);
                }
                where.ListCondition(this.DataRecordType.Settings.IdField, RDBListConditionOperator.IN, idsExpressions);
            }
            return queryContext.ExecuteNonQuery();
        }

        #endregion
    }
}
