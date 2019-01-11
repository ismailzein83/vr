using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.RDB;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using System.IO;
using System.Configuration;
using System.Collections.Concurrent;
using System.Threading;

namespace Vanrise.GenericData.FileDataStorage
{
    public class FileRecordStorageDataManager : IDataRecordDataManager
    {
        #region RDB Metadata Columns

        const string COL_ID = "ID";
        const string COL_RuntimeNodeID = "RuntimeNodeID";
        const string COL_ParentFolderRelativePath = "ParentFolderRelativePath";
        const string COL_FileNames = "FileNames";
        const string COL_FromTime = "FromTime";
        const string COL_ToTime = "ToTime";
        const string COL_NbOfRecords = "NbOfRecords";
        const string COL_MinTime = "MinTime";
        const string COL_MaxTime = "MaxTime";
        const string COL_MinID = "MinID";
        const string COL_MaxID = "MaxID";
        const string COL_IsReady = "IsReady";
        const string COL_CreatedTime = "CreatedTime";
        const string COL_LastModifiedTime = "LastModifiedTime";

        #endregion

        #region ctor/Fields

        static TimeSpan s_transactionLockMaxRetryInterval;
        static FileRecordStorageDataManager()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["FileRecordStorage_TransactionLockMaxRetryInterval"], out s_transactionLockMaxRetryInterval))
                s_transactionLockMaxRetryInterval = TimeSpan.FromMinutes(10);
        }

        FileDataStoreSettings _dataStoreSettings;
        FileDataRecordStorageSettings _dataRecordStorageSettings;
        DataRecordStorage _dataRecordStorage;

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
                    _dynamicManager = dynamicTypeGenerator.GetDynamicManager(_dataRecordStorage, _dataRecordStorageSettings, GetPreparedConfig());
                    if (_dynamicManager == null)
                        throw new NullReferenceException("_dynamicManager");
                }
                return _dynamicManager;
            }
        }

        static RecordFilterManager s_recordFilterManager = new RecordFilterManager();

        internal FileRecordStorageDataManager(FileDataStoreSettings dataStoreSettings, FileDataRecordStorageSettings dataRecordStorageSettings, DataRecordStorage dataRecordStorage)
        {
            this._dataStoreSettings = dataStoreSettings;
            this._dataRecordStorageSettings = dataRecordStorageSettings;
            this._dataRecordStorage = dataRecordStorage;
        }

        #endregion

        #region IDataRecordDataManager

        public object InitialiazeStreamForDBApply()
        {
            return new FileRecordStorageBatches();
        }

        public void WriteRecordToStream(object record, object dbApplyStream)
        {
            var batches = dbApplyStream.CastWithValidate<FileRecordStorageBatches>("dbApplyStream");
            var recordInfo = new FileRecordStorageRecordInfo();
            this.DynamicManager.FillRecordInfoFromDynamicRecord(recordInfo, record);
            DateTime batchStartTime;
            DateTime batchEndTime;
            GetBatchTimesFromRecordTime(recordInfo.RecordTime, out batchStartTime, out batchEndTime);
            var matchBatch = batches.Batches.GetOrCreateItem(batchStartTime, () => new FileRecordStorageBatchInfo { FromTime = batchStartTime, ToTime = batchEndTime });
            matchBatch.Records.Add(recordInfo);
            batches.RecordCount++;
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            return dbApplyStream;
        }

        public void ApplyStreamToDB(object stream)
        {
            var batches = stream.CastWithValidate<FileRecordStorageBatches>("stream");
            var preparedConfig = GetPreparedConfig();
            if (preparedConfig.GenerateRecordId)
                GenerateRecordIds(batches);

            foreach (var batch in batches.Batches.Values)
            {
                string parentFolderRelativePath;
                string parentFolderPath = GetParentFolderPath(batch.FromTime, true, out parentFolderRelativePath);

                long? existingFileMetadataId = GetMatchFileMetadataId(batch.FromTime, batch.ToTime);

                long fileMetadataId;

                if (existingFileMetadataId.HasValue)
                {
                    fileMetadataId = existingFileMetadataId.Value;
                }
                else
                {                    
                    fileMetadataId = InsertFileMetadataRecord(preparedConfig, batch, parentFolderRelativePath);
                }

                string fileName = BuildFileName(fileMetadataId, batch.FromTime);

                DateTime minRecordTime, maxRecordTime;
                long minRecordId, maxRecordId;
                SaveRecordsToFile(fileMetadataId, batch.Records, parentFolderPath, fileName,
                    out minRecordTime, out maxRecordTime, out minRecordId, out maxRecordId);

                bool isExistingFileMetadataUpdated = false;
                if (existingFileMetadataId.HasValue)
                {
                    var queryContext = new RDBQueryContext(GetDataProvider());
                    var updateQuery = queryContext.AddUpdateQuery();
                    updateQuery.FromTable(preparedConfig.MetadataRDBTableQuerySource);

                    var fileNamesExp = updateQuery.Column(COL_FileNames).TextConcatenation();
                    fileNamesExp.Expression1().Column(COL_FileNames);
                    fileNamesExp.Expression2().Value(string.Concat(",", fileName));

                    var nbOfRecordsExp = updateQuery.Column(COL_NbOfRecords).ArithmeticExpression(RDBArithmeticExpressionOperator.Add);
                    nbOfRecordsExp.Expression1().Column(COL_NbOfRecords);
                    nbOfRecordsExp.Expression2().Value(batch.Records.Count);

                    var minTimeExp = updateQuery.Column(COL_MinTime).CaseExpression();
                    var minTimeCase1 = minTimeExp.AddCase();
                    minTimeCase1.When().LessOrEqualCondition(COL_MinTime).Value(minRecordTime);
                    minTimeCase1.Then().Column(COL_MinTime);
                    minTimeExp.Else().Value(minRecordTime);

                    var maxTimeExp = updateQuery.Column(COL_MaxTime).CaseExpression();
                    var maxTimeCase1 = maxTimeExp.AddCase();
                    maxTimeCase1.When().GreaterOrEqualCondition(COL_MaxTime).Value(maxRecordTime);
                    maxTimeCase1.Then().Column(COL_MaxTime);
                    maxTimeExp.Else().Value(maxRecordTime);

                    var minIdExp = updateQuery.Column(COL_MinID).CaseExpression();
                    var minIdCase1 = minIdExp.AddCase();
                    minIdCase1.When().LessOrEqualCondition(COL_MinID).Value(minRecordId);
                    minIdCase1.Then().Column(COL_MinID);
                    minIdExp.Else().Value(minRecordId);

                    var maxIdExp = updateQuery.Column(COL_MaxID).CaseExpression();
                    var maxIdCase1 = maxIdExp.AddCase();
                    maxIdCase1.When().GreaterOrEqualCondition(COL_MaxID).Value(maxRecordId);
                    maxIdCase1.Then().Column(COL_MaxID);
                    maxIdExp.Else().Value(maxRecordId);

                    updateQuery.Where().EqualsCondition(COL_ID).Value(fileMetadataId);

                    LockFileToChange(fileMetadataId,
                        () =>
                        {                            
                            isExistingFileMetadataUpdated = queryContext.ExecuteNonQuery() > 0;
                        });
                }

                if (!existingFileMetadataId.HasValue || !isExistingFileMetadataUpdated)
                {                   
                    if(existingFileMetadataId.HasValue)
                    {
                        fileMetadataId = InsertFileMetadataRecord(preparedConfig, batch, parentFolderRelativePath);
                        string oldFileName = fileName;
                        fileName = BuildFileName(fileMetadataId, batch.FromTime);
                        File.Move(Path.Combine(parentFolderPath, oldFileName), Path.Combine(parentFolderPath, fileName));
                    }
                    UpdateFileMetadata(fileMetadataId, fileName, batch.Records.Count, minRecordTime, maxRecordTime, minRecordId, maxRecordId);
                }
            }
        }

        private long InsertFileMetadataRecord(FileRecordStoragePreparedConfig preparedConfig, FileRecordStorageBatchInfo batch, string parentFolderRelativePath)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var insertQuery = queryContext.AddInsertQuery();
            insertQuery.IntoTable(preparedConfig.MetadataRDBTableQuerySource);
            insertQuery.AddSelectGeneratedId();
            insertQuery.Column(COL_RuntimeNodeID).Value(Vanrise.Runtime.RuntimeHost.Current.RuntimeNodeId);
            insertQuery.Column(COL_ParentFolderRelativePath).Value(parentFolderRelativePath);
            insertQuery.Column(COL_FromTime).Value(batch.FromTime);
            insertQuery.Column(COL_ToTime).Value(batch.ToTime);
            insertQuery.Column(COL_IsReady).Value(false);
            var fileMetadataId = queryContext.ExecuteScalar().LongValue;
            return fileMetadataId;
        }

        public List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context)
        {
            throw new NotImplementedException();
        }

        public void GetDataRecords(DateTime? from, DateTime? to, RecordFilterGroup recordFilterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady, string orderColumnName = null, bool isOrderAscending = false)
        {
            //FillTimeFilterFromFilterGroupIfAny(recordFilterGroup, ref from, ref to);
            bool isOrderByDate = orderColumnName == this.DataRecordType.Settings.DateTimeField;

            List<FileRecordStorageFileMetadata> fileMetadatas = GetFileMetadatas(from, to, isOrderAscending, null);

            var dynamicManager = this.DynamicManager;
            List<FileRecordStorageFileMetadata> previousFileMetadatas = new List<FileRecordStorageFileMetadata>();
            List<FileRecordStorageRecord> currentRecords;
            if (!string.IsNullOrWhiteSpace(orderColumnName))
                currentRecords = new List<FileRecordStorageRecord>();
            else
                currentRecords = null;
            foreach (var fileMetadata in fileMetadatas)
            {
                if (shouldStop())
                    break;

                if (isOrderByDate)
                {
                    if (previousFileMetadatas.Count > 0 && !previousFileMetadatas.Any(prevFile => AreTimePeriodsOverlapped(fileMetadata.MinTime, fileMetadata.MaxTime, prevFile.MinTime, prevFile.MaxTime)))
                    {
                        //if (previousFileMetadatas.Count > 1)
                            currentRecords = isOrderAscending ? currentRecords.OrderBy(rec => rec.RecordTime).ToList() : currentRecords.OrderByDescending(rec => rec.RecordTime).ToList();
                        foreach (var record in currentRecords)
                        {
                            if (shouldStop())
                                break;
                            onItemReady(record.Record);
                        }
                        currentRecords = new List<FileRecordStorageRecord>();
                        previousFileMetadatas = new List<FileRecordStorageFileMetadata>();
                    }
                }

                bool filterFromTime = from.HasValue && from.Value > fileMetadata.FromTime;
                bool filterToTime = to.HasValue && to.Value < fileMetadata.ToTime;

                string[] fileNames;
                List<string> fileFullPaths;
                var recordInfos = ReadRecordInfosFromFileWithLock(fileMetadata.FileRecordStorageFileMetadataId, fileMetadata.ParentFolderRelativePath, out fileNames, out fileFullPaths);
                if (!isOrderAscending)
                    recordInfos.Reverse();

                foreach (var recordInfo in recordInfos)
                {
                    if (shouldStop())
                        break;
                    if (filterFromTime)
                    {
                        if (recordInfo.RecordTime < from.Value)
                        {
                            //if (isOrderAscending)
                                continue;
                            //else
                            //    break;
                        }

                    }
                    if (filterToTime)
                    {
                        if (recordInfo.RecordTime >= to.Value)
                        {
                            //if (isOrderAscending)
                            //    break;
                            //else
                                continue;
                        }
                    }
                    dynamic record = dynamicManager.GetDynamicRecordFromRecordInfo(recordInfo);

                    if (recordFilterGroup != null)
                    {
                        var recordIsFilterGroupMatchContext = new DataRecordFilterGenericFieldMatchContext(record, this.DataRecordType.DataRecordTypeId);
                        if (!s_recordFilterManager.IsFilterGroupMatch(recordFilterGroup, recordIsFilterGroupMatchContext))
                            continue;
                    }
                    if (currentRecords != null)
                        currentRecords.Add(new FileRecordStorageRecord(recordInfo, record));
                    else
                        onItemReady(record);
                }

                previousFileMetadatas.Add(fileMetadata);
            }

            if (currentRecords != null && currentRecords.Count > 0)
            {
                if (isOrderByDate)
                {
                    //if (previousFileMetadatas.Count > 1)
                        currentRecords = isOrderAscending ? currentRecords.OrderBy(rec => rec.RecordTime).ToList() : currentRecords.OrderByDescending(rec => rec.RecordTime).ToList();
                }
                else
                {
                    Func<FileRecordStorageRecord, dynamic> orderDelegate = (rec) => new DataRecordObject(this.DataRecordType.DataRecordTypeId, rec.Record).GetFieldValue(orderColumnName);
                    currentRecords = isOrderAscending ? currentRecords.OrderBy(orderDelegate).ToList() : currentRecords.OrderByDescending(orderDelegate).ToList();
                }
                foreach (var record in currentRecords)
                {
                    if (shouldStop())
                        break;
                    onItemReady(record.Record);
                }
            }
        }

        private bool AreTimePeriodsOverlapped(DateTime minTime1, DateTime maxTime1, DateTime minTime2, DateTime maxTime2)
        {
            return minTime1 <= maxTime2 && maxTime1 >= minTime2;
        }

        //private void FillTimeFilterFromFilterGroupIfAny(RecordFilterGroup recordFilterGroup, ref DateTime? from, ref DateTime? to)
        //{
        //    if (from.HasValue && to.HasValue)
        //        return;
        //    if (recordFilterGroup == null || recordFilterGroup.Filters == null || recordFilterGroup.LogicalOperator == RecordQueryLogicalOperator.Or)
        //        return;
        //    foreach(var filter in recordFilterGroup.Filters)
        //    {
        //        var dateFilter = filter as DateTimeRecordFilter;
        //        if (dateFilter != null)
        //        {
        //            if (filter.FieldName == this.DataRecordType.Settings.DateTimeField)
        //            {
        //                if(dateFilter.CompareOperator == DateTimeRecordFilterOperator.)
        //            }
        //        }
        //}

        private static void AddFileMetadataReadyCondition(RDBConditionContext where)
        {
            where.EqualsCondition(COL_IsReady).Value(true);
        }

        public List<DataRecord> GetAllDataRecords(List<string> columns)
        {
            throw new NotImplementedException();
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

        public bool Delete(List<object> recordFieldIds)
        {
            throw new NotImplementedException();
        }

        public void UpdateRecords(IEnumerable<dynamic> records, List<string> fieldsToJoin, List<string> fieldsToUpdate)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecords(DateTime from, DateTime to, RecordFilterGroup recordFilterGroup)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecords(DateTime dateTime, RecordFilterGroup recordFilterGroup)
        {
            throw new NotImplementedException();
        }

        public bool AreDataRecordsUpdated(ref object updateHandle)
        {
            throw new NotImplementedException();
        }

        public int GetDBQueryMaxParameterNumber()
        {
            return 10000000;
        }

        public DateTime? GetMinDateTimeWithMaxIdAfterId(long id, string idFieldName, string dateTimeFieldName, out long? maxId)
        {
            if (idFieldName != this.DataRecordType.Settings.IdField)
                throw new NotSupportedException($"idFieldName '{idFieldName}' is different than record type Id Field '{this.DataRecordType.Settings.IdField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");
            if (dateTimeFieldName != this.DataRecordType.Settings.DateTimeField)
                throw new NotSupportedException($"dateTimeFieldName '{dateTimeFieldName}' is different than record type DateTime Field '{this.DataRecordType.Settings.DateTimeField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");
            
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);

            var selectAggs = selectQuery.SelectAggregates();
            selectAggs.Aggregate(RDBNonCountAggregateType.MAX, COL_MaxID, "MaxId");
            selectAggs.Aggregate(RDBNonCountAggregateType.MIN, COL_MinTime, "MinDate");

            var where = selectQuery.Where();
            where.GreaterThanCondition(COL_MaxID).Value(id);
            AddFileMetadataReadyCondition(where);

            long? maxId_local = null;
            DateTime? minDate = null;
            queryContext.ExecuteReader(reader =>
            {
                if (reader.Read())
                {
                    maxId_local = reader.GetNullableLong("MaxId");
                    minDate = reader.GetNullableDateTime("MinDate");
                }
            });

            maxId = maxId_local;
            return minDate;
        }

        public void DeleteRecords(DateTime fromDate, DateTime toDate, List<long> idsToDelete, string idFieldName, string dateTimeFieldName)
        {
            if (idFieldName != this.DataRecordType.Settings.IdField)
                throw new NotSupportedException($"idFieldName '{idFieldName}' is different than record type Id Field '{this.DataRecordType.Settings.IdField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");
            if (dateTimeFieldName != this.DataRecordType.Settings.DateTimeField)
                throw new NotSupportedException($"dateTimeFieldName '{dateTimeFieldName}' is different than record type DateTime Field '{this.DataRecordType.Settings.DateTimeField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");

            List<long> remainingIdsToDelete = new List<long>(idsToDelete);
            long maxIdToDelete = remainingIdsToDelete.Max();
            long minIdToDelete = remainingIdsToDelete.Min();

            List<FileRecordStorageFileMetadata> fileMetadatas = GetFileMetadatas(fromDate, toDate, null,
                (where) =>
                {
                    where.GreaterOrEqualCondition(COL_MaxID).Value(minIdToDelete);
                    where.LessOrEqualCondition(COL_MinID).Value(maxIdToDelete);
                });

            foreach(var fileMetadata in fileMetadatas)
            {                
                HashSet<long> fileIdsToDelete = new HashSet<long>(remainingIdsToDelete.Where(id => id >= fileMetadata.MinId && id <= fileMetadata.MaxId));
                if(fileIdsToDelete.Count > 0)
                {
                    ChangeFileRecordsWithLock(fileMetadata.FileRecordStorageFileMetadataId, fileMetadata.ParentFolderRelativePath, fileMetadata.FromTime,
                        (existingRecordInfos) =>
                        {
                            List<FileRecordStorageRecordInfo> changedRecordInfos = new List<FileRecordStorageRecordInfo>();
                            foreach (var recordInfo in existingRecordInfos)
                            {
                                if (!fileIdsToDelete.Contains(recordInfo.RecordId))
                                    changedRecordInfos.Add(recordInfo);
                                else
                                    remainingIdsToDelete.Remove(recordInfo.RecordId);
                            }
                            return changedRecordInfos;
                        }, true);
                }
                if (remainingIdsToDelete.Count == 0)
                    break;
            }

        }

        public long? GetMaxId(string idFieldName, string dateTimeFieldName, out DateTime? maxDate, out DateTime? minDate)
        {
            if (idFieldName != this.DataRecordType.Settings.IdField)
                throw new NotSupportedException($"idFieldName '{idFieldName}' is different than record type Id Field '{this.DataRecordType.Settings.IdField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");
            if (dateTimeFieldName != this.DataRecordType.Settings.DateTimeField)
                throw new NotSupportedException($"dateTimeFieldName '{dateTimeFieldName}' is different than record type DateTime Field '{this.DataRecordType.Settings.DateTimeField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
            var selectAggs = selectQuery.SelectAggregates();
            selectAggs.Aggregate(RDBNonCountAggregateType.MAX, COL_MaxID, "MaxId");
            selectAggs.Aggregate(RDBNonCountAggregateType.MAX, COL_MaxTime, "MaxDate");
            selectAggs.Aggregate(RDBNonCountAggregateType.MIN, COL_MinTime, "MinDate");
            long? maxId = null;
            DateTime? maxDate_Local = null;
            DateTime? minDate_Local = null;
            queryContext.ExecuteReader(reader =>
            {
                if(reader.Read())
                {
                    maxId = reader.GetNullableLong("MaxId");
                    maxDate_Local = reader.GetNullableDateTime("MaxDate");
                    minDate_Local = reader.GetNullableDateTime("MinDate");
                }
            });
            maxDate = maxDate_Local;
            minDate = minDate_Local;
            return maxId;
        }

        #endregion

        #region Private Methods

        private void LockReadFile(long fileRecordStorageFileMetadataId, Action onFileLocked)
        {
            Lock($"FileRecordStorage_ReadFile_{fileRecordStorageFileMetadataId}", onFileLocked);
        }

        void Lock(string transactionUniqueName, Action action)
        {
            transactionUniqueName = string.Concat(transactionUniqueName, "_", this._dataRecordStorage.DataRecordStorageId);
            bool lockSucceeded = false;
            DateTime startTime = DateTime.Now;
            while (!lockSucceeded)
            {
                Vanrise.Runtime.TransactionLocker.Instance.TryLock(transactionUniqueName,
                    () =>
                    {
                        action();
                        lockSucceeded = true;
                    });
                if(!lockSucceeded)
                {
                    if ((DateTime.Now - startTime) > s_transactionLockMaxRetryInterval)
                        throw new Exception($"Max retry interval exceeded. transactionLockMaxRetryInterval '{s_transactionLockMaxRetryInterval}'");
                }
            }
        }

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

        private FileRecordStoragePreparedConfig GetPreparedConfig()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordStorageManager.CacheManager>().GetOrCreateObject(
                                string.Concat("FileRecordStorageDataManager_GetPreparedConfig_", _dataRecordStorage.DataRecordStorageId),
                                () =>
                                {
                                    var metadataTableSchema = _dataRecordStorageSettings.MetadataSchemaName;
                                    var metadataTableName = _dataRecordStorageSettings.MetadataTableName;
                                    string metadataRDBTableName = string.Concat(metadataTableSchema, "_", metadataTableName, Guid.NewGuid());

                                    var metadataRDBColumns = new Dictionary<string, RDBTableColumnDefinition>();
                                    metadataRDBColumns.Add(COL_ID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
                                    metadataRDBColumns.Add(COL_RuntimeNodeID, new RDBTableColumnDefinition { DataType = RDBDataType.UniqueIdentifier });
                                    metadataRDBColumns.Add(COL_ParentFolderRelativePath, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
                                    metadataRDBColumns.Add(COL_FileNames, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar });
                                    metadataRDBColumns.Add(COL_FromTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_ToTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_NbOfRecords, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
                                    metadataRDBColumns.Add(COL_MinTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_MaxTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_MinID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
                                    metadataRDBColumns.Add(COL_MaxID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
                                    metadataRDBColumns.Add(COL_IsReady, new RDBTableColumnDefinition { DataType = RDBDataType.Boolean });
                                    metadataRDBColumns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_LastModifiedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

                                    var metadataRDBTableDefinition = new RDBTableDefinition
                                    {
                                        DBSchemaName = metadataTableSchema,
                                        DBTableName = metadataTableName,
                                        Columns = metadataRDBColumns,
                                        IdColumnName = COL_ID,
                                        CreatedTimeColumnName = COL_CreatedTime,
                                        ModifiedTimeColumnName = COL_LastModifiedTime
                                    };


                                    List<string> fieldNamesToInsert = new List<string>();
                                    this.DataRecordType.Settings.IdField.ThrowIfNull("this.DataRecordType.Settings.IdField", this.DataRecordType.DataRecordTypeId);
                                    this.DataRecordType.Settings.DateTimeField.ThrowIfNull("this.DataRecordType.Settings.DateTimeField", this.DataRecordType.DataRecordTypeId);
                                    string idFieldName = this.DataRecordType.Settings.IdField;
                                    string dateTimeFieldName = this.DataRecordType.Settings.DateTimeField;
                                    bool idFieldFound = false;
                                    bool dateTimeFieldFound = false;
                                    foreach (var fld in this.DataRecordFieldsByName.Values.OrderBy(fld => fld.Name))
                                    {
                                        if (fld.Formula != null)
                                        {
                                            continue;
                                        }
                                        if (fld.Name == idFieldName)
                                        {
                                            idFieldFound = true;
                                            continue;
                                        }
                                        if (fld.Name == dateTimeFieldName)
                                        {
                                            dateTimeFieldFound = true;
                                            continue;
                                        }
                                        fieldNamesToInsert.Add(fld.Name);
                                    }
                                    if (!idFieldFound)
                                        throw new Exception($"ID field '{idFieldName}' not found in DataRecordType '{this.DataRecordType.DataRecordTypeId}'");
                                    if (!dateTimeFieldFound)
                                        throw new Exception($"ID field '{dateTimeFieldName}' not found in DataRecordType '{this.DataRecordType.DataRecordTypeId}'");

                                    RDBSchemaManager.Current.RegisterDefaultTableDefinition(metadataRDBTableName, metadataRDBTableDefinition);

                                    FileRecordStoragePreparedConfig preparedConfig = new FileRecordStoragePreparedConfig
                                    {
                                        MetadataRDBTableUniqueName = metadataRDBTableName,
                                        IdFieldName = idFieldName,
                                        DateTimeFieldName = dateTimeFieldName,
                                        GenerateRecordId = this._dataRecordStorageSettings.GenerateRecordIds,
                                        FieldNamesForInsert = fieldNamesToInsert.ToArray(),
                                        ConcatenatedFieldNamesForInsert = string.Join(this._dataRecordStorageSettings.FieldSeparator.ToString(), fieldNamesToInsert)
                                    };
                                    return preparedConfig;
                                });
        }

        private void ChangeFileRecordsWithLock(long fileMetadataId, string parentFolderRelativePath, DateTime batchFromTime,
            Func<List<FileRecordStorageRecordInfo>, List<FileRecordStorageRecordInfo>> changeRecords, bool recordsAlreadyOrdered = false)
        {
            string parentFolderPath = GetParentFolderPath(parentFolderRelativePath);
            LockFileToChange(fileMetadataId,
                () =>
                {
                    string[] oldFileNames;
                    List<string> oldFileFullPaths;
                    var existingRecordInfos = ReadRecordInfosFromFileWithLock(fileMetadataId, parentFolderRelativePath, out oldFileNames, out oldFileFullPaths);
                    var changedRecordInfos = changeRecords(existingRecordInfos);
                    if (changedRecordInfos != null && changedRecordInfos.Count > 0)
                    {
                        string newFileName = BuildFileName(fileMetadataId, batchFromTime);
                        DateTime minRecordTime, maxRecordTime;
                        long minRecordId, maxRecordId;
                        SaveRecordsToFile(fileMetadataId, changedRecordInfos, parentFolderPath, newFileName,
                            out minRecordTime, out maxRecordTime, out minRecordId, out maxRecordId, recordsAlreadyOrdered);

                        LockReadFile(fileMetadataId,
                            () =>
                            {
                                UpdateFileMetadata(fileMetadataId, newFileName, changedRecordInfos.Count, minRecordTime, maxRecordTime, minRecordId, maxRecordId);
                            });
                        if (oldFileFullPaths != null)
                        {
                            foreach (var oldFileFullPath in oldFileFullPaths)
                            {
                                File.Delete(oldFileFullPath);
                            }
                        }

                    }
                    else
                    {
                        var queryContext = new RDBQueryContext(GetDataProvider());
                        var deleteQuery = queryContext.AddDeleteQuery();
                        deleteQuery.FromTable(GetPreparedConfig().MetadataRDBTableQuerySource);
                        deleteQuery.Where().EqualsCondition(COL_ID).Value(fileMetadataId);
                        LockReadFile(fileMetadataId,
                            () =>
                            {
                                queryContext.ExecuteNonQuery();
                            });
                        foreach (var oldFileFullPath in oldFileFullPaths)
                        {
                            File.Delete(oldFileFullPath);
                        }
                        if (parentFolderRelativePath != null)
                        {
                            LockCreateDirectory(
                                () =>
                                {
                                    string folderRelativePath = parentFolderRelativePath;
                                    while (!string.IsNullOrEmpty(folderRelativePath))
                                    {
                                        string folderFullPath = Path.Combine(GetRootFolderPath(), folderRelativePath);
                                        if (Directory.Exists(folderFullPath) && !Directory.EnumerateFiles(folderFullPath).Any() && !Directory.EnumerateDirectories(folderFullPath).Any())
                                        {
                                            Directory.Delete(folderFullPath);
                                            int lastIndexOfSlash = folderRelativePath.LastIndexOf('\\');

                                            if (lastIndexOfSlash >= 0)
                                                folderRelativePath = folderRelativePath.Substring(0, lastIndexOfSlash);
                                            else
                                                break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                });
                        }
                    }
                });
        }

        void LockFileToChange(long fileMetadataId, Action onLocked)
        {
            Lock($"FileRecordStorage_ChangeFileRecords_{fileMetadataId}", onLocked);
        }

        private string GetParentFolderPath(DateTime batchFromTime, bool createIfNotExists, out string parentFolderRelativePath)
        {
            parentFolderRelativePath = null;
            if(this._dataRecordStorageSettings.FolderStructureType.HasValue)
            {
                switch(this._dataRecordStorageSettings.FolderStructureType.Value)
                {
                    case FileDataRecordStorageFolderStructureType.MonthDateHour:
                        parentFolderRelativePath = $@"{batchFromTime.Year}-{batchFromTime.Month.ToString().PadLeft(2, '0')}\{batchFromTime.Day.ToString().PadLeft(2, '0')}\{batchFromTime.Hour.ToString().PadLeft(2, '0')}";
                        break;
                    default:throw new NotSupportedException($"this._dataRecordStorageSettings.FolderStructureType '{this._dataRecordStorageSettings.FolderStructureType.Value.ToString()}'");
                }
            }
            string parentFolderPath = GetParentFolderPath(parentFolderRelativePath);
            if(createIfNotExists)
            {
                if(!Directory.Exists(parentFolderPath))
                {
                    LockCreateDirectory(
                        () =>
                        {
                            if (!Directory.Exists(parentFolderPath))
                                Directory.CreateDirectory(parentFolderPath);
                        });

                }
            }
            return parentFolderPath;
        }

        private string GetParentFolderPath(string parentFolderRelativePath)
        {
            return parentFolderRelativePath != null ? Path.Combine(GetRootFolderPath(), parentFolderRelativePath) : GetRootFolderPath();
        }

        private string GetRootFolderPath()
        {
            string configuredRootPath = ConfigurationManager.AppSettings[$"FileRecordStorage_{this._dataRecordStorage.DataRecordStorageId.ToString().ToLower()}_RootFolderPath"];
            if (!string.IsNullOrWhiteSpace(configuredRootPath))
                return configuredRootPath;
            else
                return this._dataRecordStorageSettings.RootFolderPath;
        }

        private void LockCreateDirectory(Action onLocked)
        {
            Lock($"FileRecordStorage_CreateDirectory", onLocked);
        }

        private void SaveRecordsToFile(long fileMetadataId, List<FileRecordStorageRecordInfo> records, 
            string parentFolderPath, string fileName, out DateTime minRecordTime, out DateTime maxRecordTime, 
            out long minRecordId, out long maxRecordId, bool recordsAlreadyOrdered = false)
        {
            
            string targetFileFullPath = Path.Combine(parentFolderPath, fileName);
            Char fieldSeparator = this._dataRecordStorageSettings.FieldSeparator;
            minRecordTime = DateTime.MaxValue;
            maxRecordTime = DateTime.MinValue;
            minRecordId = long.MaxValue;
            maxRecordId = long.MinValue;
            if (!recordsAlreadyOrdered)
                records = records.OrderBy(rec => rec.RecordTime).ToList();
            List<string> lines = new List<string>();
            foreach (var record in records)
            {
                lines.Add(string.Concat(Vanrise.Data.BaseDataManager.GetDateTimeForBCP(record.RecordTime), fieldSeparator, record.RecordId, fieldSeparator, record.RecordContent));

                if (record.RecordTime < minRecordTime)
                    minRecordTime = record.RecordTime;
                if (record.RecordTime > maxRecordTime)
                    maxRecordTime = record.RecordTime;

                if (record.RecordId < minRecordId)
                    minRecordId = record.RecordId;
                if (record.RecordId > maxRecordId)
                    maxRecordId = record.RecordId;
            }
            WriteAllLinesToFile(targetFileFullPath, lines);
        }

        private string BuildFileName(long fileMetadataId, DateTime batchFromTime)
        {
            return $@"{fileMetadataId.ToString().PadLeft(10, '0')}-{batchFromTime.ToString("yyyyMMdd HHmmss fff")}-{Guid.NewGuid().ToString().Replace("-", "")}.vrrec";
        }

        private void WriteAllLinesToFile(string targetFileFullPath, List<string> lines)
        {
            using (StreamWriter sw = new StreamWriter(targetFileFullPath))
            {
                sw.WriteLine(GetPreparedConfig().ConcatenatedFieldNamesForInsert);
                for (int i = 0; i < lines.Count; i++)
                {
                    sw.WriteLine(lines[i]);
                }
                sw.Close();
            }
            //File.WriteAllLines(targetFileFullPath, lines);
        }

        private void UpdateFileMetadata(long fileMetadataId, string fileName, long nbOfRecords, DateTime minRecordTime, DateTime maxRecordTime, long minRecordId, long maxRecordId)
        {
            var preparedConfig = GetPreparedConfig();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(preparedConfig.MetadataRDBTableQuerySource);
            updateQuery.Column(COL_FileNames).Value(fileName);
            updateQuery.Column(COL_NbOfRecords).Value(nbOfRecords);
            updateQuery.Column(COL_MinTime).Value(minRecordTime);
            updateQuery.Column(COL_MaxTime).Value(maxRecordTime);
            updateQuery.Column(COL_MinID).Value(minRecordId);
            updateQuery.Column(COL_MaxID).Value(maxRecordId);
            updateQuery.Column(COL_IsReady).Value(true);
            updateQuery.Where().EqualsCondition(COL_ID).Value(fileMetadataId);
            queryContext.ExecuteNonQuery();
        }

        private List<FileRecordStorageFileMetadata> GetFileMetadatas(DateTime? from, DateTime? to, bool? isOrderAscending, Action<RDBConditionContext> addConditionsToWhere)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
            selectQuery.SelectColumns().AllTableColumns("mtdata");
            var where = selectQuery.Where();
            if (to.HasValue)
                where.LessOrEqualCondition(COL_FromTime).Value(to.Value);
            if (from.HasValue)
                where.GreaterOrEqualCondition(COL_ToTime).Value(from.Value);
            if (addConditionsToWhere != null)
                addConditionsToWhere(where);
            AddFileMetadataReadyCondition(where);
            if (isOrderAscending.HasValue)
            {
                RDBSortDirection sortDirection = isOrderAscending.Value ? RDBSortDirection.ASC : RDBSortDirection.DESC;
                var sortContext = selectQuery.Sort();
                sortContext.ByColumn(COL_FromTime, sortDirection);
                sortContext.ByColumn(COL_ToTime, sortDirection);
            }
            List<FileRecordStorageFileMetadata> fileMetadatas = queryContext.GetItems(FileRecordStorageFileMetadataMapper);
            return fileMetadatas;
        }

        private long? GetMatchFileMetadataId(DateTime fromTime, DateTime toTime)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", 1, true);
            selectQuery.SelectColumns().Column(COL_ID);
            var where = selectQuery.Where();
            where.EqualsCondition(COL_FromTime).Value(fromTime);
            where.EqualsCondition(COL_ToTime).Value(toTime);
            AddFileMetadataReadyCondition(where);
            return queryContext.ExecuteScalar().NullableLongValue;
        }

        #region Deleted Code

        //public DateTime? GetMinDateTimeWithMaxIdAfterId(long id, string idFieldName, string dateTimeFieldName, out long? maxId)
        //{
        //    if (idFieldName != this.DataRecordType.Settings.IdField)
        //        throw new NotSupportedException($"idFieldName '{idFieldName}' is different than record type Id Field '{this.DataRecordType.Settings.IdField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");
        //    if (dateTimeFieldName != this.DataRecordType.Settings.DateTimeField)
        //        throw new NotSupportedException($"dateTimeFieldName '{dateTimeFieldName}' is different than record type DateTime Field '{this.DataRecordType.Settings.DateTimeField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");

        //    var queryContext = new RDBQueryContext(GetDataProvider());
        //    var selectQuery = queryContext.AddSelectQuery();
        //    selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
        //    selectQuery.SelectColumns().AllTableColumns("mtdata");

        //    var where = selectQuery.Where();
        //    where.GreaterThanCondition(COL_MaxID).Value(id);
        //    AddFileMetadataReadyCondition(where);

        //    List<FileRecordStorageFileMetadata> fileMetadatas = queryContext.GetItems(FileRecordStorageFileMetadataMapper);

        //    DateTime? minDate = null;
        //    maxId = null;
        //    if (fileMetadatas != null && fileMetadatas.Count > 0)
        //    {
        //        foreach (var fileMetadata in fileMetadatas)
        //        {
        //            string[] fileNames;
        //            List<string> fileFullPaths;
        //            List<FileRecordStorageRecordInfo> recordInfos = ReadRecordInfosFromFileWithLock(fileMetadata.FileRecordStorageFileMetadataId, fileMetadata.ParentFolderRelativePath, out fileNames, out fileFullPaths);
        //            if (recordInfos != null)
        //            {
        //                foreach (var recordInfo in recordInfos)
        //                {
        //                    if (recordInfo.RecordId > id)
        //                    {
        //                        if (!minDate.HasValue || minDate.Value > recordInfo.RecordTime)
        //                            minDate = recordInfo.RecordTime;
        //                        if (!maxId.HasValue || maxId.Value < recordInfo.RecordId)
        //                            maxId = recordInfo.RecordId;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return minDate;
        //}

        //private class ReadRecordInfosTaskHandle
        //{
        //    public bool IsTaskStopped { get; set; }
        //}

        //private void ReadRecordsFromFileWithLock(long fileRecordStorageFileMetadataId, string parentFolderRelativePath, string fileName,
        //    Func<FileRecordStorageRecordInfo, bool> shouldReadRecordFromInfo, Action<FileRecordStorageRecordInfo, dynamic> onRecordRead, ReadRecordInfosTaskHandle taskHandle)
        //{
        //    ConcurrentQueue<FileRecordStorageRecordInfo> qRecordInfos = new ConcurrentQueue<FileRecordStorageRecordInfo>();

        //    bool isTaskReadRecordInfosCompleted = false;
        //    Task taskReadRecordInfos = new Task(
        //        () =>
        //        {
        //            try
        //            {
        //                ReadRecordInfosFromFileWithLock(fileRecordStorageFileMetadataId, parentFolderRelativePath, fileName,
        //                    (recordInfo) =>
        //                    {
        //                        if (shouldReadRecordFromInfo(recordInfo))
        //                            qRecordInfos.Enqueue(recordInfo);
        //                    }, taskHandle);
        //            }
        //            finally
        //            {
        //                isTaskReadRecordInfosCompleted = true;
        //            }
        //        });
        //    taskReadRecordInfos.Start();

        //    var dynamicManager = this.DynamicManager;
        //    while (!taskHandle.IsTaskStopped && (!isTaskReadRecordInfosCompleted || qRecordInfos.Count > 0))
        //    {
        //        FileRecordStorageRecordInfo recordInfo;
        //        while (!taskHandle.IsTaskStopped && qRecordInfos.TryDequeue(out recordInfo))
        //        {
        //            dynamic record = dynamicManager.GetDynamicRecordFromRecordInfo(recordInfo);
        //            onRecordRead(recordInfo, record);
        //        }
        //        Thread.Sleep(50);
        //    }

        //    if (taskReadRecordInfos.Exception != null)
        //        throw taskReadRecordInfos.Exception;
        //}

        //private void ReadRecordInfosFromFileWithLock(long fileRecordStorageFileMetadataId, string parentFolderRelativePath, string fileName,
        //    Action<FileRecordStorageRecordInfo> onRecordInfoRead, ReadRecordInfosTaskHandle taskHandle)
        //{
        //    ConcurrentQueue<string> qLines = new ConcurrentQueue<string>();
        //    ConcurrentQueue<FileRecordStorageRecordInfo> qRecordInfos = new ConcurrentQueue<FileRecordStorageRecordInfo>();

        //    bool isTaskReadLinesCompleted = false;
        //    Task taskReadLines = new Task(
        //        () =>
        //        {
        //            try
        //            {
        //                ReadFileWithLock(fileRecordStorageFileMetadataId, parentFolderRelativePath, fileName,
        //                    (line) => qLines.Enqueue(line));
        //            }
        //            finally
        //            {
        //                isTaskReadLinesCompleted = true;
        //            }
        //        });

        //    bool isTaskParseLinesCompleted = false;
        //    Task taskParseLines = new Task(
        //        () =>
        //        {
        //            try
        //            {
        //                while(!taskHandle.IsTaskStopped && (!isTaskReadLinesCompleted || qLines.Count > 0))
        //                {
        //                    string line;
        //                    while(!taskHandle.IsTaskStopped && qLines.TryDequeue(out line))
        //                    {
        //                        string datetimeValue = line.Substring(0, line.IndexOf(this._dataRecordStorageSettings.FieldSeparator));
        //                        datetimeValue.ThrowIfNull("datetimeValue");
        //                        string lineWithoutDate = line.Substring(datetimeValue.Length + 1);
        //                        string idValue = lineWithoutDate.Substring(0, lineWithoutDate.IndexOf(this._dataRecordStorageSettings.FieldSeparator));
        //                        idValue.ThrowIfNull("idValue");
        //                        DateTime recordDateTime;
        //                        if (!DateTime.TryParse(datetimeValue, out recordDateTime))
        //                            throw new Exception($"Cannot parse datetimeValue '{datetimeValue}' in line '{line}'");
        //                        long recordId;
        //                        if (!long.TryParse(idValue, out recordId))
        //                            throw new Exception($"Cannot parse idValue '{idValue}' in line '{line}'");
        //                        var recordInfo = new FileRecordStorageRecordInfo
        //                        {
        //                            RecordTime = recordDateTime,
        //                            RecordId = recordId,
        //                            RecordContent = lineWithoutDate.Substring(idValue.Length + 1)
        //                        };
        //                        qRecordInfos.Enqueue(recordInfo);
        //                    }
        //                    Thread.Sleep(50);
        //                }
        //            }
        //            finally
        //            {
        //                isTaskParseLinesCompleted = true;
        //            }
        //        });

        //    taskReadLines.Start();
        //    taskParseLines.Start();

        //    while(!taskHandle.IsTaskStopped && (!isTaskParseLinesCompleted || qRecordInfos.Count > 0))
        //    {
        //        FileRecordStorageRecordInfo recordInfo;
        //        while(!taskHandle.IsTaskStopped && qRecordInfos.TryDequeue(out recordInfo))
        //        {
        //            onRecordInfoRead(recordInfo);
        //        }
        //        Thread.Sleep(50);
        //    }

        //    if (taskReadLines.Exception != null)
        //        throw taskReadLines.Exception;
        //    if (taskParseLines.Exception != null)
        //        throw taskParseLines.Exception;
        //}


        //private void ReadFileWithLock(long fileRecordStorageFileMetadataId, string parentFolderRelativePath, string fileName, Action<string> onLineRead)
        //{
        //    string fullFilePath;
        //    fullFilePath = GetFileFullPath(parentFolderRelativePath, fileName);

        //    LockReadFile(fileRecordStorageFileMetadataId,
        //        () =>
        //        {
        //            if (File.Exists(fullFilePath))
        //            {
        //                ReadAllLinesFromFile(fullFilePath, onLineRead);
        //            }
        //            else
        //            {
        //                var queryContext = new RDBQueryContext(GetDataProvider());
        //                var selectQuery = queryContext.AddSelectQuery();
        //                selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
        //                selectQuery.SelectColumns().Columns(COL_FileName);
        //                selectQuery.Where().EqualsCondition(COL_ID).Value(fileRecordStorageFileMetadataId);
        //                fileName = queryContext.ExecuteScalar().StringValue;
        //                if (fileName != null)
        //                {
        //                    fullFilePath = GetFileFullPath(parentFolderRelativePath, fileName);
        //                    ReadAllLinesFromFile(fullFilePath, onLineRead);
        //                }
        //            }
        //        });
        //}

        //private void ReadAllLinesFromFile(string fullFilePath, Action<string> onLineRead)
        //{
        //    using (FileStream fs = File.Open(fullFilePath, FileMode.Open))
        //    {
        //        using (BufferedStream bs = new BufferedStream(fs))
        //        {
        //            using (StreamReader sr = new StreamReader(bs))
        //            {
        //                string s;
        //                while ((s = sr.ReadLine()) != null)
        //                {
        //                    onLineRead(s);
        //                }
        //                sr.Close();
        //            }
        //            bs.Close();
        //        }
        //        fs.Close();
        //    }
        //    //return File.ReadAllLines(fullFilePath);
        //}

        #endregion

        private List<FileRecordStorageRecordInfo> ReadRecordInfosFromFileWithLock(long fileRecordStorageFileMetadataId, string parentFolderRelativePath, out string[] fileNames, out List<string> fileFullPaths)
        {
            List<FileRecordStorageRecordInfo> recordInfos = new List<FileRecordStorageRecordInfo>();
            string[] fileNames_Local = null;
            List<string> fileFullPaths_Local = null;
            LockReadFile(fileRecordStorageFileMetadataId,
                () =>
                {
                    var queryContext = new RDBQueryContext(GetDataProvider());
                    var selectQuery = queryContext.AddSelectQuery();
                    selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
                    selectQuery.SelectColumns().Columns(COL_FileNames);
                    selectQuery.Where().EqualsCondition(COL_ID).Value(fileRecordStorageFileMetadataId);
                    string fileNamesConcatenated = queryContext.ExecuteScalar().StringValue;
                    if (fileNamesConcatenated != null)
                    {
                        fileNames_Local = fileNamesConcatenated.Split(',');
                        fileFullPaths_Local = fileNames_Local.Select(fileName => GetFileFullPath(parentFolderRelativePath, fileName)).ToList();
                        Parallel.ForEach(fileFullPaths_Local,
                            (fullFilePath) =>
                            {
                                var lines = ReadAllLinesFromFile(fullFilePath);
                                if (lines != null)
                                {
                                    List<FileRecordStorageRecordInfo> fileRecordInfos = new List<FileRecordStorageRecordInfo>();
                                    for (int i = 0; i < lines.Count; i++)
                                    {
                                        var line = lines[i];
                                        string datetimeValue = line.Substring(0, line.IndexOf(this._dataRecordStorageSettings.FieldSeparator));
                                        datetimeValue.ThrowIfNull("datetimeValue");
                                        string lineWithoutDate = line.Substring(datetimeValue.Length + 1);
                                        string idValue = lineWithoutDate.Substring(0, lineWithoutDate.IndexOf(this._dataRecordStorageSettings.FieldSeparator));
                                        idValue.ThrowIfNull("idValue");
                                        DateTime recordDateTime;
                                        if (!DateTime.TryParse(datetimeValue, out recordDateTime))
                                            throw new Exception($"Cannot parse datetimeValue '{datetimeValue}' in line '{line}'");
                                        long recordId;
                                        if (!long.TryParse(idValue, out recordId))
                                            throw new Exception($"Cannot parse idValue '{idValue}' in line '{line}'");
                                        var recordInfo = new FileRecordStorageRecordInfo
                                        {
                                            RecordTime = recordDateTime,
                                            RecordId = recordId,
                                            RecordContent = lineWithoutDate.Substring(idValue.Length + 1)
                                        };
                                        fileRecordInfos.Add(recordInfo);
                                    }
                                    lock(recordInfos)
                                    {
                                        recordInfos.AddRange(fileRecordInfos);
                                    }
                                }
                            });
                    }
                });
            fileNames = fileNames_Local;
            fileFullPaths = fileFullPaths_Local;
            return recordInfos;
        }

        private List<string> ReadFileWithLock(long fileRecordStorageFileMetadataId, string parentFolderRelativePath, out string[] fileNames, out List<string> fileFullPaths)
        {
            List<string> lines = null;
            string[] fileNames_Local = null;
            List<string> fileFullPaths_Local = null;
            LockReadFile(fileRecordStorageFileMetadataId,
                () =>
                {
                    var queryContext = new RDBQueryContext(GetDataProvider());
                    var selectQuery = queryContext.AddSelectQuery();
                    selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
                    selectQuery.SelectColumns().Columns(COL_FileNames);
                    selectQuery.Where().EqualsCondition(COL_ID).Value(fileRecordStorageFileMetadataId);
                    string fileNamesConcatenated = queryContext.ExecuteScalar().StringValue;
                    if (fileNamesConcatenated != null)
                    {
                        fileNames_Local = fileNamesConcatenated.Split(',');
                        fileFullPaths_Local = fileNames_Local.Select(fileName => GetFileFullPath(parentFolderRelativePath, fileName)).ToList();
                        lines = new List<string>();
                        Parallel.ForEach(fileFullPaths_Local,
                            (fullFilePath) =>
                            {
                                var fileLines = ReadAllLinesFromFile(fullFilePath);
                                if (fileLines != null)
                                {
                                    lock (lines)
                                    {
                                        lines.AddRange(fileLines);
                                    }
                                }
                            });
                    }
                });
            fileNames = fileNames_Local;
            fileFullPaths = fileFullPaths_Local;
            return lines;
        }

        private List<string> ReadAllLinesFromFile(string fullFilePath)
        {
            List<string> lines = new List<string>();
            using (FileStream fs = File.Open(fullFilePath, FileMode.Open))
            {
                using (BufferedStream bs = new BufferedStream(fs))
                {
                    using (StreamReader sr = new StreamReader(bs))
                    {
                        string s;
                        bool isFirstLine = true;
                        while ((s = sr.ReadLine()) != null)
                        {
                            if(isFirstLine)//first line is the header
                            {
                                isFirstLine = false;
                                continue;
                            }
                            lines.Add(s);
                        }
                        sr.Close();
                    }
                    bs.Close();
                }
                fs.Close();
            }
            return lines;
            //return File.ReadAllLines(fullFilePath);
        }

        private string GetFileFullPath(string parentFolderRelativePath, string fileName)
        {
            return Path.Combine(GetParentFolderPath(parentFolderRelativePath), fileName);
        }

        private void GetBatchTimesFromRecordTime(DateTime recordTime, out DateTime batchStartTime, out DateTime batchEndTime)
        {
            TimeSpan storagePartitionInterval = this._dataRecordStorageSettings.StoragePartitionInterval;
            int nbOfSecondsToAdd = (int)(storagePartitionInterval.TotalSeconds * (int)((recordTime - recordTime.Date).TotalSeconds / storagePartitionInterval.TotalSeconds));
            batchStartTime = recordTime.Date.AddSeconds(nbOfSecondsToAdd);
            batchEndTime = batchStartTime.AddSeconds((int)storagePartitionInterval.TotalSeconds);
            if (batchEndTime > batchStartTime.Date.AddDays(1))
                batchEndTime = batchStartTime.Date.AddDays(1);
        }

        private FileRecordStorageFileMetadata FileRecordStorageFileMetadataMapper(IRDBDataReader reader)
        {
            return new FileRecordStorageFileMetadata
            {
                FileRecordStorageFileMetadataId = reader.GetLong(COL_ID),
                ParentFolderRelativePath = reader.GetString(COL_ParentFolderRelativePath),
                NbOfRecords = reader.GetLongWithNullHandling(COL_NbOfRecords),
                FromTime = reader.GetDateTime(COL_FromTime),
                ToTime = reader.GetDateTime(COL_ToTime),
                MinTime = reader.GetDateTime(COL_MinTime),
                MaxTime = reader.GetDateTime(COL_MaxTime),
                MinId = reader.GetLong(COL_MinID),
                MaxId = reader.GetLong(COL_MaxID)
            };
        }

        private void GenerateRecordIds(FileRecordStorageBatches batches)
        {
            long startingRecordId;
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(new VanriseType($"FileDataRecordStorage_{this._dataRecordStorage.DataRecordStorageId}"), batches.RecordCount, out startingRecordId);
            long currentId = startingRecordId;
            foreach (var batch in batches.Batches.Values)
            {
                foreach (var record in batch.Records)
                {
                    record.RecordId = currentId;
                    currentId++;
                }
            }
        }

        #endregion

        #region Private Classes

        #endregion
    }

    public class FileRecordStoragePreparedConfig
    {
        public string MetadataRDBTableUniqueName { get; set; }

        public RDBTableDefinitionQuerySource MetadataRDBTableQuerySource
        {
            get
            {
                return new RDBTableDefinitionQuerySource(this.MetadataRDBTableUniqueName);
            }
        }

        public string[] FieldNamesForInsert { get; set; }

        public string ConcatenatedFieldNamesForInsert { get; set; }

        public string IdFieldName { get; set; }

        public string DateTimeFieldName { get; set; }

        public bool GenerateRecordId { get; set; }
    }

    public class FileRecordStorageBatches
    {
        public FileRecordStorageBatches()
        {
            this.Batches = new Dictionary<DateTime, FileRecordStorageBatchInfo>();
        }
        public Dictionary<DateTime, FileRecordStorageBatchInfo> Batches { get; private set; }

        public int RecordCount { get; set; }
    }

    public class FileRecordStorageBatchInfo
    {
        public FileRecordStorageBatchInfo()
        {
            this.Records = new List<FileRecordStorageRecordInfo>();
        }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }
        
        public List<FileRecordStorageRecordInfo> Records { get; private set; }
    }

    public class FileRecordStorageRecordInfo
    {
        public DateTime RecordTime { get; set; }

        public long RecordId { get; set; }

        public string RecordContent { get; set; }
    }

    public class FileRecordStorageRecord
    {
        public FileRecordStorageRecord(FileRecordStorageRecordInfo recordInfo, dynamic record)
        {
            this.RecordInfo = recordInfo;
            this.Record = record;
        }

        public FileRecordStorageRecordInfo RecordInfo { get; private set; }

        public dynamic Record { get; private set; }

        public DateTime RecordTime
        {
            get
            {
                return this.RecordInfo.RecordTime;
            }
        }

        public long RecordId {
            get
            {
                return this.RecordInfo.RecordId;
            }
        }

    }
}