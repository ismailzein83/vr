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

namespace Vanrise.GenericData.FileDataStorage
{
    public class FileRecordStorageDataManager : IDataRecordDataManager
    {
        #region RDB Metadata Columns

        const string COL_ID = "ID";
        const string COL_ParentFolderRelativePath = "ParentFolderRelativePath";
        const string COL_FileName = "FileName";
        const string COL_FromTime = "FromTime";
        const string COL_ToTime = "ToTime";
        const string COL_MinTime = "MinTime";
        const string COL_MaxTime = "MaxTime";
        const string COL_MinID = "MinID";
        const string COL_MaxID = "MaxID";
        const string COL_IsReady = "IsReady";
        const string COL_CreatedTime = "CreatedTime";

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
                List<FileRecordStorageRecordInfo> records = batch.Records.OrderBy(rec => rec.RecordTime).ToList();
                string parentFolderPath = GetParentFolderPath(batch.FromTime, true);

                var queryContext = new RDBQueryContext(GetDataProvider());
                var insertQuery = queryContext.AddInsertQuery();
                insertQuery.IntoTable(preparedConfig.MetadataRDBTableQuerySource);
                insertQuery.AddSelectGeneratedId();
                insertQuery.Column(COL_ParentFolderRelativePath).Value(parentFolderPath);
                insertQuery.Column(COL_FromTime).Value(batch.FromTime);
                insertQuery.Column(COL_ToTime).Value(batch.ToTime);
                var fileMetadataId = queryContext.ExecuteScalar().LongValue;
                
                string fileName;
                DateTime minRecordTime, maxRecordTime;
                long minRecordId, maxRecordId;
                SaveRecordsToFile(fileMetadataId, records, batch.FromTime, parentFolderPath, null, out fileName, out minRecordTime, out maxRecordTime, out minRecordId, out maxRecordId);

