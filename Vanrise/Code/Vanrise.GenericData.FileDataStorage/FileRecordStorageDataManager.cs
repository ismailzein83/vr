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

namespace Vanrise.GenericData.FileDataStorage
{
    public class FileRecordStorageDataManager : IDataRecordDataManager
    {
        #region RDB Metadata Columns

        const string COL_ID = "ID";
        const string COL_FileName = "FileName";
        const string COL_FromTime = "FromTime";
        const string COL_ToTime = "ToTime";
        const string COL_FromID = "FromID";
        const string COL_ToID = "ToID";
        const string COL_IsReady = "IsReady";
        const string COL_CreatedTime = "CreatedTime";

        #endregion

        #region ctor/Fields

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
                List<FileRecordStorageRecordInfo> records = new List<FileRecordStorageRecordInfo>(batch.Records);
                var queryContext = new RDBQueryContext(GetDataProvider());
                var selectQuery = queryContext.AddSelectQuery();
                selectQuery.From(preparedConfig.MetadataRDBTableQuerySource, "mtdata", 1, true);
                selectQuery.SelectColumns().AllTableColumns("mtdata");
                var where = selectQuery.Where();
                where.EqualsCondition(COL_FromTime).Value(batch.FromTime);
                where.EqualsCondition(COL_ToTime).Value(batch.ToTime);
                LockInsertBatchRange(batch.FromTime, batch.ToTime,
                    () =>
                    {
                        FileRecordStorageFileMetadata fileMetadata = queryContext.GetItem(FileRecordStorageFileMetadataMapper);
                        long fileMetadataId;
                        string fileName;
                        if(fileMetadata != null)
                        {
                            fileMetadataId = fileMetadata.FileRecordStorageFileMetadataId;
                            fileName = fileMetadata.FileName;
                            List<FileRecordStorageRecordInfo> existingRecordInfos = ReadRecordInfosFromFileWithLock(fileMetadata.FileRecordStorageFileMetadataId, fileMetadata.FileName);
                            records.AddRange(existingRecordInfos);
                            string regExPattern = @"--(.*?)\.vrrec";
                            var fileSeqMatch = System.Text.RegularExpressions.Regex.Match(fileName, regExPattern);
                            string fileSeqValue = fileSeqMatch.Value;
                            long existingSequence;
                            if (!long.TryParse(fileSeqValue, out existingSequence))
                                throw new Exception($"Cannot parse fileSeqValue '{fileSeqValue}'. File Name is '{fileName}");
                            fileName = fileName.Replace(fileSeqValue, (existingSequence + 1).ToString().PadLeft(10, '0'));
                        }
                        else
                        {
                            queryContext = new RDBQueryContext(GetDataProvider());
                            var insertQuery = queryContext.AddInsertQuery();
                            insertQuery.IntoTable(preparedConfig.MetadataRDBTableQuerySource);
                            insertQuery.AddSelectGeneratedId();
                            insertQuery.Column(COL_FromTime).Value(batch.FromTime);
                            insertQuery.Column(COL_ToTime).Value(batch.ToTime);
                            fileMetadataId = queryContext.ExecuteScalar().LongValue;
                            fileName = $"{fileMetadataId.ToString().PadLeft(10, '0')}-{batch.FromTime.ToString("yyyyMMdd HHmmss fff")}--0000000001.vrrec";
                        }
                        
                        string parentFolderPath = this._dataRecordStorageSettings.RootFolderPath;
                        string targetFileFullPath = Path.Combine(parentFolderPath, fileName);
                        Char fieldSeparator = this._dataRecordStorageSettings.FieldSeparator;
                        long minRecordId = long.MaxValue;
                        long maxRecordId = long.MinValue;
                        using (StreamWriter sw = new StreamWriter(targetFileFullPath))
                        {
                            foreach(var record in records.OrderBy(rec => rec.RecordTime))
                            {
                                sw.WriteLine(string.Concat(Vanrise.Data.BaseDataManager.GetDateTimeForBCP(record.RecordTime), fieldSeparator, record.RecordId, fieldSeparator, record.RecordContent));
                                if (record.RecordId < minRecordId)
                                    minRecordId = record.RecordId;
                                if (record.RecordId > maxRecordId)
                                    maxRecordId = record.RecordId;
                            }
                        }
                        queryContext = new RDBQueryContext(GetDataProvider());
                        var updateQuery = queryContext.AddUpdateQuery();
                        updateQuery.FromTable(preparedConfig.MetadataRDBTableQuerySource);
                        updateQuery.Column(COL_FileName).Value(fileName);
                        updateQuery.Column(COL_FromID).Value(minRecordId);
                        updateQuery.Column(COL_ToID).Value(maxRecordId);
                        queryContext.ExecuteNonQuery();
                    });
            }
        }

        public List<DataRecord> GetFilteredDataRecords(IDataRecordDataManagerGetFilteredDataRecordsContext context)
        {
            throw new NotImplementedException();
        }

        public void GetDataRecords(DateTime? from, DateTime? to, RecordFilterGroup recordFilterGroup, Func<bool> shouldStop, Action<dynamic> onItemReady, string orderColumnName = null, bool isOrderAscending = false)
        {
            var queryContext = new RDBQueryContext(GetDataProvider());
            var selectQuery = queryContext.AddSelectQuery();
            selectQuery.From(GetPreparedConfig().MetadataRDBTableQuerySource, "mtdata", null, true);
            selectQuery.SelectColumns().AllTableColumns("mtdata");
            var where = selectQuery.Where();
            if (to.HasValue)
                where.LessOrEqualCondition(COL_FromTime).Value(to.Value);
            if (from.HasValue)
                where.GreaterThanCondition(COL_ToTime).Value(from.Value);
            List<FileRecordStorageFileMetadata> fileMetadatas = queryContext.GetItems(FileRecordStorageFileMetadataMapper);
            foreach(var fileMetadata in fileMetadatas)
            {
                bool filterFromTime = from.HasValue && from.Value > fileMetadata.FromTime;
                bool filterToTime = to.HasValue && to.Value < fileMetadata.ToTime;
                var recordInfos = ReadRecordInfosFromFileWithLock(fileMetadata.FileRecordStorageFileMetadataId, fileMetadata.FileName);
                foreach(var recordInfo in recordInfos)
                {
                    if(filterFromTime)
                    {
                        if (recordInfo.RecordTime < from.Value)
                            continue;
                    }
                    if(filterToTime)
                    {
                        if (recordInfo.RecordTime > to.Value)
                            break;
                    }

                }
            }
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
            throw new NotImplementedException();
        }

        public DateTime? GetMinDateTimeWithMaxIdAfterId(long id, string idFieldName, string dateTimeFieldName, out long? maxId)
        {
            throw new NotImplementedException();
        }

        public void DeleteRecords(DateTime fromDate, DateTime toDate, List<long> idsToDelete, string idFieldName, string dateTimeFieldName)
        {
            throw new NotImplementedException();
        }

        public long? GetMaxId(string idFieldName, string dateTimeFieldName, out DateTime? maxDate, out DateTime? minDate)
        {
            throw new NotImplementedException();
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
                                    metadataRDBColumns.Add(COL_FileName, new RDBTableColumnDefinition { DataType = RDBDataType.NVarchar, Size = 255 });
                                    metadataRDBColumns.Add(COL_FromTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_ToTime, new RDBTableColumnDefinition { DataType = RDBDataType.DateTime });
                                    metadataRDBColumns.Add(COL_FromID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
                                    metadataRDBColumns.Add(COL_ToID, new RDBTableColumnDefinition { DataType = RDBDataType.BigInt });
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

        private void LockInsertBatchRange(DateTime batchFromTime, DateTime batchToTime, Action onLocked)
        {            
        }
        
        private List<FileRecordStorageRecordInfo> ReadRecordInfosFromFileWithLock(long fileRecordStorageFileMetadataId, string fileName)
        {
            string[] lines = ReadFileWithLock(fileRecordStorageFileMetadataId, fileName);
            List<FileRecordStorageRecordInfo> recordInfos = new List<FileRecordStorageRecordInfo>();
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
            return recordInfos;
        }

        private string[] ReadFileWithLock(long fileRecordStorageFileMetadataId, string fileName)
        {
            return File.ReadAllLines(Path.Combine(this._dataRecordStorageSettings.RootFolderPath, fileName));
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
                FileName = reader.GetString(COL_FileName),
                FromTime = reader.GetDateTime(COL_FromTime),
                ToTime = reader.GetDateTime(COL_ToTime),
                FromId = reader.GetLong(COL_FromID),
                ToId = reader.GetLong(COL_ToID)
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
}