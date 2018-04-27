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

namespace Vanrise.GenericData.Business
{
    public class GenericBusinessEntityManager : BaseBusinessEntityManager
    {
        #region Fields / Constructors
        GenericBusinessEntityDefinitionManager _genericBEDefinitionManager;
        DataRecordStorageManager _dataRecordStorageManager;
        SecurityManager s_securityManager;

        public GenericBusinessEntityManager()
        {
            _genericBEDefinitionManager = new GenericBusinessEntityDefinitionManager();
            _dataRecordStorageManager = new DataRecordStorageManager();
            s_securityManager = new SecurityManager();
        }
        #endregion


        #region Public Methods
        public IEnumerable<GenericBusinessEntityInfo> GetGenericBusinessEntityInfo(Guid businessEntityDefinitionId, GenericBusinessEntityInfoFilter filter)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            var dataRecordStorage = _dataRecordStorageManager.GetDataRecordStorage(genericBEDefinitionSetting.DataRecordStorageId);
            if (!dataRecordStorage.Settings.EnableUseCaching)
                throw new Exception("Caching not enabled.");
            List<string> columns = new List<string>();
            List<string> columnTitles = new List<string>();
            var dataRecordFields = _genericBEDefinitionManager.GetDataRecordTypeFieldsByBEDefinitionId(businessEntityDefinitionId);
            foreach (var field in dataRecordFields)
            {
                columns.Add(field.Key);
                columnTitles.Add(field.Value.Title);
            }
            var idDataRecordField = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(businessEntityDefinitionId);
            var storageRecords = _dataRecordStorageManager.GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
            {

                FromRow = 0,
                ToRow = 1000,
                SortByColumnName = string.Format("FieldValues.{0}.Description", idDataRecordField.Name),
                Query = new DataRecordQuery
                {
                    Columns = columns,
                    ColumnTitles = columnTitles,
                    DataRecordStorageIds = new List<Guid> { genericBEDefinitionSetting.DataRecordStorageId },
                    LimitResult = 1000,
                },
            }) as BigResult<DataRecordDetail>;

            List<GenericBusinessEntityDetail> result = new List<GenericBusinessEntityDetail>();
            if (storageRecords != null && storageRecords.Data != null)
            {
                foreach (var storageRecord in storageRecords.Data)
                {
                    GenericBusinessEntityDetail genericBusinessEntityDetail = new GenericBusinessEntityDetail();
                    if (storageRecord.FieldValues != null)
                    {
                        genericBusinessEntityDetail.FieldValues = new GenericBusinessEntityValues();
                        foreach (var fieldValue in storageRecord.FieldValues)
                        {
                            genericBusinessEntityDetail.FieldValues.Add(fieldValue.Key, new GenericBusinessEntityValue
                            {
                                Value = fieldValue.Value.Value,
                                Description = fieldValue.Value.Description,
                            });
                        }
                    }
                    if (genericBEDefinitionSetting.GridDefinition.GenericBEGridActions != null)
                    {
                        genericBusinessEntityDetail.AvailableGridActionIds = new List<Guid>();

                        foreach (var genericBEGridAction in genericBEDefinitionSetting.GridDefinition.GenericBEGridActions)
                        {
                            genericBusinessEntityDetail.AvailableGridActionIds.Add(genericBEGridAction.GenericBEGridActionId);
                        }
                    }
                    if (genericBEDefinitionSetting.GridDefinition.GenericBEGridViews != null)
                    {
                        genericBusinessEntityDetail.AvailableGridViewIds = new List<Guid>();
                        foreach (var genericBEGridView in genericBEDefinitionSetting.GridDefinition.GenericBEGridViews)
                        {
                            genericBusinessEntityDetail.AvailableGridViewIds.Add(genericBEGridView.GenericBEViewDefinitionId);
                        }
                    }
                    result.Add(genericBusinessEntityDetail);
                }
            }
            //var cachedGenericBusinessEntities = GetCachedGenericBusinessEntities(businessEntityDefinitionId);
            //if (filter != null)
            //{

