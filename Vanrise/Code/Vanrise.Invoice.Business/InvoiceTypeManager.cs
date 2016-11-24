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
namespace Vanrise.Invoice.Business
{
    public class InvoiceTypeManager
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
                        attribute = fieldType.Type.GetGridColumnAttribute();
                }

                invoiceTypeRuntime.MainGridRuntimeColumns.Add(new InvoiceUIGridColumnRunTime
                {
                    CustomFieldName = gridColumn.CustomFieldName,
                    Attribute = attribute,
                    Field = gridColumn.Field,
                    Header = gridColumn.Header
                });
            }
            invoiceTypeRuntime.InvoicePartnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerSettings();
     
            return invoiceTypeRuntime;
        }
        public GeneratorInvoiceTypeRuntime GetGeneratorInvoiceTypeRuntime(Guid invoiceTypeId)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType = invoiceTypes.GetRecord(invoiceTypeId);
            GeneratorInvoiceTypeRuntime generatorInvoiceTypeRuntime = new Entities.GeneratorInvoiceTypeRuntime();
            generatorInvoiceTypeRuntime.InvoiceType = invoiceType;
            generatorInvoiceTypeRuntime.InvoicePartnerSettings = invoiceType.Settings.ExtendedSettings.GetPartnerSettings();
            return generatorInvoiceTypeRuntime;
        }
        public List<InvoiceGeneratorAction> GetInvoiceGeneratorActions(GenerateInvoiceInput generateInvoiceInput)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            var invoiceType = invoiceTypes.GetRecord(generateInvoiceInput.InvoiceTypeId);
            InvoiceGeneratorActionFilterConditionContext context = new InvoiceGeneratorActionFilterConditionContext
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
        public IEnumerable<GridColumnAttribute> CovertToGridColumnAttribute(ConvertToGridColumnAttributeInput input)
        {
            List<GridColumnAttribute> gridColumnAttributes = null;
            if(input.GridColumns != null)
            {
                gridColumnAttributes = new List<GridColumnAttribute>();
                foreach(var column in input.GridColumns)
                {
                    if (column.FieldType == null)
                        throw new NullReferenceException(string.Format("{0} is not mapped to field type.", column.FieldName));
                    var gridAttribute = column.FieldType.GetGridColumnAttribute();
                    gridAttribute.HeaderText = column.Header;
                    gridAttribute.Field = column.FieldName;
                    gridColumnAttributes.Add(gridAttribute);
                }
            }
            return gridColumnAttributes;

        }
        public IEnumerable<InvoiceTypeInfo> GetInvoiceTypesInfo(InvoiceTypeFilter filter)
        {
            var invoiceTypes = GetCachedInvoiceTypes();
            if (filter != null)
            {
            }
            return invoiceTypes.MapRecords(InvoiceTypeInfoMapper);
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
