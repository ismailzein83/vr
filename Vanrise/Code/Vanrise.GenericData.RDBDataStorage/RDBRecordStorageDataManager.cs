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
using Vanrise.Entities;

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
            SelectRecords(null, null, from, to, false, recordFilterGroup, isOrderAscending ? OrderDirection.Ascending : OrderDirection.Descending, orderColumnName, (reader) =>
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
            SelectRecords(context.FieldNames, context.LimitResult, context.FromTime, context.ToTime, true, context.FilterGroup, context.Direction, null,
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

            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(rdbRegistrationInfo.RDBTableQuerySource);

            if (rdbRegistrationInfo.IsIdIdentity)
                insertQuery.AddSelectGeneratedId();

            SetRDBWriteQueryColumnsFromDict(insertQuery, false, null, fieldValues, createdUserId, modifiedUserId, rdbRegistrationInfo);

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
                    fieldValues.TryGetValue(GetIdFieldNameWithValidate(), out insertedId);
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
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(rdbRegistrationInfo.RDBTableQuerySource);
            
            string idFieldName = GetIdFieldNameWithValidate();
            object idValue;
            if (!fieldValues.TryGetValue(idFieldName, out idValue) || idValue == null)
                throw new NullReferenceException("idValue");

            SetRDBWriteQueryColumnsFromDict(updateQuery, true, idValue, fieldValues, null, modifiedUserId, rdbRegistrationInfo);

            var where = updateQuery.Where();
            where.EqualsCondition(idFieldName).ObjectValue(idValue);

            if (filterGroup != null)
                AddFilterGroupCondition(filterGroup, where);

            return queryContext.ExecuteNonQuery() > 0;
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
                            NullableFieldNames = mainStorageRegistrationInfo.NullableFieldNames
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
                                        NullableFieldNames = nullableFieldNames
                                    };
                                    return registrationInfo;
                                });
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
                foreach (var fieldName in fieldNames)
                {
                    if (!rdbRegistrationInfo.NullableFieldNames.Contains(fieldName))
                        selectColumns.Column(fieldName);
                }
            }
            else
            {
                selectColumns.AllTableColumns("rec");
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

            queryContext.ExecuteReader(onReaderReady);
        }

        private void AddFilterGroupCondition(RecordFilterGroup filterGroup, RDBConditionContext conditionContext)
        {
            var rdbRegistrationInfo = GetRDBRegistrationInfo();
            var recordFilterRDBBuilder = new RecordFilterRDBBuilder(
                (fieldName, expressionContext) =>
                {
                    if (rdbRegistrationInfo.NullableFieldNames.Contains(fieldName))
                        expressionContext.Null();
                    else
                        expressionContext.Column(fieldName);
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