            //}
            return result.MapRecords((record) =>
            {
                return GenericBusinessEntityInfoMapper(businessEntityDefinitionId, record);
            });
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
                    DataRecordStorageIds = new List<Guid> { genericBEDefinitionSetting.DataRecordStorageId },
                    //Direction=,
                    LimitResult = 1000,
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
        public string GetGenericBusinessEntityName(Object genericBusinessEntityId, Guid businessEntityDefinitionId)
        {
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(businessEntityDefinitionId);
            var genericBusinessEntity = GetGenericBusinessEntity(genericBusinessEntityId, businessEntityDefinitionId);
            genericBEDefinitionSetting.TitleFieldName.ThrowIfNull("genericBEDefinitionSetting.TitleFieldName");
            if (genericBusinessEntity != null && genericBusinessEntity.FieldValues != null)
            {
                var fieldValue = genericBusinessEntity.FieldValues[genericBEDefinitionSetting.TitleFieldName];
                if (fieldValue != null)
                    return fieldValue.ToString();
            }
            return null;
        }

        public IDataRetrievalResult<GenericBusinessEntityDetail> GetFilteredGenericBusinessEntities(DataRetrievalInput<GenericBusinessEntityQuery> input)
        {

            Func<GenericBusinessEntity, bool> filterExpression = (genericBusinessEntity) =>
            {

                return true;
            };
            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(input.Query.BusinessEntityDefinitionId);
            genericBEDefinitionSetting.ThrowIfNull("genericBEDefinitionSetting", input.Query.BusinessEntityDefinitionId);
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

            var storageRecords = _dataRecordStorageManager.GetFilteredDataRecords(new DataRetrievalInput<DataRecordQuery>
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
                    DataRecordStorageIds = new List<Guid> { genericBEDefinitionSetting.DataRecordStorageId },
                    Direction = OrderDirection.Descending,
                    LimitResult = 1000,
                    //SortColumns=,
                    //ToTime
                },
            }) as BigResult<DataRecordDetail>;

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
                        GenericBusinessEntityDetail genericBusinessEntityDetail = new GenericBusinessEntityDetail();
                        if (storageRecord.FieldValues != null)
                        {
                            genericBusinessEntityDetail.FieldValues = new GenericBusinessEntityValues();
                            foreach (var fieldValue in storageRecord.FieldValues)
                            {
                                genericBusinessEntityDetail.FieldValues.Add(fieldValue.Key, new GenericBusinessEntityValue
                                {
                                    Value = fieldValue.Value.Value,
                                    Description = fieldValue.Value.Description,
                                });
                            }
                        }
                        if (genericBEDefinitionSetting.GridDefinition.GenericBEGridActions != null)
                        {
                            genericBusinessEntityDetail.AvailableGridActionIds = new List<Guid>();



                            foreach (var genericBEGridAction in genericBEDefinitionSetting.GridDefinition.GenericBEGridActions)
                            {

                                var genericBeAction = genericBeActions.GetRecord(genericBEGridAction.GenericBEActionId);
                                genericBeAction.ThrowIfNull("Generic Be Action ", genericBEGridAction.GenericBEActionId);

                                var settings = genericBeAction.Settings;
                                settings.ThrowIfNull("Generic Be Action Settings", genericBEGridAction.GenericBEActionId);


                                if (settings.DoesUserHaveAccess(actionContext))
                                    genericBusinessEntityDetail.AvailableGridActionIds.Add(genericBEGridAction.GenericBEGridActionId);
                            }
                        }
                        if (genericBEDefinitionSetting.GridDefinition.GenericBEGridViews != null)
                        {
                            genericBusinessEntityDetail.AvailableGridViewIds = new List<Guid>();
                            foreach (var genericBEGridView in genericBEDefinitionSetting.GridDefinition.GenericBEGridViews)
                            {
                                var viewSettings = genericBEGridView.Settings;
                                viewSettings.ThrowIfNull("Generic BE View Settings", genericBEGridView.GenericBEViewDefinitionId);
                                if (viewSettings.DoesUserHaveAccess(viewContext))
                                    genericBusinessEntityDetail.AvailableGridViewIds.Add(genericBEGridView.GenericBEViewDefinitionId);
                            }
                        }

                        resultDetail.Add(genericBusinessEntityDetail);
                    }
                }

                result.Data = resultDetail;
            }
            VRActionLogger.Current.LogGetFilteredAction(new GenericBusinessEntityLoggableEntity(input.Query.BusinessEntityDefinitionId), input);
            //   var vrCases =  storageDataManager.GetFilteredDataRecords()
            //  return DataRetrievalManager.Instance.ProcessResult(input, vrCases.ToBigResult(input, filterExpression, VRCaseDetailMapper));
            return result;
        }
        public InsertOperationOutput<GenericBusinessEntityDetail> AddGenericBusinessEntity(GenericBusinessEntityToAdd genericBusinessEntityToAdd)
        {
            InsertOperationOutput<GenericBusinessEntityDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<GenericBusinessEntityDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(genericBusinessEntityToAdd.BusinessEntityDefinitionId);
            genericBusinessEntityToAdd.FieldValues = new DataRecordTypeManager().ParseDicValuesToFieldType(genericBEDefinitionSetting.DataRecordTypeId, genericBusinessEntityToAdd.FieldValues);

            var idFieldType = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(genericBusinessEntityToAdd.BusinessEntityDefinitionId);

            OnBeforeSaveHandler(genericBEDefinitionSetting, genericBusinessEntityToAdd.BusinessEntityDefinitionId, null, genericBusinessEntityToAdd);
            var fieldTypes = _genericBEDefinitionManager.GetDataRecordTypeFields(genericBEDefinitionSetting.DataRecordTypeId);
            OnBeforeSaveMethod(fieldTypes, genericBusinessEntityToAdd.BusinessEntityDefinitionId, genericBusinessEntityToAdd, null);



            Object insertedId;
            bool hasInsertedId;
            bool insertActionSucc = _dataRecordStorageManager.AddDataRecord(genericBEDefinitionSetting.DataRecordStorageId, genericBusinessEntityToAdd.FieldValues, out insertedId, out hasInsertedId);

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

                OnAfterSaveHandler(genericBEDefinitionSetting, genericBusinessEntityToAdd.BusinessEntityDefinitionId, null, genericBusinessEntity);
                OnAfterSaveMethod(fieldTypes, genericBusinessEntityToAdd.BusinessEntityDefinitionId, genericBusinessEntity, genericBusinessEntityId);

                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = GenericBusinessEntityDetailMapper(genericBusinessEntityToAdd.BusinessEntityDefinitionId, genericBusinessEntity);
            }
            else
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;

            return insertOperationOutput;
        }
        public UpdateOperationOutput<GenericBusinessEntityDetail> UpdateGenericBusinessEntity(GenericBusinessEntityToUpdate genericBusinessEntityToUpdate)
        {
            UpdateOperationOutput<GenericBusinessEntityDetail> updateOperationOutput = new UpdateOperationOutput<GenericBusinessEntityDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var idFieldType = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(genericBusinessEntityToUpdate.BusinessEntityDefinitionId);
            genericBusinessEntityToUpdate.GenericBusinessEntityId = idFieldType.Type.ParseValueToFieldType(new DataRecordFieldTypeParseValueToFieldTypeContext(genericBusinessEntityToUpdate.GenericBusinessEntityId));

            var genericBEDefinitionSetting = _genericBEDefinitionManager.GetGenericBEDefinitionSettings(genericBusinessEntityToUpdate.BusinessEntityDefinitionId);
            genericBusinessEntityToUpdate.FieldValues = new DataRecordTypeManager().ParseDicValuesToFieldType(genericBEDefinitionSetting.DataRecordTypeId, genericBusinessEntityToUpdate.FieldValues);


            var fieldTypes = _genericBEDefinitionManager.GetDataRecordTypeFields(genericBEDefinitionSetting.DataRecordTypeId);
            OnBeforeSaveMethod(fieldTypes, genericBusinessEntityToUpdate.BusinessEntityDefinitionId, genericBusinessEntityToUpdate, genericBusinessEntityToUpdate.GenericBusinessEntityId);


            var oldGenericBE = GetGenericBusinessEntity(genericBusinessEntityToUpdate.GenericBusinessEntityId, genericBusinessEntityToUpdate.BusinessEntityDefinitionId);

            bool updateActionSucc = _dataRecordStorageManager.UpdateDataRecord(genericBEDefinitionSetting.DataRecordStorageId, genericBusinessEntityToUpdate.GenericBusinessEntityId, genericBusinessEntityToUpdate.FieldValues);

            if (updateActionSucc)
            {

                var genericBusinessEntity = GetGenericBusinessEntity(genericBusinessEntityToUpdate.GenericBusinessEntityId, genericBusinessEntityToUpdate.BusinessEntityDefinitionId);

                UpdateStatusHistoryIfAvailable(genericBusinessEntityToUpdate.BusinessEntityDefinitionId, genericBusinessEntity, genericBusinessEntityToUpdate.GenericBusinessEntityId);


                VRActionLogger.Current.TrackAndLogObjectUpdated(new GenericBusinessEntityLoggableEntity(genericBusinessEntityToUpdate.BusinessEntityDefinitionId), genericBusinessEntityToUpdate, oldGenericBE);

                OnAfterSaveHandler(genericBEDefinitionSetting, genericBusinessEntityToUpdate.BusinessEntityDefinitionId, oldGenericBE, genericBusinessEntity);
                OnAfterSaveMethod(fieldTypes, genericBusinessEntityToUpdate.BusinessEntityDefinitionId, genericBusinessEntity, genericBusinessEntityToUpdate.GenericBusinessEntityId);


                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = GenericBusinessEntityDetailMapper(genericBusinessEntityToUpdate.BusinessEntityDefinitionId, genericBusinessEntity);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public void LogObjectCustomAction(Guid businessEntityDefinitionId, GenericBusinessEntity genericBusinessEntity, string actionName, bool isObjectUpdated, string actionDescription)
        {
            VRActionLogger.Current.LogObjectCustomAction(new GenericBusinessEntityLoggableEntity(businessEntityDefinitionId), actionName, isObjectUpdated, genericBusinessEntity, actionDescription);
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
                        if (!businessEntityStatusHistoryManager.InsertStatusHistory(businessEntityDefinitionId, genericBusinessEntityId.ToString(), statusFieldName,Guid.Parse(statusValue.ToString())))
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
        private void OnBeforeSaveHandler(GenericBEDefinitionSettings genericBEDefinitionSetting, Guid businessEntityDefinitionId, GenericBusinessEntity oldEntity, GenericBusinessEntity newEntity)
        {
            if (genericBEDefinitionSetting.OnBeforeInsertHandler != null)
            {
                genericBEDefinitionSetting.OnBeforeInsertHandler.Execute(new GenericBEOnBeforeInsertHandlerContext
                {
                    DefinitionSettings = genericBEDefinitionSetting,
                    GenericBusinessEntity = newEntity,
                    BusinessEntityDefinitionId = businessEntityDefinitionId,
                });
            }
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
        private void OnAfterSaveHandler(GenericBEDefinitionSettings genericBEDefinitionSetting, Guid businessEntityDefinitionId, GenericBusinessEntity oldEntity, GenericBusinessEntity newEntity)
        {
            if (genericBEDefinitionSetting.OnAfterSaveHandler != null)
            {
                genericBEDefinitionSetting.OnAfterSaveHandler.Execute(new GenericBEOnAfterSaveHandlerContext
                {
                    DefinitionSettings = genericBEDefinitionSetting,
                    NewEntity = newEntity,
                    BusinessEntityDefinitionId = businessEntityDefinitionId,
                    OldEntity = oldEntity
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

            var gridDefinition = _genericBEDefinitionManager.GetGenericBEDefinitionGridDefinition(businessEntityDefinitionId);
            if (gridDefinition != null)
            {
                if (gridDefinition.GenericBEGridActions != null)
                {
                    genericBusinessEntityDetail.AvailableGridActionIds = new List<Guid>();
                    foreach (var genericBEGridAction in gridDefinition.GenericBEGridActions)
                    {
                        genericBusinessEntityDetail.AvailableGridActionIds.Add(genericBEGridAction.GenericBEGridActionId);
                    }
                }
                if (gridDefinition.GenericBEGridViews != null)
                {
                    genericBusinessEntityDetail.AvailableGridViewIds = new List<Guid>();
                    foreach (var genericBEGridView in gridDefinition.GenericBEGridViews)
                    {
                        genericBusinessEntityDetail.AvailableGridViewIds.Add(genericBEGridView.GenericBEViewDefinitionId);
                    }
                }
            }

            return genericBusinessEntityDetail;
        }

        private GenericBusinessEntityInfo GenericBusinessEntityInfoMapper(Guid businessEntityDefinitionId, GenericBusinessEntityDetail genericBusinessEntityDetail)
        {
            var idDataRecordField = _genericBEDefinitionManager.GetIdFieldTypeForGenericBE(businessEntityDefinitionId);
            var titleFieldName = _genericBEDefinitionManager.GetGenericBEDefinitionTitleFieldName(businessEntityDefinitionId);

            GenericBusinessEntityInfo entityInfo = new GenericBusinessEntityInfo();
            entityInfo.GenericBusinessEntityId = genericBusinessEntityDetail.FieldValues[idDataRecordField.Name].Value;
            var titleValue = genericBusinessEntityDetail.FieldValues[titleFieldName].Value;
            entityInfo.Name = titleValue != null ? titleValue.ToString() : null;
            return entityInfo;
        }
        #endregion


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

        #region security

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
