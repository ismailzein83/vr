using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Entities;
using Vanrise.GenericData.Data;
using Vanrise.GenericData.Entities;
using Vanrise.Common;
using System.Collections.Concurrent;
using System.Reflection;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.Common.Business;
using System.Collections;
using Vanrise.Common.Excel;
using Aspose.Cells;
using Vanrise.GenericData.Entities.GenericRules;
using System.Drawing;
using System.IO;
using Vanrise.BusinessProcess.Business;

namespace Vanrise.GenericData.Business
{
    public class GenericBusinessEntityManager : BaseBusinessEntityManager, IGenericBusinessEntityManager
    {
        #region Fields / Constructors
        GenericBusinessEntityDefinitionManager _genericBEDefinitionManager;
        DataRecordStorageManager _dataRecordStorageManager;
        SecurityManager s_securityManager;
        VRConnectionManager _connectionManager;
        public GenericBusinessEntityManager()
        {
            _genericBEDefinitionManager = new GenericBusinessEntityDefinitionManager();
            _dataRecordStorageManager = new DataRecordStorageManager();
            s_securityManager = new SecurityManager();
            _connectionManager = new VRConnectionManager();
        }
        #endregion



        #region Public Methods

        static DataRecordStorageManager.RecordCacheManager s_cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<DataRecordStorageManager.RecordCacheManager>();
        DataRecordStorageManager.RecordCacheManager GetCacheManager()
        {
            return s_cacheManager;
        }
        public R GetCachedOrCreate<R>(Object cacheName, Guid businessEntityDefinitionId, Func<R> createObject)
        {
            var dataRecordStorageId = _genericBEDefinitionManager.GetGenericBEDataRecordStorageId(businessEntityDefinitionId);
            return GetCacheManager().GetOrCreateObject(cacheName, dataRecordStorageId, createObject);
        }

        //public void SetCacheExpired(Guid businessEntityDefinitionId)
        //{
        //    var dataRecordStorageId = _genericBEDefinitionManager.GetGenericBEDataRecordStorageId(businessEntityDefinitionId);
        //    GetCacheManager().SetCacheExpired(dataRecordStorageId);
        //}


        public IEnumerable<GenericBusinessEntityInfo> GetGenericBusinessEntityInfo(Guid businessEntityDefinitionId, GenericBusinessEntityInfoFilter filter)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            if (genericBEDefinitionSetting.DataRecordStorageId.HasValue)
            {
                var dataRecordStorage = _dataRecordStorageManager.GetDataRecordStorage(genericBEDefinitionSetting.DataRecordStorageId.Value);
                if (!dataRecordStorage.Settings.EnableUseCaching)
                    throw new Exception($"Caching not enabled for Data Record Storage {genericBEDefinitionSetting.DataRecordStorageId.Value}");

                List<string> columns = new List<string>();
                List<string> columnTitles = new List<string>();
                var dataRecordFields = _genericBEDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);
                foreach (var field in dataRecordFields)
                {
                    columns.Add(field.Key);
                    columnTitles.Add(field.Value.Title);
                }
                var idDataRecordField = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(businessEntityDefinitionId);
                var storageRecords = _dataRecordStorageManager.GetAllDataRecords(genericBEDefinitionSetting.DataRecordStorageId.Value, columns, null);


                List<GenericBusinessEntity> genericBusinessEntities = new List<GenericBusinessEntity>();

