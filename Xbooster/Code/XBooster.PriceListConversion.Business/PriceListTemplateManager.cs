﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Security.Business;
using XBooster.PriceListConversion.Data;
using XBooster.PriceListConversion.Entities;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Common.Business;
namespace XBooster.PriceListConversion.Business
{
    public class PriceListTemplateManager
    {
        #region Fields

        int _loggedInUserId = new SecurityContext().GetLoggedInUserId();

        #endregion

        #region Public Methods

        public Vanrise.Entities.IDataRetrievalResult<PriceListTemplateDetail> GetFilteredInputPriceListTemplates(Vanrise.Entities.DataRetrievalInput<PriceListTemplateQuery> input)
        {
            var priceListTemplates = GetCachedPriceListTemplates();

            Func<PriceListTemplate, bool> filterExpression = (itm) =>
                 (itm.Type == Constants.OutputPriceListTemplate
                 && itm.UserId == _loggedInUserId
                 && (input.Query.Name == null || itm.Name == input.Query.Name));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, priceListTemplates.ToBigResult(input, filterExpression, PriceListTemplateDetailMapper));
        }
        public PriceListTemplate GetPriceListTemplate(int priceListTemplateId)
        {
            Dictionary<int, PriceListTemplate> priceListTemplates = GetCachedPriceListTemplates();
            PriceListTemplate priceListTemplate = priceListTemplates.GetRecord(priceListTemplateId);
            return (priceListTemplate != null && priceListTemplate.UserId == _loggedInUserId) ? priceListTemplate : null;
        }
        public Vanrise.Entities.InsertOperationOutput<PriceListTemplateDetail> AddOutputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            InsertOperationOutput<PriceListTemplateDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PriceListTemplateDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int priceListTemplateId = -1;

            IPriceListTemplateDataManager dataManager = PriceListConversionDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
            priceListTemplate.UserId = _loggedInUserId;
            priceListTemplate.Type = Constants.OutputPriceListTemplate;
            bool insertActionSucc = dataManager.InsertPriceListTemplate(priceListTemplate, out priceListTemplateId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                priceListTemplate.PriceListTemplateId = priceListTemplateId;
                insertOperationOutput.InsertedObject = PriceListTemplateDetailMapper(priceListTemplate);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<PriceListTemplateDetail> UpdateOutputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            IPriceListTemplateDataManager dataManager = PriceListConversionDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
            priceListTemplate.UserId = _loggedInUserId;
            priceListTemplate.Type = Constants.OutputPriceListTemplate;
            bool updateActionSucc = dataManager.UpdatePriceListTemplate(priceListTemplate);
            UpdateOperationOutput<PriceListTemplateDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<PriceListTemplateDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PriceListTemplateDetailMapper(priceListTemplate);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }


        public Vanrise.Entities.InsertOperationOutput<PriceListTemplateDetail> AddInputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            InsertOperationOutput<PriceListTemplateDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<PriceListTemplateDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int priceListTemplateId = -1;

            IPriceListTemplateDataManager dataManager = PriceListConversionDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
            priceListTemplate.UserId = _loggedInUserId;
            priceListTemplate.Type = Constants.InputPriceListTemplate;
            bool insertActionSucc = dataManager.InsertPriceListTemplate(priceListTemplate, out priceListTemplateId);

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                priceListTemplate.PriceListTemplateId = priceListTemplateId;
                insertOperationOutput.InsertedObject = PriceListTemplateDetailMapper(priceListTemplate);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<PriceListTemplateDetail> UpdateInputPriceListTemplate(PriceListTemplate priceListTemplate)
        {
            IPriceListTemplateDataManager dataManager = PriceListConversionDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
            priceListTemplate.UserId = _loggedInUserId;
            priceListTemplate.Type = Constants.InputPriceListTemplate;
            bool updateActionSucc = dataManager.UpdatePriceListTemplate(priceListTemplate);
            UpdateOperationOutput<PriceListTemplateDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<PriceListTemplateDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = PriceListTemplateDetailMapper(priceListTemplate);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }


        public IEnumerable<TemplateConfig> GetOutputPriceListConfigurationTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.OutputPriceListConfigurationTemplateConfigs);
        }
        public IEnumerable<TemplateConfig> GetInputPriceListConfigurationTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.InputPriceListConfigurationTemplateConfigs);
        }
        public IEnumerable<PriceListTemplateInfo> GetOutputPriceListTemplates(PriceListTemplateFilter filter)
        {
            Dictionary<int, PriceListTemplate> priceListTemplates = GetCachedPriceListTemplates();
            return priceListTemplates.Values.MapRecords(PriceListTemplateInfoMapper,itm => itm.UserId == _loggedInUserId && itm.Type == Constants.OutputPriceListTemplate);
        }
        public IEnumerable<PriceListTemplateInfo> GetInputPriceListTemplates(PriceListTemplateFilter filter)
        {
            Dictionary<int, PriceListTemplate> priceListTemplates = GetCachedPriceListTemplates();
            return priceListTemplates.Values.MapRecords(PriceListTemplateInfoMapper, itm => itm.UserId == _loggedInUserId && itm.Type == Constants.InputPriceListTemplate);
        }
        public IEnumerable<TemplateConfig> GetOutputFieldMappingTemplateConfigs()
        {
            var templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.OutputFieldMappingTemplateConfigs);
        }
        #endregion

        #region Private Classes

        class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IPriceListTemplateDataManager _dataManager = PriceListConversionDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArePriceListTemplatesUpdated(ref _updateHandle);
            }
        }

        #endregion

        #region Private Methods

        private Dictionary<int, PriceListTemplate> GetCachedPriceListTemplates()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetPriceListTemplates", () =>
            {
                IPriceListTemplateDataManager dataManager = PriceListConversionDataManagerFactory.GetDataManager<IPriceListTemplateDataManager>();
                IEnumerable<PriceListTemplate> priceListTemplates = dataManager.GetPriceListTemplates();
                return priceListTemplates.ToDictionary(kvp => kvp.PriceListTemplateId, kvp => kvp);
            });
        }

        #endregion

        #region Mapper

        private PriceListTemplateDetail PriceListTemplateDetailMapper(PriceListTemplate priceListTemplate)
        {
            return new PriceListTemplateDetail
            {
                Entity = priceListTemplate
            };
        }
        private PriceListTemplateInfo PriceListTemplateInfoMapper(PriceListTemplate priceListTemplate)
        {
            return new PriceListTemplateInfo
            {
                Name = priceListTemplate.Name,
                PriceListTemplateId = priceListTemplate.PriceListTemplateId,
            };
        }
        #endregion
    }
}
