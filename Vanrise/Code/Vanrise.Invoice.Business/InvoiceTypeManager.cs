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
        public IEnumerable<InvoiceGeneratorConfig> GetInvoiceGeneratorConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<InvoiceGeneratorConfig>(InvoiceGeneratorConfig.EXTENSION_TYPE);
        }
        public IEnumerable<InvoiceGridActionSettingsConfig> GetInvoiceGridActionSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<InvoiceGridActionSettingsConfig>(InvoiceGridActionSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<RDLCDataSourceSettingsConfig> GetRDLCDataSourceSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<RDLCDataSourceSettingsConfig>(RDLCDataSourceSettingsConfig.EXTENSION_TYPE);
        }
        public IEnumerable<RDLCParameterSettingsConfig> GetRDLCParameterSettingsConfigs()
        {
            var extensionConfiguration = new ExtensionConfigurationManager();
            return extensionConfiguration.GetExtensionConfigurations<RDLCParameterSettingsConfig>(RDLCParameterSettingsConfig.EXTENSION_TYPE);
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
        #endregion

    } 
}