                if (storageRecords != null && storageRecords.Count() > 0)
                {
                    foreach (var storageRecord in storageRecords)
                    {
                        GenericBusinessEntity genericBusinessEntity = new GenericBusinessEntity()
                        {
                            FieldValues = new Dictionary<string, object>()
                        };
                        if (storageRecord.FieldValues != null)
                        {
                            foreach (var fieldValue in storageRecord.FieldValues)
                            {
                                genericBusinessEntity.FieldValues.Add(fieldValue.Key, fieldValue.Value);
                            }
                        }
                        genericBusinessEntities.Add(genericBusinessEntity);
                    }
                }
                Func<GenericBusinessEntity, bool> filterFunc = (genericBE) =>
                {
                    if (filter != null)
                    {
                        if (filter.Filters != null && filter.Filters.Count() > 0)
                        {
                            var context = new GenericBusinessEntityFilterContext
                            {
                                GenericBusinessEntity = genericBE
                            };
                            foreach (var genericBEFilter in filter.Filters)
                            {
                                if (!genericBEFilter.IsMatch(context))
                                    return false;
                            }
                        }
                    }
                    return true;
                };
                return genericBusinessEntities.MapRecords((record) =>
                {
                    return GenericBusinessEntityInfoMapper(businessEntityDefinitionId, record);
                }, filterFunc);
            }
            else
            {
                var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(genericBEDefinitionSetting.VRConnectionId.Value);
                VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                string serializedFilter = filter != null ? Vanrise.Common.Serializer.Serialize(filter) : null;
                return connectionSettings.Get<IEnumerable<GenericBusinessEntityInfo>>(string.Format("/api/VR_GenericData/GenericBusinessEntity/GetGenericBusinessEntityInfo?businessEntityDefinitionId={0}&serializedFilter={1}", genericBEDefinitionSetting.RemoteGenericBEDefinitionId.Value, serializedFilter));
            }

        }
        public GenericBusinessEntityRuntimeEditor GetGenericBusinessEntityEditorRuntime(Guid businessEntityDefinitionId, Object genericBusinessEntityId, int? historyId)
        {
            return new GenericBusinessEntityRuntimeEditor
            {
                GenericBusinessEntity = genericBusinessEntityId != null ? GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId) : historyId.HasValue ? new VRObjectTrackingManager().GetObjectDetailById(historyId.Value) as GenericBusinessEntity : null,
                DefinitionTitle = _genericBEDefinitionManager.GetGenericBEDefinitionTitle(businessEntityDefinitionId),
                GenericBEDefinitionSettings = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId),
                TitleFieldName = _genericBEDefinitionManager.GetGenericBEDefinitionTitleFieldName(businessEntityDefinitionId)
            };
        }
        public GenericBusinessEntity GetGenericBusinessEntity(Object genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            if (genericBEDefinitionSetting.DataRecordStorageId.HasValue)
            {
                var idDataRecordField = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(businessEntityDefinitionId);

                List<string> columns = new List<string>();
                List<string> columnTitles = new List<string>();
                var dataRecordFields = _genericBEDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);
                foreach (var field in dataRecordFields)
                {
                    columns.Add(field.Key);
                    columnTitles.Add(field.Value.Title);
                }

                //  GetGridColumnNamesAndTitles(businessEntityDefinitionId, out columns, out columnTitles);

                var storageRecords = _dataRecordStorageManager.GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
                {
                    FromRow = 0,
                    ToRow = 2,
                    SortByColumnName = string.Format("FieldValues.{0}.Description", idDataRecordField.Name),
                    Query = new DataRecordQuery
                    {
                        Columns = columns,
                        Filters = new List<DataRecordFilter>
                    {
                        new DataRecordFilter {
                            FieldName = idDataRecordField.Name,
                            FilterValues =  new List<object> {
                                genericBusinessEntityId
                            }
                        }
                    },
                        FromTime = DateTime.MinValue,
                        ColumnTitles = columnTitles,
                        DataRecordStorageIds = new List<Guid> { genericBEDefinitionSetting.DataRecordStorageId.Value },
                        //Direction=,
                      //  LimitResult = 1000,
                        //SortColumns=,
                        //ToTime
                    },
                }) as BigResult<DataRecordDetail>;
                if (storageRecords.Data != null)
                {
                    var item = storageRecords.Data.FirstOrDefault();
                    if (item != null && item.FieldValues != null)
                    {
                        Dictionary<string, Object> fieldValues = new Dictionary<string, Object>();
                        foreach (var fieldValue in item.FieldValues)
                        {
                            if (fieldValue.Value.Value != null)
                                fieldValues.Add(fieldValue.Key, fieldValue.Value.Value);
                        }
                        return new GenericBusinessEntity
                        {
                            FieldValues = fieldValues,
                        };
                    }

                }
                return null;
            }
            else
            {
                var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(genericBEDefinitionSetting.VRConnectionId.Value);
                VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                return connectionSettings.Get<GenericBusinessEntity>(string.Format("/api/VR_GenericData/GenericBusinessEntity/GetGenericBusinessEntity?businessEntityDefinitionId={0}&genericBusinessEntityId={1}", genericBEDefinitionSetting.RemoteGenericBEDefinitionId.Value, genericBusinessEntityId));
            }

        }

        public GenericBusinessEntityDetail GetGenericBusinessEntityDetail(Object genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            var genericBusinessEntity = GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
            if (genericBusinessEntity == null)
                return null;
            return GenericBusinessEntityDetailMapper(businessEntityDefinitionId, genericBusinessEntity);
        }

        public string GetGenericBusinessEntityName(Object genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            var genericBusinessEntity = GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
            genericBEDefinitionSetting.TitleFieldName.ThrowIfNull("genericBEDefinitionSetting.TitleFieldName");
            var titleFieldType = _genericBEDefinitionManager.GetDataRecordTypeFieldByBEDefinitionId(businessEntityDefinitionId, genericBEDefinitionSetting.TitleFieldName);
            titleFieldType.ThrowIfNull("titleFieldType");
            if (genericBusinessEntity != null && genericBusinessEntity.FieldValues != null)
            {
                var fieldValue = genericBusinessEntity.FieldValues.GetRecord(genericBEDefinitionSetting.TitleFieldName);
                if (fieldValue != null)
                {
                    return titleFieldType.Type.GetDescription(fieldValue);
                }
            }
            return null;
        }

        public List<GenericBusinessEntity> GetAllGenericBusinessEntities(Guid businessEntityDefinitionId, List<string> columnsNeeded = null, RecordFilterGroup filterGroup = null)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", businessEntityDefinitionId);
            if (!genericBEDefinitionSetting.DataRecordStorageId.HasValue)
            {
                throw new NullReferenceException("genericBEDefinitionSetting.DataRecordStorageId");
            }
            var dataRecords = _dataRecordStorageManager.GetAllDataRecords(genericBEDefinitionSetting.DataRecordStorageId.Value, columnsNeeded, filterGroup);
            List<GenericBusinessEntity> results = new List<GenericBusinessEntity>();

            if (dataRecords != null)
            {
                foreach (var storageRecord in dataRecords)
                {
                    GenericBusinessEntity genericBusinessEntity = new GenericBusinessEntity();
                    if (storageRecord.FieldValues != null)
                    {
                        genericBusinessEntity.FieldValues = new Dictionary<string, object>();

                        foreach (var fieldValue in storageRecord.FieldValues)
                        {
                            genericBusinessEntity.FieldValues.Add(fieldValue.Key, fieldValue.Value);
                        }
                    }
                    results.Add(genericBusinessEntity);
                }
            }

            return results;

        }

        public IDataRetrievalResult<GenericBusinessEntityDetail> GetFilteredGenericBusinessEntities(DataRetrievalInput<GenericBusinessEntityQuery> input)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(input.Query.BusinessEntityDefinitionId);
            genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", input.Query.BusinessEntityDefinitionId);
            var queryInterceptorContext = new GenericBEOnBeforeGetFilteredHandlerPrepareQueryContext()
            {
                Query = input.Query,
                VRConnectionId = genericBEDefinitionSetting.VRConnectionId
            };
            if (genericBEDefinitionSetting.OnBeforeGetFilteredHandler != null)
                genericBEDefinitionSetting.OnBeforeGetFilteredHandler.PrepareQuery(queryInterceptorContext);

            if (genericBEDefinitionSetting.DataRecordStorageId.HasValue)
            {
                Func<GenericBusinessEntity, bool> filterExpression = (genericBusinessEntity) =>
                {
                    return true;
                };
                List<string> columns = new List<string>();
                List<string> columnTitles = new List<string>();
                GetGridColumnNamesAndTitles(input.Query.BusinessEntityDefinitionId, out columns, out columnTitles);

                var genericBeActions = _genericBEDefinitionManager.GetCachedGenericBEActionsByActionId(input.Query.BusinessEntityDefinitionId);

                var actionContext = new GenericBEActionDefinitionCheckAccessContext
                {
                    BusinessEntityDefinitionId = input.Query.BusinessEntityDefinitionId,
                    UserId = SecurityContext.Current.GetLoggedInUserId()
                };
                var viewContext = new GenericBEViewDefinitionCheckAccessContext
                {
                    BusinessEntityDefinitionId = input.Query.BusinessEntityDefinitionId,
                    UserId = SecurityContext.Current.GetLoggedInUserId()
                };
                string sortByColumnName = input.SortByColumnName;

                var storageRecordsResult = _dataRecordStorageManager.GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
                {
                    FromRow = input.FromRow,
                    ToRow = input.ToRow,
                    SortByColumnName = sortByColumnName,
                    IsAPICall = input.IsAPICall,
                    IsSortDescending = input.IsSortDescending,
                    ResultKey = input.ResultKey,
                    GetSummary = input.GetSummary,
                    DataRetrievalResultType = input.DataRetrievalResultType,
                    Query = new DataRecordQuery
                    {
                        Columns = columns,
                        FilterGroup = input.Query.FilterGroup,
                        Filters = input.Query.Filters != null ? input.Query.Filters.MapRecords(x =>
                            new DataRecordFilter
                            {
                                FieldName = x.FieldName,
                                FilterValues = x.FilterValues
                            }
                        ).ToList() : null,
                        FromTime = input.Query.FromTime.HasValue ? input.Query.FromTime.Value : DateTime.MinValue,
                        ToTime = input.Query.ToTime.HasValue ? input.Query.ToTime.Value : DateTime.MaxValue,
                        ColumnTitles = columnTitles,
                        DataRecordStorageIds = new List<Guid> { genericBEDefinitionSetting.DataRecordStorageId.Value },
                        Direction = OrderDirection.Descending,
                        LimitResult = input.Query.LimitResult,
                        BulkActionState = input.Query.BulkActionState
                        //SortColumns=,
                        //ToTime
                    },
                });

                if (input.DataRetrievalResultType == DataRetrievalResultType.Excel)
                {
                    var storageRecords1 = storageRecordsResult as ExcelResult<DataRecordDetail>;
                    return new ExcelResult<GenericBusinessEntityDetail>
                    {
                        ExcelFileContent = storageRecords1.ExcelFileContent,
                        ExcelFileStream = storageRecords1.ExcelFileStream
                    };
                }

                var storageRecords = storageRecordsResult as BigResult<DataRecordDetail>;

                BigResult<GenericBusinessEntityDetail> result = new BigResult<GenericBusinessEntityDetail>();
                if (storageRecords != null)
                {
                    result.ResultKey = storageRecords.ResultKey;
                    result.TotalCount = storageRecords.TotalCount;
                    List<GenericBusinessEntityDetail> resultDetail = new List<GenericBusinessEntityDetail>();
                    if (storageRecords.Data != null)
                    {
                        foreach (var storageRecord in storageRecords.Data)
                        {
                            GenericBusinessEntity genericBusinessEntity = new GenericBusinessEntity();
                            if (storageRecord.FieldValues != null)
                            {
                                genericBusinessEntity.FieldValues = new Dictionary<string, object>();

                                foreach (var fieldValue in storageRecord.FieldValues)
                                {
                                    genericBusinessEntity.FieldValues.Add(fieldValue.Key, fieldValue.Value.Value);
                                }
                            }

                            resultDetail.Add(GenericBusinessEntityDetailMapper(input.Query.BusinessEntityDefinitionId, genericBusinessEntity));
                        }
                    }

                    result.Data = resultDetail;
                }
                VRActionLogger.Current.LogGetFilteredAction(new GenericBusinessEntityLoggableEntity(input.Query.BusinessEntityDefinitionId), input);
                //   var vrCases =  storageDataManager.GetFilteredDataRecords()
                //  return DataRetrievalManager.Instance.ProcessResult(input, vrCases.ToBigResult(input, filterExpression, VRCaseDetailMapper));
                return result;
            }
            else
            {
                var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(genericBEDefinitionSetting.VRConnectionId.Value);
                VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                var clonedInput = Vanrise.Common.Utilities.CloneObject<DataRetrievalInput<GenericBusinessEntityQuery>>(input);
                clonedInput.Query.BusinessEntityDefinitionId = genericBEDefinitionSetting.RemoteGenericBEDefinitionId.Value;
                clonedInput.IsAPICall = true;
                if (clonedInput.DataRetrievalResultType == DataRetrievalResultType.Excel)
                {
                    return connectionSettings.Post<DataRetrievalInput<GenericBusinessEntityQuery>, RemoteExcelResult<GenericBusinessEntityDetail>>("/api/VR_GenericData/GenericBusinessEntity/GetFilteredGenericBusinessEntities", clonedInput, true);
                }
                else
                {
                    var genericBusinessEntities = connectionSettings.Post<DataRetrievalInput<GenericBusinessEntityQuery>, BigResult<GenericBusinessEntityDetail>>("/api/VR_GenericData/GenericBusinessEntity/GetFilteredGenericBusinessEntities", clonedInput, true);
                    BigResult<GenericBusinessEntityDetail> result = new BigResult<GenericBusinessEntityDetail>();
                    if (genericBusinessEntities != null)
                    {
                        result.ResultKey = genericBusinessEntities.ResultKey;
                        result.TotalCount = genericBusinessEntities.TotalCount;
                        List<GenericBusinessEntityDetail> resultDetail = new List<GenericBusinessEntityDetail>();
                        if (genericBusinessEntities.Data != null)
                        {
                            foreach (var genericbe in genericBusinessEntities.Data)
                            {
                                GenericBusinessEntity genericBusinessEntity = new GenericBusinessEntity();
                                if (genericbe.FieldValues != null)
                                {
                                    genericBusinessEntity.FieldValues = new Dictionary<string, object>();

                                    foreach (var fieldValue in genericbe.FieldValues)
                                    {
                                        genericBusinessEntity.FieldValues.Add(fieldValue.Key, fieldValue.Value.Value);
                                    }
                                }
                                resultDetail.Add(GenericBusinessEntityDetailMapper(input.Query.BusinessEntityDefinitionId, genericBusinessEntity));
                            }
                        }
                        result.Data = resultDetail;
                    }
                    VRActionLogger.Current.LogGetFilteredAction(new GenericBusinessEntityLoggableEntity(input.Query.BusinessEntityDefinitionId), input);
                    return result;
                }
            }

        }
        public string GenericBusinessEntityDefinitionTitle(Guid businessEntityDefinitionId)
        {
            BusinessEntityDefinitionManager _manager = new BusinessEntityDefinitionManager();
            return _manager.GetBusinessEntityDefinition(businessEntityDefinitionId).Title;
        }
        public InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntityToAdd genericBusinessEntityToAdd)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(genericBusinessEntityToAdd.BusinessEntityDefinitionId);
            if (genericBEDefinitionSetting.OnBeforeGetFilteredHandler != null)
            {
                genericBEDefinitionSetting.OnBeforeGetFilteredHandler.onBeforeAdd(new GenericBEOnBeforeAddHandlerContext()
                {
                    GenericBusinessEntityToAdd = genericBusinessEntityToAdd,
                    VRConnectionId = genericBEDefinitionSetting.VRConnectionId
                });
            }
            if (genericBEDefinitionSetting.DataRecordStorageId.HasValue)
            {
                InsertOperationOutput<GenericBusinessEntityDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail>();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
                insertOperationOutput.InsertedObject = null;

                genericBusinessEntityToAdd.FieldValues = new DataRecordTypeManager().ParseDicValuesToFieldType(genericBEDefinitionSetting.DataRecordTypeId, genericBusinessEntityToAdd.FieldValues);

                var idFieldType = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(genericBusinessEntityToAdd.BusinessEntityDefinitionId);

                var outputResult = OnBeforeSaveHandler(genericBEDefinitionSetting, genericBusinessEntityToAdd.BusinessEntityDefinitionId, null, genericBusinessEntityToAdd, HandlerOperationType.Add);
                if (outputResult != null && outputResult.Result == false)
                {
                    if (outputResult.Messages != null)
                    {
                        insertOperationOutput.AdditionalMessages = new List<InsertAdditionalMessage>();
                        foreach (var message in outputResult.Messages)
                        {
                            insertOperationOutput.AdditionalMessages.Add(new InsertAdditionalMessage()
                            {
                                Result = Vanrise.Entities.InsertOperationResult.Failed,
                                Message = message
                            });
                        }
                    }
                    return insertOperationOutput;
                }
                var fieldTypes = _genericBEDefinitionManager.GetDataRecordTypeFields(genericBEDefinitionSetting.DataRecordTypeId);
                OnBeforeSaveMethod(fieldTypes, genericBusinessEntityToAdd.BusinessEntityDefinitionId, genericBusinessEntityToAdd, null);



                Object insertedId;
                bool hasInsertedId;
                bool insertActionSucc = _dataRecordStorageManager.AddDataRecord(genericBEDefinitionSetting.DataRecordStorageId.Value, genericBusinessEntityToAdd.FieldValues, out insertedId, out hasInsertedId);

                if (insertActionSucc)
                {
                    Object genericBusinessEntityId = hasInsertedId ? insertedId : genericBusinessEntityToAdd.FieldValues.GetRecord(idFieldType.Name);

                    var genericBusinessEntity = GetGenericBusinessEntity(genericBusinessEntityId, genericBusinessEntityToAdd.BusinessEntityDefinitionId);

                    UpdateStatusHistoryIfAvailable(genericBusinessEntityToAdd.BusinessEntityDefinitionId, genericBusinessEntity, genericBusinessEntityId);

                    if (hasInsertedId)
                    {
                        genericBusinessEntityToAdd.FieldValues.Add(idFieldType.Name, insertedId);
                    }
                    VRActionLogger.Current.TrackAndLogObjectAdded(new GenericBusinessEntityLoggableEntity(genericBusinessEntityToAdd.BusinessEntityDefinitionId), genericBusinessEntityToAdd);

                    OnAfterSaveHandler(genericBEDefinitionSetting, genericBusinessEntityToAdd.BusinessEntityDefinitionId, null, genericBusinessEntity, HandlerOperationType.Add);
                    OnAfterSaveMethod(fieldTypes, genericBusinessEntityToAdd.BusinessEntityDefinitionId, genericBusinessEntity, genericBusinessEntityId);

                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                    insertOperationOutput.InsertedObject = GenericBusinessEntityDetailMapper(genericBusinessEntityToAdd.BusinessEntityDefinitionId, genericBusinessEntity);
                }
                else
                    insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

                return insertOperationOutput;
            }
            else
            {
                var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(genericBEDefinitionSetting.VRConnectionId.Value);
                VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                var clonedInput = Vanrise.Common.Utilities.CloneObject<GenericBusinessEntityToAdd>(genericBusinessEntityToAdd);
                clonedInput.BusinessEntityDefinitionId = genericBEDefinitionSetting.RemoteGenericBEDefinitionId.Value;
                return connectionSettings.Post<GenericBusinessEntityToAdd, InsertOperationOutput<GenericBusinessEntityDetail>>("/api/VR_GenericData/GenericBusinessEntity/AddGenericBusinessEntity", clonedInput, true);
            }

        }
        public UpdateOperationOutput<GenericBusinessEntityDetail> UpdateGenericBusinessEntity(GenericBusinessEntityToUpdate genericBusinessEntityToUpdate)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(genericBusinessEntityToUpdate.BusinessEntityDefinitionId);
            if (genericBEDefinitionSetting.OnBeforeGetFilteredHandler != null)
            {
                genericBEDefinitionSetting.OnBeforeGetFilteredHandler.onBeforeUpdate(new GenericBEOnBeforeUpdateHandlerContext()
                {
                    GenericBusinessEntityToUpdate = genericBusinessEntityToUpdate,
                    VRConnectionId = genericBEDefinitionSetting.VRConnectionId
                });
            }
            if (genericBEDefinitionSetting.DataRecordStorageId.HasValue)
            {
                UpdateOperationOutput<GenericBusinessEntityDetail> updateOperationOutput = new UpdateOperationOutput<GenericBusinessEntityDetail>();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
                updateOperationOutput.UpdatedObject = null;

                var oldGenericBE = GetGenericBusinessEntity(genericBusinessEntityToUpdate.GenericBusinessEntityId, genericBusinessEntityToUpdate.BusinessEntityDefinitionId);

                GenericBusinessEntity newGenericBE = new GenericBusinessEntity();
                newGenericBE.FieldValues = new Dictionary<string, Object>(oldGenericBE.FieldValues);

                foreach (var oldfield in oldGenericBE.FieldValues)
                {
                    foreach (var field in genericBusinessEntityToUpdate.FieldValues)
                    {
                        if (oldfield.Key == field.Key) { newGenericBE.FieldValues[field.Key] = field.Value; break; }
                    }

                }

                foreach (var field in genericBusinessEntityToUpdate.FieldValues)
                {
                    if (!newGenericBE.FieldValues.Keys.Contains(field.Key))
                    {
                        newGenericBE.FieldValues.Add(field.Key, field.Value);
                    }
                }

                var idFieldType = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(genericBusinessEntityToUpdate.BusinessEntityDefinitionId);
                genericBusinessEntityToUpdate.GenericBusinessEntityId = idFieldType.Type.ParseValueToFieldType(new DataRecordFieldTypeParseValueToFieldTypeContext(genericBusinessEntityToUpdate.GenericBusinessEntityId));

                genericBusinessEntityToUpdate.FieldValues = new DataRecordTypeManager().ParseDicValuesToFieldType(genericBEDefinitionSetting.DataRecordTypeId, genericBusinessEntityToUpdate.FieldValues);
                newGenericBE.FieldValues = new DataRecordTypeManager().ParseDicValuesToFieldType(genericBEDefinitionSetting.DataRecordTypeId, newGenericBE.FieldValues);

                var fieldTypes = _genericBEDefinitionManager.GetDataRecordTypeFields(genericBEDefinitionSetting.DataRecordTypeId);
                OnBeforeSaveMethod(fieldTypes, genericBusinessEntityToUpdate.BusinessEntityDefinitionId, newGenericBE, genericBusinessEntityToUpdate.GenericBusinessEntityId);

                var outputResult = OnBeforeSaveHandler(genericBEDefinitionSetting, genericBusinessEntityToUpdate.BusinessEntityDefinitionId, oldGenericBE, newGenericBE, HandlerOperationType.Update);
                if (outputResult != null && outputResult.Result == false)
                {
                    if (outputResult.Messages != null)
                    {
                        updateOperationOutput.AdditionalMessages = new List<UpdateAdditionalMessage>();
                        foreach (var message in outputResult.Messages)
                        {
                            updateOperationOutput.AdditionalMessages.Add(new UpdateAdditionalMessage()
                            {
                                Result = Vanrise.Entities.UpdateOperationResult.Failed,
                                Message = message
                            });
                        }
                    }
                    return updateOperationOutput;
                }

                bool updateActionSucc = _dataRecordStorageManager.UpdateDataRecord(genericBEDefinitionSetting.DataRecordStorageId.Value, genericBusinessEntityToUpdate.GenericBusinessEntityId, genericBusinessEntityToUpdate.FieldValues, genericBusinessEntityToUpdate.FilterGroup);

                if (updateActionSucc)
                {

                    var genericBusinessEntity = GetGenericBusinessEntity(genericBusinessEntityToUpdate.GenericBusinessEntityId, genericBusinessEntityToUpdate.BusinessEntityDefinitionId);

                    UpdateStatusHistoryIfAvailable(genericBusinessEntityToUpdate.BusinessEntityDefinitionId, genericBusinessEntity, genericBusinessEntityToUpdate.GenericBusinessEntityId);


                    VRActionLogger.Current.TrackAndLogObjectUpdated(new GenericBusinessEntityLoggableEntity(genericBusinessEntityToUpdate.BusinessEntityDefinitionId), genericBusinessEntityToUpdate, oldGenericBE);

                    OnAfterSaveHandler(genericBEDefinitionSetting, genericBusinessEntityToUpdate.BusinessEntityDefinitionId, oldGenericBE, newGenericBE, HandlerOperationType.Update);
                    OnAfterSaveMethod(fieldTypes, genericBusinessEntityToUpdate.BusinessEntityDefinitionId, newGenericBE, genericBusinessEntityToUpdate.GenericBusinessEntityId);

                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                    updateOperationOutput.UpdatedObject = GenericBusinessEntityDetailMapper(genericBusinessEntityToUpdate.BusinessEntityDefinitionId, genericBusinessEntity);
                }
                else
                {
                    updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
                }

                return updateOperationOutput;
            }
            else
            {
                var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(genericBEDefinitionSetting.VRConnectionId.Value);
                VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                var clonedInput = Vanrise.Common.Utilities.CloneObject<GenericBusinessEntityToUpdate>(genericBusinessEntityToUpdate);
                clonedInput.BusinessEntityDefinitionId = genericBEDefinitionSetting.RemoteGenericBEDefinitionId.Value;
                return connectionSettings.Post<GenericBusinessEntityToUpdate, UpdateOperationOutput<GenericBusinessEntityDetail>>("/api/VR_GenericData/GenericBusinessEntity/UpdateGenericBusinessEntity", clonedInput, true);
            }
        }

        public DeleteOperationOutput<object> DeleteGenericBusinessEntity(DeleteGenericBusinessEntityInput input)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(input.BusinessEntityDefinitionId);
            if (genericBEDefinitionSetting.DataRecordStorageId.HasValue)
            {
                var deleteOperationOutput = new Vanrise.Entities.DeleteOperationOutput<object>()
                {
                    Result = Vanrise.Entities.DeleteOperationResult.Failed
                };
                if (input.GenericBusinessEntityIds != null && input.GenericBusinessEntityIds.Count > 0)
                {
                    List<GenericBusinessEntity> oldGenericBEs = new List<GenericBusinessEntity>();
                    foreach (var genericBusinessEntityId in input.GenericBusinessEntityIds)
                    {
                        var oldGenericBE = GetGenericBusinessEntity(genericBusinessEntityId, input.BusinessEntityDefinitionId);
                        oldGenericBEs.Add(oldGenericBE);
                    }
                    bool deleteActionSucc = _dataRecordStorageManager.DeleteDataRecord(genericBEDefinitionSetting.DataRecordStorageId.Value, input.GenericBusinessEntityIds);
                    if (deleteActionSucc)
                    {
                        foreach (var oldGenericBE in oldGenericBEs)
                        {
                            VRActionLogger.Current.TrackAndLogObjectDeleted(new GenericBusinessEntityLoggableEntity(input.BusinessEntityDefinitionId), oldGenericBE);
                        }
                        deleteOperationOutput.Result = Vanrise.Entities.DeleteOperationResult.Succeeded;
                    }
                }
                return deleteOperationOutput;
            }
            else
            {
                var vrConnection = _connectionManager.GetVRConnection<VRInterAppRestConnection>(genericBEDefinitionSetting.VRConnectionId.Value);
                VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
                var clonedInput = Vanrise.Common.Utilities.CloneObject<DeleteGenericBusinessEntityInput>(input);
                clonedInput.BusinessEntityDefinitionId = genericBEDefinitionSetting.RemoteGenericBEDefinitionId.Value;
                return connectionSettings.Post<DeleteGenericBusinessEntityInput, DeleteOperationOutput<object>>(String.Format("/api/VR_GenericData/GenericBusinessEntity/DeleteGenericBusinessEntity"), clonedInput);
            }
        }

        public void LogObjectCustomAction(Guid businessEntityDefinitionId, GenericBusinessEntity genericBusinessEntity, string actionName, bool isObjectUpdated, string actionDescription)
        {
            VRActionLogger.Current.LogObjectCustomAction(new GenericBusinessEntityLoggableEntity(businessEntityDefinitionId), actionName, isObjectUpdated, genericBusinessEntity, actionDescription);
        }

        public byte[] DownloadGenericBusinessEntityTemplate(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            var dataRecordTypeFields = new DataRecordTypeManager().GetDataRecordTypeFields(genericBEDefinitionSetting.DataRecordTypeId);
            var excelFile = new VRExcelFile();
            var excelSheet = new VRExcelSheet();
            var index = 0;
            if (genericBEDefinitionSetting.UploadFields != null)
            {
                foreach (var item in genericBEDefinitionSetting.UploadFields)
                {
                    var dataRecordTypeField = dataRecordTypeFields.GetRecord(item.FieldName);
                    if (dataRecordTypeField != null)
                    {
                        excelSheet.AddCell(new VRExcelCell
                        {
                            RowIndex = 0,
                            ColumnIndex = index++,
                            Value = dataRecordTypeField.Title
                        });
                    }
                }
            }
            excelFile.AddSheet(excelSheet);
            return excelFile.GenerateExcelFile();
        }

        public UploadGenericBusinessEntityLog AddUploadedGenericBusinessEntities(Guid businessEntityDefinitionId, int fileId)
        {
            var uploadOutput = new UploadGenericBusinessEntityLog
            {
                NumberOfItemsAdded = 0,
                NumberOfItemsFailed = 0
            };
            string errorMessage = null;
            List<ParsedGenericBERow> parsedExcel = null;
            if (!ParseExcel(fileId, out errorMessage, out parsedExcel))
            {
                uploadOutput.ErrorMessage = errorMessage;
                return uploadOutput;
            }
            var addedGenericRows = new List<GenericBERowToAdd>();
            var invalidGenericRows = new List<GenericBEInvalidRow>();

            CreateGenericBEFromExcel(parsedExcel, businessEntityDefinitionId, invalidGenericRows, addedGenericRows);

            ReflectGenericBEsToDBAndExcel(addedGenericRows, invalidGenericRows, businessEntityDefinitionId, fileId, uploadOutput);

            return uploadOutput;
        }

        public ExecuteGenericBEBulkActionProcessOutput ExecuteGenericBEBulkActions(ExecuteGenericBEBulkActionProcessInput input)
        {
            input.BulkActionFinalState.ThrowIfNull("input.BulkActionFinalState");
            input.BulkActionFinalState.TargetItems.ThrowIfNull("input.BulkActionFinalState.TargetItems");
            if (input.BulkActionFinalState.TargetItems.Count == 0 && !input.BulkActionFinalState.IsAllSelected)
            {
                return new ExecuteGenericBEBulkActionProcessOutput { Succeed = false, OutputMessage = "At least one business entity must be selected." };
            }
            int userId = Vanrise.Security.Business.SecurityContext.Current.GetLoggedInUserId();

            GenericBEBulkActionProcessInput genericBEBulkActionProcessInput = new GenericBEBulkActionProcessInput()
            {
                BEDefinitionId = input.GenericBEDefinitionId,
                BulkActions = input.GenericBEBulkActions,
                BulkActionFinalState = input.BulkActionFinalState,
                HandlingErrorOption = input.HandlingErrorOption,
                UserId = userId
            };
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput
            {
                InputArguments = genericBEBulkActionProcessInput
            };
            var result = new BPInstanceManager().CreateNewProcess(createProcessInput);
            return new ExecuteGenericBEBulkActionProcessOutput { Succeed = true, ProcessInstanceId = result.ProcessInstanceId };
        }
        private bool ParseExcel(long fileId, out string errorMessage, out List<ParsedGenericBERow> parsedExcel)
        {
            VRFileManager fileManager = new VRFileManager();
            var file = fileManager.GetFile(fileId);
            file.ThrowIfNull("file", fileId);
            byte[] bytes = file.Content;
            var fileStream = new System.IO.MemoryStream(bytes);
            Workbook workbook = new Workbook(fileStream);
            Worksheet worksheet = workbook.Worksheets[0];
            errorMessage = null;
            parsedExcel = new List<ParsedGenericBERow>();

            var nbOfRows = worksheet.Cells.MaxRow + 1;
            var nbOfCols = worksheet.Cells.MaxColumn;

            if (nbOfRows == 1)
            {
                errorMessage = "Empty Template";
                return false;
            }

            for (int rowIndex = 1; rowIndex <= nbOfRows; rowIndex++)
            {
                ParsedGenericBERow parsedRow = new ParsedGenericBERow
                {
                    RowIndex = rowIndex,
                    ColumnValueByFieldName = new Dictionary<string, object>()
                };
                for (int colIndex = 0; colIndex <= nbOfCols; colIndex++)
                {
                    var header = worksheet.Cells[0, colIndex];
                    if (header.Value == null)
                        continue;

                    var cell = worksheet.Cells[rowIndex, colIndex];

                    if (cell != null)
                    {
                        if (parsedRow.ColumnValueByFieldName.ContainsKey(header.Value.ToString().Trim()))
                        {
                            errorMessage = "Invalid File Format";
                            return false;
                        }
                        else
                        {
                            if (cell.Value != null)
                                parsedRow.ColumnValueByFieldName.Add(header.Value.ToString().Trim(), cell.Value);
                        }
                    }

                }
                parsedExcel.Add(parsedRow);
            }

            return true;
        }

        private void CreateGenericBEFromExcel(List<ParsedGenericBERow> parsedExcel, Guid businessEntityDefinitionId, List<GenericBEInvalidRow> invalidGenericRows, List<GenericBERowToAdd> addedGenericRows)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", businessEntityDefinitionId);
            if (genericBEDefinitionSetting.ShowUpload == false)
                throw new NotSupportedException("Does not support bulk uploading");

            var dataRecordFields = new DataRecordTypeManager().GetDataRecordTypeFields(genericBEDefinitionSetting.DataRecordTypeId);
            var uploadFields = new Dictionary<string, DataRecordFieldExel>();
            bool invalid;

            if (genericBEDefinitionSetting.UploadFields != null)
            {
                foreach (var item in genericBEDefinitionSetting.UploadFields)
                {
                    var dataRecordField = dataRecordFields.GetRecord(item.FieldName);
                    if (dataRecordField != null)
                    {
                        uploadFields.Add(dataRecordField.Title, new DataRecordFieldExel
                        {
                            IsRequired = item.IsRequired,
                            DataRecordField = dataRecordField,
                            FieldName = item.FieldName
                        });
                    }
                }
            }


            if (parsedExcel != null && parsedExcel.Count != 0)
            {
                foreach (var parsedRow in parsedExcel)
                {
                    invalid = false;
                    var fieldValues = new Dictionary<string, object>();
                    if (parsedRow.ColumnValueByFieldName != null && parsedRow.ColumnValueByFieldName.Count == 0)
                    {
                        continue;
                    }

                    if (uploadFields != null)
                        foreach (var uploadField in uploadFields)
                        {
                            var fieldValue = parsedRow.ColumnValueByFieldName.GetRecord(uploadField.Key);
                            if (fieldValue == null && uploadField.Value.IsRequired)
                            {
                                invalidGenericRows.Add(new GenericBEInvalidRow
                                {
                                    ErrorMessage = uploadField.Key + " is  required ",
                                    RowIndex = parsedRow.RowIndex
                                });
                                invalid = true;
                                break;
                            }

                            if (fieldValue == null)
                            {
                                fieldValues.Add(uploadField.Value.FieldName, null);
                                continue;
                            }

                            GetValueByDescriptionContext getValueByDescriptionContext = new GetValueByDescriptionContext
                            {
                                FieldDescription = fieldValue.ToString(),
                                FieldType = uploadField.Value.DataRecordField.Type,
                            };

                            uploadField.Value.DataRecordField.Type.GetValueByDescription(getValueByDescriptionContext);
                            if (getValueByDescriptionContext.FieldValue == null)
                            {
                                invalidGenericRows.Add(new GenericBEInvalidRow
                                {
                                    ErrorMessage = getValueByDescriptionContext.FieldDescription + " Doesn't have a value",
                                    RowIndex = parsedRow.RowIndex,
                                });
                                invalid = true;
                                break;
                            }

                            fieldValues.Add(uploadField.Value.FieldName, getValueByDescriptionContext.FieldValue);
                        }

                    if (!invalid)
                    {
                        addedGenericRows.Add(new GenericBERowToAdd
                        {
                            RowIndex = parsedRow.RowIndex,
                            GenericBusinessEntityToAdd = new GenericBusinessEntityToAdd
                            {
                                BusinessEntityDefinitionId = businessEntityDefinitionId,
                                FieldValues = fieldValues,
                            }
                        });
                    }
                }
            }
        }

        public void ReflectGenericBEsToDBAndExcel(List<GenericBERowToAdd> addedGenericRows, List<GenericBEInvalidRow> invalidGenericRows, Guid businessEntityDefinitionId, long uploadFileId, UploadGenericBusinessEntityLog uploadOutput)
        {
            long outputFileId = 0;
            uploadOutput.NumberOfItemsFailed += invalidGenericRows.Count;
            VRFileManager manager = new VRFileManager();
            VRFile file = manager.GetFile(uploadFileId);
            var fileStream = new System.IO.MemoryStream(file.Content);
            Workbook UploadOutputWorkbook = new Workbook(fileStream);
            Vanrise.Common.Utilities.ActivateAspose();
            Worksheet UploadOutputWorksheet = UploadOutputWorkbook.Worksheets[0];

            UploadOutputWorksheet.Name = "Result";
            int colnum = UploadOutputWorksheet.Cells.MaxColumn + 1;
            UploadOutputWorksheet.Cells.SetColumnWidth(colnum, 20);
            UploadOutputWorksheet.Cells.SetColumnWidth(colnum + 1, 40);
            UploadOutputWorksheet.Cells[0, colnum].PutValue("Result");
            UploadOutputWorksheet.Cells[0, colnum + 1].PutValue("Error Message");

            Style headerStyle = new Style();
            headerStyle.Font.Name = "Times New Roman";
            headerStyle.Font.Color = Color.Red;
            headerStyle.Font.Size = 14;
            headerStyle.Font.IsBold = true;

            UploadOutputWorksheet.Cells[0, colnum].SetStyle(headerStyle);
            UploadOutputWorksheet.Cells[0, colnum + 1].SetStyle(headerStyle);

            Style cellStyle = new Style();
            cellStyle.Font.Name = "Times New Roman";
            cellStyle.Font.Color = Color.Black;
            cellStyle.Font.Size = 12;

            if (addedGenericRows != null)
            {
                foreach (var genericRow in addedGenericRows)
                {
                    var insertOperationOutput = this.AddGenericBusinessEntity(genericRow.GenericBusinessEntityToAdd);


                    switch (insertOperationOutput.Result)
                    {
                        case InsertOperationResult.Succeeded:
                            {
                                uploadOutput.NumberOfItemsAdded++;
                                UploadOutputWorksheet.Cells[genericRow.RowIndex, colnum].PutValue("Succeeded.");
                                UploadOutputWorksheet.Cells[genericRow.RowIndex, colnum + 1].PutValue("");
                                UploadOutputWorksheet.Cells[genericRow.RowIndex, colnum].SetStyle(cellStyle);
                                break;
                            }
                        case InsertOperationResult.SameExists:
                            {
                                uploadOutput.NumberOfItemsFailed++;
                                invalidGenericRows.Add(new GenericBEInvalidRow
                                {
                                    RowIndex = genericRow.RowIndex,
                                    ErrorMessage = $"An error occured while adding business entity to database: Same Entity already exists"
                                });
                                break;
                            }
                        case InsertOperationResult.Failed:
                            {
                                uploadOutput.NumberOfItemsFailed++;
                                StringBuilder errorMessage = new StringBuilder(insertOperationOutput.Message);
                                if (insertOperationOutput.AdditionalMessages != null && insertOperationOutput.AdditionalMessages.Count > 0)
                                    errorMessage.AppendLine(String.Join(". ", insertOperationOutput.AdditionalMessages.Select((item) => { return item.Result == InsertOperationResult.Failed ? item.Message : null; })));

                                invalidGenericRows.Add(new GenericBEInvalidRow
                                {
                                    RowIndex = genericRow.RowIndex,
                                    ErrorMessage = $"An error occured while adding business entity to database. {errorMessage}"
                                });
                                break;
                            }
                        default: throw new NotSupportedException(string.Format("InsertOperationResult '{0}' is not supported", insertOperationOutput.Result));
                    }
                }
            }

            if (invalidGenericRows != null)
            {
                foreach (var genericRowFailed in invalidGenericRows)
                {
                    UploadOutputWorksheet.Cells[genericRowFailed.RowIndex, colnum].PutValue("Failed.");
                    UploadOutputWorksheet.Cells[genericRowFailed.RowIndex, colnum + 1].PutValue(genericRowFailed.ErrorMessage);
                    UploadOutputWorksheet.Cells[genericRowFailed.RowIndex, colnum].SetStyle(cellStyle);
                    UploadOutputWorksheet.Cells[genericRowFailed.RowIndex, colnum + 1].SetStyle(cellStyle);
                }
            }

            MemoryStream memoryStream = new MemoryStream();
            memoryStream = UploadOutputWorkbook.SaveToStream();

            VRFile returnedFile = new VRFile() { Content = memoryStream.ToArray(), Name = "UploadGenericBusinessEntitiesOutput", Extension = ".xlsx", IsTemp = true };

            VRFileManager fileManager = new VRFileManager();
            outputFileId = fileManager.AddFile(returnedFile);
            uploadOutput.FileID = outputFileId;
        }

        public byte[] DownloadBusinessEntityLog(long fileID)
        {
            VRFileManager fileManager = new VRFileManager();
            VRFile file = fileManager.GetFile(fileID);
            return file.Content;
        }

        public GenericBEAction GetGenericBEAction(Guid businessEntityDefinitionId, Guid actionId)
        {
            var accountBEDefinitionSettings = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            if (accountBEDefinitionSettings != null && accountBEDefinitionSettings.GenericBEActions != null)
            {
                return accountBEDefinitionSettings.GenericBEActions.FindRecord(x => x.GenericBEActionId == actionId);
            }
            return null;
        }
        public string GetGenericBETitleFieldValue(Object genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            return GetGenericBusinessEntityName(genericBusinessEntityId, businessEntityDefinitionId);
        }

        #endregion

        #region Private Methods

        private bool UpdateStatusHistoryIfAvailable(Guid businessEntityDefinitionId, GenericBusinessEntity genericBusinessEntity, object genericBusinessEntityId)
        {
            var statusFieldNames = _genericBEDefinitionManager.GetStatusFieldNames(businessEntityDefinitionId);
            BusinessEntityStatusHistoryManager businessEntityStatusHistoryManager = new BusinessEntityStatusHistoryManager();
            bool result = true;
            if (statusFieldNames != null && statusFieldNames.Count > 0)
            {
                foreach (var statusFieldName in statusFieldNames)
                {
                    var statusValue = genericBusinessEntity.FieldValues.GetRecord(statusFieldName);
                    if (statusValue != null)
                    {
                        if (!businessEntityStatusHistoryManager.InsertStatusHistory(businessEntityDefinitionId, genericBusinessEntityId.ToString(), statusFieldName, Guid.Parse(statusValue.ToString())))
                            result = false;
                    }
                }
            }
            return result;
        }
        private void OnBeforeSaveMethod(Dictionary<string, DataRecordField> fieldTypes, Guid businessEntityDefinitionId, GenericBusinessEntity genericBusinessEntity, Object genericBusinessEntityId)
        {
            if (fieldTypes != null)
            {
                foreach (var fieldType in fieldTypes)
                {
                    var fieldValue = genericBusinessEntity.FieldValues.GetRecord(fieldType.Key);
                    if (fieldValue != null)
                    {
                        fieldType.Value.Type.onBeforeSave(new DataRecordFieldTypeOnBeforeSaveContext
                        {
                            BusinessEntityDefinitionId = businessEntityDefinitionId,
                            BusinessEntityId = genericBusinessEntityId,
                            FieldValue = fieldValue
                        });
                    }

                }
            }

        }
        private OutputResult OnBeforeSaveHandler(GenericBEDefinitionSettings genericBEDefinitionSetting, Guid businessEntityDefinitionId, GenericBusinessEntity oldEntity, GenericBusinessEntity newEntity, HandlerOperationType operationType)
        {

            if (genericBEDefinitionSetting.OnBeforeInsertHandler != null)
            {
                GenericBEOnBeforeInsertHandlerContext context = new GenericBEOnBeforeInsertHandlerContext()
                {
                    DefinitionSettings = genericBEDefinitionSetting,
                    GenericBusinessEntity = newEntity,
                    OldGenericBusinessEntity = oldEntity,
                    OperationType = operationType,
                    BusinessEntityDefinitionId = businessEntityDefinitionId,
                    OutputResult = new OutputResult()
                    {
                        Result = true,
                        Messages = new List<string>()
                    }
                };

                genericBEDefinitionSetting.OnBeforeInsertHandler.Execute(context);
                return context.OutputResult;
            }
            return null;
        }
        private void OnAfterSaveMethod(Dictionary<string, DataRecordField> fieldTypes, Guid businessEntityDefinitionId, GenericBusinessEntity genericBusinessEntity, Object genericBusinessEntityId)
        {
            if (fieldTypes != null)
            {
                foreach (var fieldType in fieldTypes)
                {
                    var fieldValue = genericBusinessEntity.FieldValues.GetRecord(fieldType.Key);
                    if (fieldValue != null)
                    {
                        fieldType.Value.Type.onAfterSave(new DataRecordFieldTypeOnAfterSaveContext
                        {
                            BusinessEntityDefinitionId = businessEntityDefinitionId,
                            BusinessEntityId = genericBusinessEntityId,
                            FieldValue = fieldValue
                        });
                    }

                }
            }
        }
        private void OnAfterSaveHandler(GenericBEDefinitionSettings genericBEDefinitionSetting, Guid businessEntityDefinitionId, GenericBusinessEntity oldEntity, GenericBusinessEntity newEntity, HandlerOperationType operationType)
        {
            if (genericBEDefinitionSetting.OnAfterSaveHandler != null)
            {
                genericBEDefinitionSetting.OnAfterSaveHandler.Execute(new GenericBEOnAfterSaveHandlerContext
                {
                    DefinitionSettings = genericBEDefinitionSetting,
                    NewEntity = newEntity,
                    BusinessEntityDefinitionId = businessEntityDefinitionId,
                    OldEntity = oldEntity,
                    OperationType = operationType
                });
            }
        }
        private void GetGridColumnNamesAndTitles(Guid businessEntityDefinitionId, out List<string> columnNames, out List<string> columnTitles)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            columnNames = new List<string>();
            columnTitles = new List<string>();
            genericBEDefinitionSetting.GridDefinition.ThrowIfNull("genericBEDefinitionSetting.GridDefinition");
            genericBEDefinitionSetting.GridDefinition.ColumnDefinitions.ThrowIfNull("genericBEDefinitionSetting.GridDefinition.ColumnDefinitions");
            foreach (var gridColumn in genericBEDefinitionSetting.GridDefinition.ColumnDefinitions)
            {
                columnNames.Add(gridColumn.FieldName);
                columnTitles.Add(gridColumn.FieldTitle);
            }
        }

        private Dictionary<Object, GenericBusinessEntity> GetCachedGenericBEByTitleField(Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            if (genericBEDefinitionSetting.DataRecordStorageId.HasValue)
            {
                var dataRecordStorage = _dataRecordStorageManager.GetDataRecordStorage(genericBEDefinitionSetting.DataRecordStorageId.Value);
                if (!dataRecordStorage.Settings.EnableUseCaching)
                    throw new Exception($"Caching not enabled for Data Record Storage {genericBEDefinitionSetting.DataRecordStorageId.Value}");

                return GetCachedOrCreate<Dictionary<Object, GenericBusinessEntity>>("GetCachedGenericBEByTitleField", businessEntityDefinitionId, () =>
                {
                    Dictionary<object, GenericBusinessEntity> resultByTitleFieldName = new Dictionary<object, GenericBusinessEntity>();

                    var dataRecordFields = _genericBEDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);
                    if (dataRecordFields == null)
                        return null;

                    List<string> columns = new List<string>();
                    columns.AddRange(dataRecordFields.Keys);

                    var records = _dataRecordStorageManager.GetAllDataRecords(genericBEDefinitionSetting.DataRecordStorageId.Value, columns);
                    if (records == null || records.Count() == 0)
                        return null;

                    foreach (var record in records)
                    {
                        var titleValue = record.FieldValues.GetRecord(genericBEDefinitionSetting.TitleFieldName);
                        if (!resultByTitleFieldName.ContainsKey(titleValue))
                            resultByTitleFieldName.Add(titleValue, DataRecordStorageToGenericBEMapper(record));
                    }

                    return resultByTitleFieldName.Count > 0 ? resultByTitleFieldName : null;

                });
            }
            return null;
        }
        private GenericBusinessEntity DataRecordStorageToGenericBEMapper(DataRecord dataRecord)
        {
            if (dataRecord == null)
                return null;

            GenericBusinessEntity genericBE = new GenericBusinessEntity { FieldValues = new Dictionary<string, object>() };

            foreach (var field in dataRecord.FieldValues)
            {
                genericBE.FieldValues.Add(field.Key, field.Value);
            }
            return genericBE;
        }

        //private bool DoesUserHaveAccess(int userId, RequiredPermissionSettings requiredPermission)
        //{
        //    SecurityManager secManager = new SecurityManager();
        //    if (!secManager.IsAllowed(requiredPermission, userId))
        //        return false;
        //    return true;

        //}
        //private bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        //{
        //    int userId = SecurityContext.Current.GetLoggedInUserId();
        //    SecurityManager secManager = new SecurityManager();
        //    if (!secManager.IsAllowed(requiredPermission, userId))
        //        return false;
        //    return true;

        //}
        #endregion

        #region Private Classes

        public class GenericBusinessEntityLoggableEntity : VRLoggableEntityBase
        {
            Guid _businessEntityDefinitionId;
            static GenericBusinessEntityDefinitionManager s_genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
            static GenericBusinessEntityManager s_genericBusinessEntityManager = new GenericBusinessEntityManager();
            DataRecordField idDataRecordField;
            public GenericBusinessEntityLoggableEntity(Guid businessEntityDefinitionId)
            {
                _businessEntityDefinitionId = businessEntityDefinitionId;
                idDataRecordField = s_genericBusinessEntityDefinitionManager.GetIdFieldTypeForGenericBE(_businessEntityDefinitionId);
            }

            public override string EntityUniqueName
            {
                get { return String.Format("VR_GenericData_GenericBusinessEntity_{0}", _businessEntityDefinitionId); }
            }

            public override string EntityDisplayName
            {
                get { return s_genericBusinessEntityDefinitionManager.GetGenericBEDefinitionName(_businessEntityDefinitionId); }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_GenericData_GenericBusinessEntity_ViewHistoryItem"; }
            }


            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                GenericBusinessEntity genericBusinessEntity = context.Object.CastWithValidate<GenericBusinessEntity>("context.Object");
                genericBusinessEntity.FieldValues.ThrowIfNull("genericBusinessEntity.FieldValues");
                return genericBusinessEntity.FieldValues[idDataRecordField.Name];
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                GenericBusinessEntity genericBusinessEntity = context.Object.CastWithValidate<GenericBusinessEntity>("context.Object");
                genericBusinessEntity.FieldValues.ThrowIfNull("genericBusinessEntity.FieldValues");
                var objectId = genericBusinessEntity.FieldValues[idDataRecordField.Name];
                return s_genericBusinessEntityManager.GetGenericBusinessEntityName(objectId, _businessEntityDefinitionId);
            }

            public override string ModuleName
            {
                get { return "Generic Data"; }
            }

            public override VRActionAuditChangeInfoDefinition GetChangeInfoDefinition(IVRLoggableEntityGetChangeInfoDefinitionContext context)
            {
                return new GenericFieldsActionAuditChangeInfoDefinition
                {
                    BusinessEntityDefinitionId = _businessEntityDefinitionId
                };
            }
        }

        private class DataRecordFieldExel
        {
            public DataRecordField DataRecordField { get; set; }
            public bool IsRequired { get; set; }
            public string FieldName { get; set; }
        }
        #endregion

        #region Mappers
        private GenericBusinessEntityDetail GenericBusinessEntityDetailMapper(Guid businessEntityDefinitionId, GenericBusinessEntity genericBusinessEntity)
        {
            GenericBusinessEntityDetail genericBusinessEntityDetail = new GenericBusinessEntityDetail();
            genericBusinessEntityDetail.FieldValues = new GenericBusinessEntityValues();
            var dataRecordTypeFields = _genericBEDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);

            foreach (var fieldValue in genericBusinessEntity.FieldValues)
            {
                var dataRecordTypeField = dataRecordTypeFields.FindRecord(x => x.Name == fieldValue.Key);
                if (dataRecordTypeField != null)
                {
                    genericBusinessEntityDetail.FieldValues.Add(fieldValue.Key, new GenericBusinessEntityValue
                    {
                        Value = fieldValue.Value,
                        Description = dataRecordTypeField.Type.GetDescription(fieldValue.Value)
                    });
                }
            }

            var viewContext = new GenericBEViewDefinitionCheckAccessContext
            {
                BusinessEntityDefinitionId = businessEntityDefinitionId,
                UserId = SecurityContext.Current.GetLoggedInUserId()
            };

            var gridDefinition = _genericBEDefinitionManager.GetGenericBEDefinitionGridDefinition(businessEntityDefinitionId);
            if (gridDefinition != null)
            {
                if (gridDefinition.GenericBEGridActions != null)
                {
                    var genericBeActions = _genericBEDefinitionManager.GetCachedGenericBEActionsByActionId(businessEntityDefinitionId);
                    genericBusinessEntityDetail.AvailableGridActionIds = new List<Guid>();
                    var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
                    GenericBEActionFilterConditionContext genericBEActionFilterConditionContext = new GenericBEActionFilterConditionContext { Entity = genericBusinessEntity, DefinitionSettings = genericBEDefinitionSetting };
                    foreach (var genericBEGridAction in gridDefinition.GenericBEGridActions)
                    {
                        var actionContext = new GenericBEActionDefinitionCheckAccessContext
                        {
                            BusinessEntityDefinitionId = businessEntityDefinitionId,
                            UserId = SecurityContext.Current.GetLoggedInUserId(),
                            GenericBEAction = GetGenericBEAction(businessEntityDefinitionId, genericBEGridAction.GenericBEActionId)
                        };

                        if (genericBEGridAction.FilterCondition == null || genericBEGridAction.FilterCondition.IsFilterMatch(genericBEActionFilterConditionContext))
                        {

                            var genericBeAction = genericBeActions.GetRecord(genericBEGridAction.GenericBEActionId);
                            genericBeAction.ThrowIfNull("Generic Be Action ", genericBEGridAction.GenericBEActionId);

                            var settings = genericBeAction.Settings;
                            settings.ThrowIfNull("Generic Be Action Settings", genericBEGridAction.GenericBEActionId);


                            if (settings.DoesUserHaveAccess(actionContext))
                                genericBusinessEntityDetail.AvailableGridActionIds.Add(genericBEGridAction.GenericBEGridActionId);
                        }
                    }
                }
                if (gridDefinition.GenericBEGridViews != null)
                {
                    genericBusinessEntityDetail.AvailableGridViewIds = new List<Guid>();
                    foreach (var genericBEGridView in gridDefinition.GenericBEGridViews)
                    {
                        var viewSettings = genericBEGridView.Settings;
                        viewSettings.ThrowIfNull("Generic BE View Settings", genericBEGridView.GenericBEViewDefinitionId);
                        if (viewSettings.DoesUserHaveAccess(viewContext))
                            genericBusinessEntityDetail.AvailableGridViewIds.Add(genericBEGridView.GenericBEViewDefinitionId);
                    }
                }
            }

            return genericBusinessEntityDetail;
        }

        private GenericBusinessEntityInfo GenericBusinessEntityInfoMapper(Guid businessEntityDefinitionId, GenericBusinessEntity genericBusinessEntity)
        {
            var idDataRecordField = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(businessEntityDefinitionId);
            var titleFieldName = _genericBEDefinitionManager.GetGenericBEDefinitionTitleFieldName(businessEntityDefinitionId);
            var titleFieldType = _genericBEDefinitionManager.GetDataRecordTypeFieldByBEDefinitionId(businessEntityDefinitionId, titleFieldName);
            titleFieldType.ThrowIfNull("titleFieldType");
            GenericBusinessEntityInfo entityInfo = new GenericBusinessEntityInfo();
            entityInfo.GenericBusinessEntityId = genericBusinessEntity.FieldValues[idDataRecordField.Name];
            var titleValue = genericBusinessEntity.FieldValues[titleFieldName];
            entityInfo.Name = titleFieldType.Type.GetDescription(titleValue);
            return entityInfo;
        }
        #endregion

        #region BaseBusinessEntityManager
        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetGenericBusinessEntityName(context.EntityId, context.EntityDefinition.BusinessEntityDefinitionId);
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            throw new NotImplementedException();
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override void GetIdByDescription(IBusinessEntityGetIdByDescriptionContext context)
        {
            context.ThrowIfNull("context");

            var cachedGenericBEByTitleField = GetCachedGenericBEByTitleField(context.BusinessEntityDefinitionId);
            if (cachedGenericBEByTitleField == null)
            {
                context.ErrorMessage = $"No data exist for this entity.";
                return;
            }

            var genericBE = cachedGenericBEByTitleField.GetRecord(context.FieldDescription);
            if (genericBE != null)
            {
                var idDataRecordField = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(context.BusinessEntityDefinitionId);
                context.FieldValue = genericBE.FieldValues.GetRecord(idDataRecordField.Name);
            }
            else
            {
                var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(context.BusinessEntityDefinitionId);
                genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", context.BusinessEntityDefinitionId);
                context.ErrorMessage = $"This { genericBEDefinitionSetting.TitleFieldName} does not exist.";
            }
        }

        #endregion

        #region security
        public bool DoesUserHaveActionAccess(string actionKind, Guid BusinessEntityDefinitionId, Guid businessEntityActionTypeId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveActionAccess(actionKind, userId, BusinessEntityDefinitionId, businessEntityActionTypeId);
        }
        public bool DoesUserHaveActionAccess(string actionKind, int userId, Guid BusinessEntityDefinitionId, Guid ActionTypeId)
        {

            var genericBEaction = this.GetGenericBEAction(BusinessEntityDefinitionId, ActionTypeId);
            genericBEaction.ThrowIfNull("BusinrAction ", ActionTypeId);
            genericBEaction.Settings.ThrowIfNull("Business Entity Action Settings", ActionTypeId);

            var context = new GenericBEActionDefinitionCheckAccessContext
            {
                UserId = userId,
                BusinessEntityDefinitionId = BusinessEntityDefinitionId,
                GenericBEAction = genericBEaction
            };

            if (actionKind != genericBEaction.Settings.ActionKind || !genericBEaction.Settings.DoesUserHaveAccess(context))
                return false;
            return true;
        }



        public bool DoesUserHaveViewAccess(int userId, List<Guid> genericBeDefinitionIds)
        {
            foreach (var id in genericBeDefinitionIds)
            {
                if (DoesUserHaveViewAccess(userId, id))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveViewAccess(Guid genericBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, genericBeDefinitionId, (sec) => sec.ViewRequiredPermission);
        }
        public bool DoesUserHaveViewAccess(int userId, Guid genericBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, genericBeDefinitionId, (sec) => sec.ViewRequiredPermission);
        }
        public bool DoesUserHaveAddAccess(Guid genericBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, genericBeDefinitionId, (sec) => sec.AddRequiredPermission);

        }
        public bool DoesUserHaveDeleteAccess(Guid genericBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveAccess(userId, genericBeDefinitionId, (sec) => sec.DeleteRequiredPermission);
        }
        public bool DoesUserHaveEditAccess(Guid genericBeDefinitionId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveEditAccess(userId, genericBeDefinitionId);
        }
        public bool DoesUserHaveEditAccess(int userId, Guid genericBeDefinitionId)
        {
            return DoesUserHaveAccess(userId, genericBeDefinitionId, (sec) => sec.EditRequiredPermission);
        }
        private bool DoesUserHaveAccess(int userId, Guid businessEntityDefinitionId, Func<GenericBEDefinitionSecurity, Vanrise.Security.Entities.RequiredPermissionSettings> getRequiredPermissionSetting)
        {
            var accountBEDefinitionSettings = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            if (accountBEDefinitionSettings != null && accountBEDefinitionSettings.Security != null && getRequiredPermissionSetting(accountBEDefinitionSettings.Security) != null)
                return s_securityManager.IsAllowed(getRequiredPermissionSetting(accountBEDefinitionSettings.Security), userId);
            else
                return true;
        }

        #endregion

    }
    public class GenericBusinessEntityRuntimeEditor
    {
        public GenericBEDefinitionSettings GenericBEDefinitionSettings { get; set; }
        public GenericBusinessEntity GenericBusinessEntity { get; set; }
        public string DefinitionTitle { get; set; }
        public string TitleFieldName { get; set; }
    }
}
