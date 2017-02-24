using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Data;
using Vanrise.Invoice.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
using Vanrise.Caching;
using Vanrise.GenericData.Business;
using Vanrise.Security.Business;
using Vanrise.Security.Entities;
using Vanrise.GenericData.MainExtensions.DataRecordFields;
namespace Vanrise.Invoice.Business
{
    public class InvoiceTypeManager : IInvoiceTypeManager
    {

        #region Public Methods
        public InvoiceType GetInvoiceType(Guid invoiceTypeId)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            return invoiceTypes.GetRecord(invoiceTypeId);
        }
        public InvoiceTypeRuntime GetInvoiceTypeRuntime(Guid invoiceTypeId)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType =  invoiceTypes.GetRecord(invoiceTypeId);
            InvoiceTypeRuntime invoiceTypeRuntime = new InvoiceTypeRuntime();
            invoiceTypeRuntime.InvoiceType = invoiceType;
            invoiceTypeRuntime.MainGridRuntimeColumns = new List<InvoiceUIGridColumnRunTime>();

            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var recordType = dataRecordTypeManager.GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            if (recordType == null)
                throw new NullReferenceException(String.Format("Record Type {0} Not Found.", invoiceType.Settings.InvoiceDetailsRecordTypeId));
            foreach(var gridColumn in invoiceType.Settings.InvoiceGridSettings.MainGridColumns)
            {
                GridColumnAttribute attribute = null;
                if(gridColumn.CustomFieldName != null)
                {
                      var fieldType = recordType.Fields.FirstOrDefault(x=>x.Name == gridColumn.CustomFieldName);
                      if (fieldType != null)
                          attribute = fieldType.Type.GetGridColumnAttribute(null);
                }

                invoiceTypeRuntime.MainGridRuntimeColumns.Add(new InvoiceUIGridColumnRunTime
                {
                    CustomFieldName = gridColumn.CustomFieldName,
                    Attribute = attribute,
                    Field = gridColumn.Field,
                    Header = gridColumn.Header
                });
            }
            invoiceTypeRuntime.InvoicePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
     
