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

        public Dictionary<Guid, InvoiceAction> GetInvoiceActionsByActionId(Guid invoiceTypeId)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(string.Format("InvoiceTypeManager_GetInvoiceActionsByActionId_{0}", invoiceTypeId),
              () =>
              {
                  var invoiceType = GetInvoiceType(invoiceTypeId);
                  invoiceType.ThrowIfNull("Invoice Type", invoiceTypeId);
                  invoiceType.Settings.ThrowIfNull("Invoice Type Settings", invoiceTypeId);
                  invoiceType.Settings.InvoiceActions.ThrowIfNull("Invoice Type Settings Actions", invoiceTypeId);
                  return invoiceType.Settings.InvoiceActions.ToDictionary(c => c.InvoiceActionId, c => c);
              });
        }
        public InvoiceTypeExtendedSettings GetInvoiceTypeExtendedSettings(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
            return invoiceType.Settings.ExtendedSettings;
        }
        public string GetInvoiceTypeName(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            if (invoiceType != null)
                return invoiceType.Name;
            return null;
        }
        public Guid GetInvoiceTypeCommentDefinitionId(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
                return invoiceType.Settings.InvoiceCommentDefinitionId.Value;
           
        }
        public Guid? GetInvToAccBalanceRelationId(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
            return invoiceType.Settings.InvToAccBalanceRelationId;
        }
        public InvoiceTypeRuntime GetInvoiceTypeRuntime(Guid invoiceTypeId)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType = invoiceTypes.GetRecord(invoiceTypeId);
            InvoiceTypeRuntime invoiceTypeRuntime = new InvoiceTypeRuntime();
            invoiceTypeRuntime.InvoiceType = invoiceType;
            invoiceTypeRuntime.MainGridRuntimeColumns = new List<InvoiceUIGridColumnRunTime>();

            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var recordType = dataRecordTypeManager.GetDataRecordType(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            if (recordType == null)
                throw new NullReferenceException(String.Format("Record Type {0} Not Found.", invoiceType.Settings.InvoiceDetailsRecordTypeId));
            foreach (var gridColumn in invoiceType.Settings.InvoiceGridSettings.MainGridColumns)
            {
                GridColumnAttribute attribute = null;
                if (gridColumn.CustomFieldName != null)
                {
                    var fieldType = recordType.Fields.FirstOrDefault(x => x.Name == gridColumn.CustomFieldName);
                    if (fieldType != null)
                    {
                        attribute = fieldType.Type.GetGridColumnAttribute(null);
                    }

                }
                int? widthFactor = null;
                int? fixedWidth = null;
                if (gridColumn.GridColumnSettings != null)
                {
                    widthFactor = GridColumnWidthFactorConstants.GetColumnWidthFactor(gridColumn.GridColumnSettings);
                    if (!widthFactor.HasValue)
                        fixedWidth = gridColumn.GridColumnSettings.FixedWidth;
                }
                invoiceTypeRuntime.MainGridRuntimeColumns.Add(new InvoiceUIGridColumnRunTime
                {
                    CustomFieldName = gridColumn.CustomFieldName,
                    Attribute = attribute,
                    Field = gridColumn.Field,
                    Header = gridColumn.Header,
                    WidthFactor = widthFactor,
                    FixedWidth = fixedWidth,
                    UseDescription = gridColumn.UseDescription
                });
            }
            invoiceTypeRuntime.InvoicePartnerManager = invoiceType.Settings.ExtendedSettings.GetPartnerManager();

            return invoiceTypeRuntime;
        }
        public List<InvoiceUIGridColumnRunTime> GetInvoiceTypeGridColumns(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            List<InvoiceUIGridColumnRunTime> gridColumns = new List<InvoiceUIGridColumnRunTime>();
            DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            var dataRecordTypeFields = dataRecordTypeManager.GetDataRecordTypeFields(invoiceType.Settings.InvoiceDetailsRecordTypeId);
            dataRecordTypeFields.ThrowIfNull("dataRecordTypeFields");

            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.DateTime }.GetGridColumnAttribute(null),
                Field = InvoiceField.CustomField,
                Header = "Created Time"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.Date }.GetGridColumnAttribute(null),
                Field = InvoiceField.DueDate,
                Header = "Due Date"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.Date }.GetGridColumnAttribute(null),
                Field = InvoiceField.FromDate,
                Header = "From Date"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.Date }.GetGridColumnAttribute(null),
                Field = InvoiceField.IssueDate,
                Header = "Issue Date"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldNumberType() { DataType = FieldNumberDataType.BigInt }.GetGridColumnAttribute(null),
                Field = InvoiceField.InvoiceId,
                Header = "Invoice Id"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldBooleanType().GetGridColumnAttribute(null),
                Field = InvoiceField.Lock,
                Header = "Lock"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldTextType().GetGridColumnAttribute(null),
                Field = InvoiceField.Note,
                Header = "Note"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldTextType().GetGridColumnAttribute(null),
                Field = InvoiceField.Paid,
                Header = "Paid"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldTextType().GetGridColumnAttribute(null),
                Field = InvoiceField.Partner,
                Header = "Partner"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldTextType().GetGridColumnAttribute(null),
                Field = InvoiceField.SerialNumber,
                Header = "Serial Number"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldDateTimeType() { DataType = FieldDateTimeDataType.Date }.GetGridColumnAttribute(null),
                Field = InvoiceField.ToDate,
                Header = "To Date"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldTextType().GetGridColumnAttribute(null),
                Field = InvoiceField.UserId,
                Header = "User Id"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldBooleanType().GetGridColumnAttribute(null),
                Field = InvoiceField.IsAutomatic,
                Header = "Is Automatic"
            });
            gridColumns.Add(new InvoiceUIGridColumnRunTime
            {
                Attribute = new FieldBooleanType().GetGridColumnAttribute(null),
                Field = InvoiceField.IsSent,
                Header = "Is Sent"
            });
            foreach (var dataRecordTypeField in dataRecordTypeFields)
            {
                GridColumnAttribute attribute = dataRecordTypeField.Value.Type.GetGridColumnAttribute(null);
                gridColumns.Add(new InvoiceUIGridColumnRunTime
                {
                    CustomFieldName = dataRecordTypeField.Value.Name,
                    Attribute = attribute,
                    Field = InvoiceField.CustomField,
                    Header = dataRecordTypeField.Value.Title
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
            foreach (var action in invoiceType.Settings.InvoiceGeneratorActions)
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
            VRActionLogger.Current.LogGetFilteredAction(InvoiceTypeLoggableEntity.Instance, input);
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
                VRActionLogger.Current.TrackAndLogObjectAdded(InvoiceTypeLoggableEntity.Instance, invoiceType);
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
                VRActionLogger.Current.TrackAndLogObjectUpdated(InvoiceTypeLoggableEntity.Instance, invoiceType);
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
            if (input.GridColumns != null)
            {
                gridColumnAttributes = new List<GridColumnAttribute>();
                foreach (var column in input.GridColumns)
                {
                    if (column.FieldType == null)
                        throw new NullReferenceException(string.Format("{0} is not mapped to field type.", column.FieldName));
                    var gridAttribute = column.FieldType.GetGridColumnAttribute(null);
                    gridAttribute.HeaderText = column.Header;
                    gridAttribute.Field = column.FieldName;
                    gridAttribute.Tag = column.FieldName;

                    if (column.GridColumnSettings != null)
                    {
                        gridAttribute.WidthFactor = GridColumnWidthFactorConstants.GetColumnWidthFactor(column.GridColumnSettings);
                        if (!gridAttribute.WidthFactor.HasValue)
                            gridAttribute.FixedWidth = column.GridColumnSettings.FixedWidth;
                    }
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

        public IEnumerable<InvoiceSettingPartDefinitionInfo> GetInvoiceSettingPartsInfo(InvoiceSettingPartsInfoFilter filter)
        {
            filter.ThrowIfNull("InvoiceSettingPartsInfoFilter");
            var invoiceType = GetInvoiceType(filter.InvoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", filter.InvoiceTypeId);
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
            List<InvoiceSettingPartDefinitionInfo> invoiceSettingPartDefinitionInfo = new List<InvoiceSettingPartDefinitionInfo>();
            ExtensionConfigurationManager extensionConfigurationManager = new ExtensionConfigurationManager();
            if (invoiceType.Settings.InvoiceSettingPartUISections != null)
            {
                foreach (var invoiceSettingPartUISection in invoiceType.Settings.InvoiceSettingPartUISections)
                {
                    if (invoiceSettingPartUISection.Rows != null)
                    {
                        foreach (var row in invoiceSettingPartUISection.Rows)
                        {
                            if (row.Parts != null)
                            {
                                foreach (var part in row.Parts)
                                {
                                    if (!filter.OnlyIsOverridable || part.IsOverridable == true)
                                    {
                                        invoiceSettingPartDefinitionInfo.Add(new InvoiceSettingPartDefinitionInfo
                                        {
                                            PartConfigId = part.PartConfigId,
                                            Name = extensionConfigurationManager.GetExtensionConfigurationTitle<InvoiceSettingPartConfig>(part.PartConfigId, InvoiceSettingPartConfig.EXTENSION_TYPE)
                                        });
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return invoiceSettingPartDefinitionInfo;
        }
        public bool DoesUserHaveViewAccess(Guid invoiceTypeId)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DoesUserHaveViewAccess(userId, invoiceTypeId);
        }

        public bool DoesUserHaveViewAccess(int userId, Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.Security != null && invoiceType.Settings.Security.ViewRequiredPermission != null)
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
        public bool DoesUserHaveViewSettingsAccess(int userId, InvoiceType invoiceType)
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

        public bool DosesUserHaveActionAccess(InvoiceActionType type, Guid invoiceTypeId, Guid ActionTypeId)
        {

            int userId = SecurityContext.Current.GetLoggedInUserId();
            return DosesUserHaveActionAccess(type, userId, invoiceTypeId, ActionTypeId);
        }

        public bool DosesUserHaveActionAccess(InvoiceActionType type, int userId, Guid invoiceTypeId, Guid ActionTypeId)
        {

            var invoiceActions = GetInvoiceActionsByActionId(invoiceTypeId);
            var invoiceActionType = invoiceActions.GetRecord(ActionTypeId);
            invoiceActionType.ThrowIfNull("Invoice Action ", ActionTypeId);
            invoiceActionType.Settings.ThrowIfNull("Invoice Action Settings", ActionTypeId);

            var context = new InvoiceActionSettingsCheckAccessContext
            {
                UserId = userId,
                InvoiceAction = invoiceActionType
            };

            if (invoiceActionType.Settings.Type != type || !invoiceActionType.Settings.DoesUserHaveAccess(context))
                return false;

            return true;
        }
        public IEnumerable<InvoiceType> GetInvoiceTypes()
        {
            return GetCachedInvoiceTypes().Values;
        }

        public string GetInvoicePartnerSelector(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            if (invoiceType != null && invoiceType.Settings != null && invoiceType.Settings.ExtendedSettings != null)
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
        public List<InvoiceBulkActionDefinitionEntity> GetMenualInvoiceBulkActionsDefinitions(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            var invoiceMenualBulkActions = invoiceType.Settings.InvoiceMenualBulkActions;
            List<InvoiceBulkActionDefinitionEntity> invoiceBulkActionDefinitionEntities = null;

            if (invoiceMenualBulkActions != null)
            {
                invoiceBulkActionDefinitionEntities = new List<InvoiceBulkActionDefinitionEntity>();

                foreach (var invoiceMenualBulkAction in invoiceMenualBulkActions)
                {
                    var invoiceBulkAction = invoiceType.Settings.InvoiceBulkActions.FindRecord(x => x.InvoiceBulkActionId == invoiceMenualBulkAction.InvoiceBulkActionId);
                    if (invoiceBulkAction != null)
                    {
                        invoiceBulkActionDefinitionEntities.Add(new InvoiceBulkActionDefinitionEntity
                        {
                            InvoiceBulkAction = invoiceBulkAction,
                            InvoiceMenualBulkAction = invoiceMenualBulkAction,
                            InvoiceAttachments = invoiceType.Settings.InvoiceAttachments
                        });
                    }
                }
            }
            return invoiceBulkActionDefinitionEntities;
        }
        public string GetPartnerName(Guid invoiceTypeId, string partnerId)
        {
            PartnerManager partnerManager = new PartnerManager();
            return partnerManager.GetPartnerName(invoiceTypeId, partnerId);
        }
        public string GetPartnerInvoiceSettingFilterFQTN(Guid invoiceTypeId)
        {
            PartnerManager partnerManager = new PartnerManager();
            return partnerManager.GetPartnerInvoiceSettingFilterFQTN(invoiceTypeId);
        }
        public InvoiceAttachment GetInvoiceAttachment(Guid invoiceTypeId, Guid invoiceAttachmentId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
            return GetInvoiceAttachment(invoiceType, invoiceAttachmentId);
        }
        public InvoiceAttachment GetInvoiceAttachment(InvoiceType invoiceType, Guid invoiceAttachmentId)
        {
            invoiceType.Settings.ThrowIfNull("invoiceType.Settings");
            invoiceType.Settings.InvoiceAttachments.ThrowIfNull("invoiceType.Settings.InvoiceAttachments");
            var invoiceAttachment = invoiceType.Settings.InvoiceAttachments.FirstOrDefault(x => x.InvoiceAttachmentId == invoiceAttachmentId);
            invoiceAttachment.ThrowIfNull("invoiceAttachment");
            return invoiceAttachment;
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
            return dataRecordFields.Select(x => x.Key);
        }
        public IEnumerable<InvoiceAttachmentInfo> GetRemoteInvoiceTypeAttachmentsInfo(Guid invoiceTypeId)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType = invoiceTypes.GetRecord(invoiceTypeId);
            if (invoiceType == null)
                throw new NullReferenceException(string.Format("invoiceType:{0}", invoiceTypeId));
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
        public InvoiceAttachment GeInvoiceTypeAttachment(Guid invoiceTypeId, Guid invoiceAttachmentId)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType = invoiceTypes.GetRecord(invoiceTypeId);
            if (invoiceType == null)
                throw new NullReferenceException(string.Format("invoiceType:{0}", invoiceTypeId));
            if (invoiceType.Settings.InvoiceAttachments != null && invoiceType.Settings.InvoiceAttachments.Count > 0)
            {
                return invoiceType.Settings.InvoiceAttachments.FindRecord(x => x.InvoiceAttachmentId == invoiceAttachmentId);
            }
            return null;
        }
        public IEnumerable<ItemSetNameStorageRule> GetItemSetNamesStorageRules(Guid invoiceTypeId)
        {
            var invoiceType = GetInvoiceType(invoiceTypeId);
            invoiceType.ThrowIfNull("invoiceType", invoiceTypeId);
            return invoiceType.Settings.ItemSetNamesStorageRules;
        }
        public string GetItemSetNameStorageInfo(Guid invoiceTypeId, string itemSetName)
        {
            var itemSetNamesStorageRules = GetItemSetNamesStorageRules(invoiceTypeId);
            if (itemSetNamesStorageRules != null)
            {
                foreach (var itemSetNamesStorageRule in itemSetNamesStorageRules)
                {
                    ItemSetNameStorageRuleContext context = new ItemSetNameStorageRuleContext
                    {
                        ItemSetName = itemSetName
                    };
                    if (itemSetNamesStorageRule.Settings.IsApplicable(context))
                        return context.StorageConnectionString;

                }
            }

            return null;
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

        private class InvoiceTypeLoggableEntity : VRLoggableEntityBase
        {
            public static InvoiceTypeLoggableEntity Instance = new InvoiceTypeLoggableEntity();

            private InvoiceTypeLoggableEntity()
            {

            }

            static InvoiceTypeManager s_invoiceTypeManager = new InvoiceTypeManager();

            public override string EntityUniqueName
            {
                get { return "VR_Invoice_InvoiceType"; }
            }

            public override string ModuleName
            {
                get { return "Invoice"; }
            }

            public override string EntityDisplayName
            {
                get { return "Invoice Type"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Invoice_InvoiceType_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                InvoiceType invoiceType = context.Object.CastWithValidate<InvoiceType>("context.Object");
                return invoiceType.InvoiceTypeId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                InvoiceType invoiceType = context.Object.CastWithValidate<InvoiceType>("context.Object");
                return s_invoiceTypeManager.GetInvoiceTypeName(invoiceType.InvoiceTypeId);
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