                UpdateFileMetadata(fileMetadataId, fileName, minRecordTime, maxRecordTime, minRecordId, maxRecordId);
            }
        }

        public List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context)
        {
            throw new NotImplementedException();
        }

        public void GetDataRecords(DateTime? from, DateTime? to, RecordFilterGroup recordFilterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady, string orderColumnName = null, bool isOrderAscending = false)
        {
            bool isOrderByDate = orderColumnName == this.DataRecordType.Settings.DateTimeField;

            List<FileRecordStorageFileMetadata> fileMetadatas = GetFileMetadatas(from, to, isOrderAscending);

            var dynamicManager = this.DynamicManager;
            FileRecordStorageFileMetadata previousFileMetadata = null;
            List<FileRecordStorageRecord> currentRecords;
            if (!string.IsNullOrWhiteSpace(orderColumnName))
                currentRecords = new List<FileRecordStorageRecord>();
            else
                currentRecords = null;
            int nbOfPreviousFiles = 0;
            foreach (var fileMetadata in fileMetadatas)
            {
                if (shouldStop())
                    break;

                if (isOrderByDate)
                {
                    if (previousFileMetadata != null && !Utilities.AreTimePeriodsOverlapped(fileMetadata.MinTime, fileMetadata.MaxTime, previousFileMetadata.MinTime, previousFileMetadata.MaxTime))
                    {
                        if (nbOfPreviousFiles > 1)
                            currentRecords = isOrderAscending ? currentRecords.OrderBy(rec => rec.RecordTime).ToList() : currentRecords.OrderByDescending(rec => rec.RecordTime).ToList();
                        foreach (var record in currentRecords)
                        {
                            if (shouldStop())
                                break;
                            onItemReady(record.Record);
                        }
                        currentRecords = new List<FileRecordStorageRecord>();
                        nbOfPreviousFiles = 0;
                    }
                }

                bool filterFromTime = from.HasValue && from.Value > fileMetadata.FromTime;
                bool filterToTime = to.HasValue && to.Value < fileMetadata.ToTime;
                var recordInfos = ReadRecordInfosFromFileWithLock(fileMetadata.FileRecordStorageFileMetadataId, fileMetadata.ParentFolderRelativePath, fileMetadata.FileName);
                if (!isOrderAscending)
                    recordInfos = recordInfos.OrderByDescending(rec => rec.RecordTime).ToList();

                foreach (var recordInfo in recordInfos)
                {
                    if (shouldStop())
                        break;
                    if (filterFromTime)
                    {
                        if (recordInfo.RecordTime < from.Value)
                        {
                            if (isOrderAscending)
                                continue;
                            else
                                break;
                        }

                    }
                    if (filterToTime)
                    {
                        if (recordInfo.RecordTime > to.Value)
                        {
                            if (isOrderAscending)
                                break;
                            else
                                continue;
                        }
                    }
                    dynamic record = dynamicManager.GetDynamicRecordFromRecordInfo(recordInfo);
                    if (recordFilterGroup != null)
                    {
                        var recordIsFilterGroupMatchContext = new DataRecordFilterGenericFieldMatchContext(record, _dataRecordType.DataRecordTypeId);
                        if (!s_recordFilterManager.IsFilterGroupMatch(recordFilterGroup, recordIsFilterGroupMatchContext))
                            continue;
                    }
                    if (currentRecords != null)
                        currentRecords.Add(new FileRecordStorageRecord(recordInfo, record));
                    else
                        onItemReady(record);
                }
                previousFileMetadata = fileMetadata;
                nbOfPreviousFiles++;
            }

            if (currentRecords != null && currentRecords.Count > 0)
            {
                if (isOrderByDate)
                {
                    if (nbOfPreviousFiles > 1)
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

        private static void AddFileMetadataReadyCondition(RDBConditionContext where)
        {
            where.ConditionIfColumnNotNull(COL_IsReady).EqualsCondition(COL_IsReady).Value(true);
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
            return 2000;
        }

        public DateTime? GetMinDateTimeWithMaxIdAfterId(long id, string idFieldName, string dateTimeFieldName, out long? maxId)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecords(DateTime fromDate, DateTime toDate, List<long> idsToDelete, string idFieldName, string dateTimeFieldName)
        {
            if (idFieldName != this.DataRecordType.Settings.IdField)
                throw new NotSupportedException($"idFieldName '{idFieldName}' is different than record type Id Field '{this.DataRecordType.Settings.IdField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");
            if (dateTimeFieldName != this.DataRecordType.Settings.DateTimeField)
                throw new NotSupportedException($"dateTimeFieldName '{dateTimeFieldName}' is different than record type DateTime Field '{this.DataRecordType.Settings.DateTimeField}'. Record TypeId '{this.DataRecordType.DataRecordTypeId}'");
            
            List<FileRecordStorageFileMetadata> fileMetadatas = GetFileMetadatas(fromDate, toDate, null);

            foreach(var fileMetadata in fileMetadatas)
            {
                List<long> fileIdsToDelete = idsToDelete.Where(id => id >= fileMetadata.MinId && id <= fileMetadata.MaxId).ToList();
                if(fileIdsToDelete.Count > 0)
                {
                    ChangeFileRecordsWithLock(fileMetadata.FileRecordStorageFileMetadataId, fileMetadata.ParentFolderRelativePath, fileMetadata.FileName, fileMetadata.FromTime,
                        (existingRecordInfos) =>
                        {
                            List<FileRecordStorageRecordInfo> changedRecordInfos = new List<FileRecordStorageRecordInfo>();
                            foreach(var recordInfo in existingRecordInfos)
                            {
                                if (!fileIdsToDelete.Contains(recordInfo.RecordId))
                                    changedRecordInfos.Add(recordInfo);
                            }
                            return changedRecordInfos;
                        });
                }
            }

        }

        public long? GetMaxId(string idFieldName, string dateTimeFieldName, out DateTime? maxDate, out DateTime? minDate)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        private void LockReadFile(long fileRecordStorageFileMetadataId, Action onFileLocked)
        {
            Lock($"FileRecordStorage_ReadFile_{fileRecordStorageFileMetadataId}", onFileLocked);
        }

        void Lock(string transactionUniqueName, Action action)
        {
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
                                    metadataRDBColumns.Add(COL_ParentFolderRelativePath, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 1000 });
                                    metadataRDBColumns.Add(COL_FileName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
                                    metadataRDBColumns.Add(COL_FromTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_ToTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_MinTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_MaxTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_MinID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
                                    metadataRDBColumns.Add(COL_MaxID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
                                    metadataRDBColumns.Add(COL_CreatedTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });

                                    var metadataRDBTableDefinition = new RDBTableDefinition
                                    {
                                        DBSchemaName = metadataTableSchema,
                                        DBTableName = metadataTableName,
                                        Columns = metadataRDBColumns,
                                        IdColumnName = COL_ID,
                                        CreatedTimeColumnName = COL_CreatedTime
                                    };


                                    List<string> fieldNamesToInsert = new List<string>();
                                    this._dataRecordType.Settings.IdField.ThrowIfNull("this._dataRecordType.Settings.IdField", this._dataRecordType.DataRecordTypeId);
                                    this._dataRecordType.Settings.DateTimeField.ThrowIfNull("this._dataRecordType.Settings.DateTimeField", this._dataRecordType.DataRecordTypeId);
                                    string idFieldName = this._dataRecordType.Settings.IdField;
                                    string dateTimeFieldName = this._dataRecordType.Settings.DateTimeField;
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
                                        throw new Exception($"ID field '{idFieldName}' not found in DataRecordType '{this._dataRecordType.DataRecordTypeId}'");
                                    if (!dateTimeFieldFound)
                                        throw new Exception($"ID field '{dateTimeFieldName}' not found in DataRecordType '{this._dataRecordType.DataRecordTypeId}'");

                                    RDBSchemaManager.Current.RegisterDefaultTableDefinition(metadataRDBTableName, metadataRDBTableDefinition);

                                    FileRecordStoragePreparedConfig preparedConfig = new FileRecordStoragePreparedConfig
                                    {
                                        MetadataRDBTableUniqueName = metadataRDBTableName,
                                        IdFieldName = idFieldName,
                                        DateTimeFieldName = dateTimeFieldName,
                                        GenerateRecordId = this._dataRecordStorageSettings.GenerateRecordIds,
                                        FieldNamesForInsert = fieldNamesToInsert.ToArray()
                                    };
                                    return preparedConfig;
                                });
        }

        private void ChangeFileRecordsWithLock(long fileMetadataId, string parentFolderRelativePath, string fileName, DateTime batchFromTime,
            Func<List<FileRecordStorageRecordInfo>, List<FileRecordStorageRecordInfo>> changeRecords)
        {
            Lock($"FileRecordStorage_ChangeFileRecords_{fileMetadataId}",
                () =>
                {
                    var existingRecordInfos = ReadRecordInfosFromFileWithLock(fileMetadataId, parentFolderRelativePath, fileName);
                    var changedRecordInfos = changeRecords(existingRecordInfos);
                    string newFileName;
                    DateTime minRecordTime, maxRecordTime;
                    long minRecordId, maxRecordId;
                    string parentFolderPath = parentFolderRelativePath != null ? Path.Combine(this._dataRecordStorageSettings.RootFolderPath, parentFolderRelativePath) : this._dataRecordStorageSettings.RootFolderPath;
                    SaveRecordsToFile(fileMetadataId, changedRecordInfos, batchFromTime, parentFolderPath, Guid.NewGuid().ToString().Replace("-", ""), out newFileName, out minRecordTime, out maxRecordTime, out minRecordId, out maxRecordId);
                    string oldFileFullPath = Path.Combine(parentFolderPath, fileName);
                    LockReadFile(fileMetadataId,
                        () =>
                        {
                            UpdateFileMetadata(fileMetadataId, newFileName, minRecordTime, maxRecordTime, minRecordId, maxRecordId);
                            File.Delete(oldFileFullPath);
                        });
                });
        }

        private string GetParentFolderPath(DateTime batchFromTime, bool createIfNotExists)
        {
            string rootFolderPath = this._dataRecordStorageSettings.RootFolderPath;
            string parentFolderRelativePath = null;
            if(this._dataRecordStorageSettings.FolderStructureType.HasValue)
            {
                switch(this._dataRecordStorageSettings.FolderStructureType.Value)
                {
                    case FileDataRecordStorageFolderStructureType.MonthDateHour:
                        parentFolderRelativePath = $@"{batchFromTime.Year}-{batchFromTime.Month.ToString().PadLeft(2, '0')}\{batchFromTime.Day.ToString().PadLeft(2, '0')}\{batchFromTime.Hour.ToString().PadLeft(2, '0')}";
                        break;
                }
            }
            string parentFolderPath = parentFolderRelativePath != null ? Path.Combine(rootFolderPath, parentFolderRelativePath) : rootFolderPath;
            if(createIfNotExists)
            {
                if(!Directory.Exists(parentFolderPath))
                {
                    LockCreateDirectory(parentFolderPath,
                        () =>
                        {
                            if (!Directory.Exists(parentFolderPath))
                                Directory.CreateDirectory(parentFolderPath);
                        });

                }
            }
            return parentFolderPath;
        }

        private void LockCreateDirectory(string parentFolderPath, Action onLocked)
        {
            Lock($"FileRecordStorage_CreateDirectory", onLocked);
        }

        private void SaveRecordsToFile(long fileMetadataId, List<FileRecordStorageRecordInfo> records, DateTime batchFromTime, string parentFolderPath, string fileSuffix, out string fileName, out DateTime minRecordTime, out DateTime maxRecordTime, out long minRecordId, out long maxRecordId)
        {
            fileName = $@"{fileMetadataId.ToString().PadLeft(10, '0')}-{batchFromTime.ToString("yyyyMMdd HHmmss fff")}{(fileSuffix != null ? string.Concat("-", fileSuffix) : "")}.vrrec";
            string targetFileFullPath = Path.Combine(parentFolderPath, fileName);
            Char fieldSeparator = this._dataRecordStorageSettings.FieldSeparator;
            minRecordTime = DateTime.MaxValue;
            maxRecordTime = DateTime.MinValue;
            minRecordId = long.MaxValue;
            maxRecordId = long.MinValue;
            using (StreamWriter sw = new StreamWriter(targetFileFullPath))
            {
                foreach (var record in records.OrderBy(rec => rec.RecordTime))
                {
                    sw.WriteLine(string.Concat(Vanrise.Data.BaseDataManager.GetDateTimeForBCP(record.RecordTime), fieldSeparator, record.RecordId, fieldSeparator, record.RecordContent));

                    if (record.RecordTime < minRecordTime)
                        minRecordTime = record.RecordTime;
                    if (record.RecordTime > maxRecordTime)
                        maxRecordTime = record.RecordTime;

                    if (record.RecordId < minRecordId)
                        minRecordId = record.RecordId;
                    if (record.RecordId > maxRecordId)
                        maxRecordId = record.RecordId;
                }
            }
        }

        private void UpdateFileMetadata(long fileMetadataId, string fileName, DateTime minRecordTime, DateTime maxRecordTime, long minRecordId, long maxRecordId)
        {
            var preparedConfig = GetPreparedConfig();
            var queryContext = new RDBQueryContext(GetDataProvider());
            var updateQuery = queryContext.AddUpdateQuery();
            updateQuery.FromTable(preparedConfig.MetadataRDBTableQuerySource);
            updateQuery.Column(COL_FileName).Value(fileName);
            updateQuery.Column(COL_MinTime).Value(minRecordTime);
            updateQuery.Column(COL_MaxTime).Value(maxRecordTime);
            updateQuery.Column(COL_MinID).Value(minRecordId);
            updateQuery.Column(COL_MaxID).Value(maxRecordId);
            updateQuery.Where().EqualsCondition(COL_ID).Value(fileMetadataId);
            queryContext.ExecuteNonQuery();
        }

        private List<FileRecordStorageFileMetadata> GetFileMetadatas(DateTime? from, DateTime? to, bool? isOrderAscending)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
            selectQuery.SelectColumns().AllTableColumns("mtdata");
            var where = selectQuery.Where();
            if (to.HasValue)
                where.LessOrEqualCondition(COL_MinTime).Value(to.Value);
            if (from.HasValue)
                where.GreaterThanCondition(COL_MaxTime).Value(from.Value);
            AddFileMetadataReadyCondition(where);
            if (isOrderAscending.HasValue)
            {
                RDBSortDirection sortDirection = isOrderAscending.Value ? RDBSortDirection.ASC : RDBSortDirection.DESC;
                var sortContext = selectQuery.Sort();
                sortContext.ByColumn(COL_MinTime, sortDirection);
                sortContext.ByColumn(COL_MaxTime, sortDirection);
            }
            List<FileRecordStorageFileMetadata> fileMetadatas = queryContext.GetItems(FileRecordStorageFileMetadataMapper);
            return fileMetadatas;
        }

        private List<FileRecordStorageRecordInfo> ReadRecordInfosFromFileWithLock(long fileRecordStorageFileMetadataId, string parentFolderRelativePath, string fileName)
        {
            string[] lines = ReadFileWithLock(fileRecordStorageFileMetadataId, parentFolderRelativePath, fileName);
            List<FileRecordStorageRecordInfo> recordInfos = new List<FileRecordStorageRecordInfo>();
            if (lines != null)
            {
                foreach (var line in lines)
                {
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
                    recordInfos.Add(recordInfo);
                }
            }
            return recordInfos;
        }

        private string[] ReadFileWithLock(long fileRecordStorageFileMetadataId, string parentFolderRelativePath, string fileName)
        {
            string fullFilePath;
            fullFilePath = GetFileFullPath(parentFolderRelativePath, fileName);

            string[] lines = null;
            LockReadFile(fileRecordStorageFileMetadataId,
                () =>
                {
                    if (File.Exists(fullFilePath))
                    {
                        lines = File.ReadAllLines(fullFilePath);
                    }
                    else
                    {
                        var queryContext = new RDBQueryContext(GetDataProvider());
                        var selectQuery = queryContext.AddSelectQuery();
                        selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
                        selectQuery.SelectColumns().Columns(COL_FileName);
                        selectQuery.Where().EqualsCondition(COL_ID).Value(fileRecordStorageFileMetadataId);
                        fileName = queryContext.ExecuteScalar().StringValue;
                        if (fileName != null)
                        {
                            fullFilePath = GetFileFullPath(parentFolderRelativePath, fileName);
                            lines = File.ReadAllLines(fullFilePath);
                        }
                    }
                });
            return lines;
        }

        private string GetFileFullPath(string parentFolderRelativePath, string fileName)
        {
            string fullFilePath;
            if (parentFolderRelativePath != null)
                fullFilePath = Path.Combine(this._dataRecordStorageSettings.RootFolderPath, parentFolderRelativePath, fileName);
            else
                fullFilePath = Path.Combine(this._dataRecordStorageSettings.RootFolderPath, fileName);
            return fullFilePath;
        }

        private void GetBatchTimesFromRecordTime(DateTime recordTime, out DateTime batchStartTime, out DateTime batchEndTime)
        {
            TimeSpan storagePartitionInterval = this._dataRecordStorageSettings.StoragePartitionInterval;
            int nbOfSecondsToAdd = (int)(storagePartitionInterval.TotalSeconds * ((recordTime - recordTime.Date).TotalSeconds / storagePartitionInterval.TotalSeconds));
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
                FileName = reader.GetString(COL_FileName),
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