            return invoiceTypeRuntime;
        }
        public List<InvoiceUIGridColumnRunTime> GetInvoiceTypeGridColumns(Guid invoiceTypeId)
        {
            var invoiceType =  GetInvoiceType(invoiceTypeId);
            List<InvoiceUIGridColumnRunTime> gridColumns = new List<InvoiceUIGridColumnRunTime>();
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var recordType = dataRecordTypeManager.GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            if (recordType == null)
                throw new NullReferenceException(String.Format("Record Type {0} Not Found.", invoiceType.Settings.InvoiceDetailsRecordTypeId));
            foreach (var gridColumn in invoiceType.Settings.InvoiceGridSettings.MainGridColumns)
            {
                GridColumnAttribute attribute = null;

                switch(gridColumn.Field)
                {
                    case InvoiceField.CustomField: 
                        if (gridColumn.CustomFieldName != null)
                        {
                            var fieldType = recordType.Fields.FirstOrDefault(x => x.Name == gridColumn.CustomFieldName);
                            if (fieldType != null)
                                attribute = fieldType.Type.GetGridColumnAttribute(null);
                        }
                        break;
                    case InvoiceField.CreatedTime: 
                        attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.DateTime }.GetGridColumnAttribute(null);
                         break;
                    case InvoiceField.DueDate: 
                         attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.DateTime }.GetGridColumnAttribute(null);
                         break;
                    case InvoiceField.FromDate: 
                        attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.DateTime }.GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.InvoiceId:
                        attribute = new FieldNumberType() { DataType = FieldNumberDataType.BigInt }.GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.IssueDate:                      
                        attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.DateTime }.GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.Lock:
                        attribute = new FieldBooleanType().GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.Note:
                        attribute = new FieldTextType().GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.Paid: 
                        attribute = new FieldBooleanType().GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.Partner:
                        attribute = new FieldTextType().GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.SerialNumber:
                        attribute = new FieldTextType().GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.TimeZone:
                        attribute = new FieldTextType().GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.TimeZoneOffset:
                        attribute = new FieldTextType().GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.ToDate:
                        attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.DateTime }.GetGridColumnAttribute(null);
                        break;
                    case InvoiceField.UserId:
                        attribute = new FieldTextType().GetGridColumnAttribute(null);
                        break;
                }
              
                gridColumns.Add(new InvoiceUIGridColumnRunTime
                {
                    CustomFieldName = gridColumn.CustomFieldName,
                    Attribute = attribute,
                    Field = gridColumn.Field,
                    Header = gridColumn.Header
                });
            }
            return gridColumns;
        }
        public GeneratorInvoiceTypeRuntime GetGeneratorInvoiceTypeRuntime(Guid invoiceTypeId)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType = invoiceTypes.GetRecord(invoiceTypeId);
            GeneratorInvoiceTypeRuntime generatorInvoiceTypeRuntime = new Entities.GeneratorInvoiceTypeRuntime();
            generatorInvoiceTypeRuntime.InvoiceType = invoiceType;
            generatorInvoiceTypeRuntime.InvoicePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
            return generatorInvoiceTypeRuntime;
        }
        public List<InvoiceGeneratorAction> GetInvoiceGeneratorActions(GenerateInvoiceInput generateInvoiceInput)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType = invoiceTypes.GetRecord(generateInvoiceInput.InvoiceTypeId);
            PartnerInvoiceFilterConditionContext context = new PartnerInvoiceFilterConditionContext
            {
                InvoiceType = invoiceType,
                generateInvoiceInput = generateInvoiceInput
            };

            List<InvoiceGeneratorAction> actions = new List<InvoiceGeneratorAction>();
            foreach(var action in invoiceType.Settings.InvoiceGeneratorActions)
            {
                if (action.FilterCondition == null || action.FilterCondition.IsFilterMatch(context))
                {
                    actions.Add(action);
                }
            }
            return actions;
        }
        public IDataRetrievalResult<InvoiceTypeDetail> GetFilteredInvoiceTypes(DataRetrievalInput<InvoiceTypeQuery> input)
        {
            var allItems = GetCachedInvoiceTypes();

            Func<InvoiceType, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, InvoiceTypeDetailMapper));
        }
        public Vanrise.Entities.InsertOperationOutput<InvoiceTypeDetail> AddInvoiceType(InvoiceType invoiceType)
        {
            var insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<InvoiceTypeDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;

            IInvoiceTypeDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceTypeDataManager>();
            invoiceType.InvoiceTypeId = Guid.NewGuid();
            if (dataManager.InsertInvoiceType(invoiceType))
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = InvoiceTypeDetailMapper(invoiceType);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<InvoiceTypeDetail> UpdateInvoiceType(InvoiceType invoiceType)
        {
            var updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<InvoiceTypeDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IInvoiceTypeDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceTypeDataManager>();

            if (dataManager.UpdateInvoiceType(invoiceType))
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = InvoiceTypeDetailMapper(invoiceType);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public IEnumerable<GridColumnAttribute> ConvertToGridColumnAttribute(ConvertToGridColumnAttributeInput input)
        {
            List<GridColumnAttribute> gridColumnAttributes = null;
            if(input.GridColumns != null)
            {
                gridColumnAttributes = new List<GridColumnAttribute>();
                foreach(var column in input.GridColumns)
                {
                    if (column.FieldType == null)
                        throw new NullReferenceException(string.Format("{0} is not mapped to field type.", column.FieldName));
                    var gridAttribute = column.FieldType.GetGridColumnAttribute(null);
                    gridAttribute.HeaderText = column.Header;
                    gridAttribute.Field = column.FieldName;
                    gridAttribute.Tag = column.FieldName;
                    gridColumnAttributes.Add(gridAttribute);
                }
            }
            return gridColumnAttributes;

        }
        public IEnumerable<InvoiceTypeInfo> GetInvoiceTypesInfo(InvoiceTypeInfoFilter filter)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            Func<InvoiceType, bool> filterExpression = (x) =>
            {

                if (!DoesUserHaveViewSettingsAccess(SecurityContext.Current.GetLoggedInUserId(), x))
                    return false;

                return true;
            };
            if (filter != null)
            {
            }
            return invoiceTypes.FindAllRecords(filterExpression).MapRecords(InvoiceTypeInfoMapper);
        }
      

        public bool DoesUserHaveViewAccess(Guid invoiceTypeId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveViewAccess(userId, invoiceTypeId);
        }
        public bool DoesUserHaveViewAccess(int userId, Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            if (invoiceType != null &&   invoiceType.Settings !=null && invoiceType.Settings.Security != null && invoiceType.Settings.Security.ViewRequiredPermission != null)
                return DoesUserHaveAccess(userId, invoiceType.Settings.Security.ViewRequiredPermission);
            return true;
        }

        public bool DoesUserHaveGenerateAccess(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.Security != null && invoiceType.Settings.Security.GenerateRequiredPermission != null)
                return DoesUserHaveAccess(invoiceType.Settings.Security.GenerateRequiredPermission);
            return true;
        }
        public bool DoesUserHaveViewSettingsAccess(int userId)
        {
            var allItems = GetInvoiceTypes();
            foreach (var invoice in allItems)
            {
                if (DoesUserHaveViewSettingsAccess(userId, invoice))
                    return true;
            }
            return false;
        }
        public bool DoesUserHaveViewSettingsAccess(Guid invoiceTypeId)
        {
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveViewSettingsAccess(userId, invoiceType);
        }
        public bool DoesUserHaveViewSettingsAccess(int userId,InvoiceType invoiceType)
        {
            if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.Security != null && invoiceType.Settings.Security.ViewSettingsRequiredPermission != null)
                return DoesUserHaveAccess(invoiceType.Settings.Security.ViewSettingsRequiredPermission);
            return true;
        }

        public bool DoesUserHaveAddSettingsAccess(Guid invoiceTypeId)
        {
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
            if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.Security != null && invoiceType.Settings.Security.AddSettingsRequiredPermission != null)
                return DoesUserHaveAccess(invoiceType.Settings.Security.AddSettingsRequiredPermission);
            return true;
        }
        public bool DoesUserHaveEditSettingsAccess(Guid invoiceTypeId)
        {
            var invoiceType = new InvoiceTypeManager().GetInvoiceType(invoiceTypeId);
            if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.Security != null && invoiceType.Settings.Security.EditSettingsRequiredPermission != null)
                return DoesUserHaveAccess(invoiceType.Settings.Security.EditSettingsRequiredPermission);
            return true;
        }       
        public IEnumerable<InvoiceType> GetInvoiceTypes()
        {
            return GetCachedInvoiceTypes().Values;
        }

        public string GetInvoicePartnerSelector(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
             if(invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.ExtendedSettings != null)
             {
                 var invoicePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();
                 if (invoicePartnerManager != null)
                     return invoicePartnerManager.PartnerSelector;
             }
             return null;
        }
        public InvoiceAction GetInvoiceAction(Guid invoiceTypeId, Guid invoiceActionId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            var invoiceAction = invoiceType.Settings.InvoiceActions.FirstOrDefault(x => x.InvoiceActionId == invoiceActionId);
            if (invoiceAction == null)
                throw new NullReferenceException("invoiceAction");
            return invoiceAction;
        }

        public IEnumerable<InvoiceFieldInfo> GetRemoteInvoiceFieldsInfo()
        {
            List<InvoiceFieldInfo> invoiceFields = new List<InvoiceFieldInfo>();
            Type enumType = typeof(InvoiceField);
            var enumFields = enumType.GetEnumValues();
            foreach (var item in enumFields)
            {
                InvoiceField field = (InvoiceField)item;
                invoiceFields.Add(new InvoiceFieldInfo
                {
                    InvoiceFieldId = field,
                    Name = Utilities.GetEnumDescription<InvoiceField>(field)
                });
            }
            return invoiceFields;
        }
        public IEnumerable<string> GetRemoteInvoiceTypeCustomFieldsInfo(Guid invoiceTypeId)
        {
            var invoiceType = this.GetInvoiceType(invoiceTypeId);
            var dataRecordFields = new DataRecordTypeManager().GetDataRecordTypeFields(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            List<string> invoiceCustomFields = new List<string>();
            if (dataRecordFields == null)
                return null;
            return dataRecordFields.Select(x=>x.Key);
        }
        public IEnumerable<InvoiceAttachmentInfo> GetRemoteInvoiceTypeAttachmentsInfo(Guid invoiceTypeId)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType = invoiceTypes.GetRecord(invoiceTypeId);
            if(invoiceType == null)
                throw new NullReferenceException(string.Format("invoiceType:{0}",invoiceTypeId));
            List<InvoiceAttachmentInfo> invoiceAttachmentsInfo = new List<InvoiceAttachmentInfo>();
            if (invoiceType.Settings.InvoiceAttachments != null && invoiceType.Settings.InvoiceAttachments.Count > 0)
            {
                foreach (var attachment in invoiceType.Settings.InvoiceAttachments)
                {
                    invoiceAttachmentsInfo.Add(new InvoiceAttachmentInfo
                    {
                        InvoiceAttachmentId = attachment.InvoiceAttachmentId,
                        Title = attachment.Title
                    });
                }
            }
            return invoiceAttachmentsInfo;
        }
        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IInvoiceTypeDataManager _dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceTypeDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreInvoiceTypesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods
        private Dictionary<Guid, InvoiceType> GetCachedInvoiceTypes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetInvoiceTypes",
              () =>
              {
                  IInvoiceTypeDataManager dataManager = InvoiceDataManagerFactory.GetDataManager<IInvoiceTypeDataManager>();
                  IEnumerable<InvoiceType> invoiceTypes = dataManager.GetInvoiceTypes();
                  return invoiceTypes.ToDictionary(c => c.InvoiceTypeId, c => c);
              });
        }
        private bool DoesUserHaveAccess(int userId, RequiredPermissionSettings requiredPermission)
        {
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        private bool DoesUserHaveAccess(RequiredPermissionSettings requiredPermission)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            SecurityManager secManager = new SecurityManager();
            if (!secManager.IsAllowed(requiredPermission, userId))
                return false;
            return true;

        }
        #endregion
     
        #region Mappers

        private InvoiceTypeDetail InvoiceTypeDetailMapper(InvoiceType invoiceTypeObject)
        {
            InvoiceTypeDetail invoiceTypeDetail = new InvoiceTypeDetail();
            invoiceTypeDetail.Entity = invoiceTypeObject;
            return invoiceTypeDetail;
        }
        private InvoiceTypeInfo InvoiceTypeInfoMapper(InvoiceType invoiceTypeObject)
        {
            return new InvoiceTypeInfo
            {
                InvoiceTypeId = invoiceTypeObject.InvoiceTypeId,
                Name = invoiceTypeObject.Name
            };
        }
        #endregion

    } 
}
