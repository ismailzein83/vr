using Demo.Module.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Caching;
using Vanrise.Common;
using Vanrise.Entities;
using Demo.Module.Entities;
using Demo.Module.Entities.ProductInfo;
using Vanrise.Common.Business;

public class PageDefinitionManager
{
    #region Public Methods


    public IEnumerable<PageDefinitionInfo> GetPageDefinitionsInfo()
    {

        var allPageDefinitions = GetCachedPageDefinitions();
        Func<PageDefinition, bool> filterFunc = (PageDefinition) =>
        {
            return true;
        };
        return allPageDefinitions.MapRecords(PageDefinitionInfoMapper, filterFunc).OrderBy(pageDefinition => pageDefinition.Name);

    }

    public IDataRetrievalResult<PageDefinitionDetails> GetFilteredPageDefinitions(DataRetrievalInput<PageDefinitionQuery> input)
    {
        var allPageDefinitions = GetCachedPageDefinitions();
        Func<PageDefinition, bool> filterExpression = (pageDefinition) =>
        {
            if (input.Query.Name != null && !pageDefinition.Name.ToLower().Contains(input.Query.Name.ToLower()))
                return false;

            return true;
        };
        return DataRetrievalManager.Instance.ProcessResult(input, allPageDefinitions.ToBigResult(input, filterExpression, PageDefinitionDetailMapper));

    }
    public IEnumerable<FieldTypeConfig> GetFieldTypeConfigs()
    {
        var extensionConfigurationManager = new ExtensionConfigurationManager();
        return extensionConfigurationManager.GetExtensionConfigurations<FieldTypeConfig>(FieldTypeConfig.EXTENSION_TYPE);
    }

    public IEnumerable<SubViewConfig> GetSubViewConfigs()
    {
        var extensionConfigurationManager = new ExtensionConfigurationManager();
        return extensionConfigurationManager.GetExtensionConfigurations<SubViewConfig>(SubViewConfig.EXTENSION_TYPE);
    }

    public IEnumerable<PageDefinitionConfig> GetPageDefinitionConfigs()
    {
        var extensionConfigurationManager = new ExtensionConfigurationManager();
        return extensionConfigurationManager.GetExtensionConfigurations<PageDefinitionConfig>(PageDefinitionConfig.EXTENSION_TYPE);
    }
    public InsertOperationOutput<PageDefinitionDetails> AddPageDefinition(PageDefinition pageDefinition)
    {
        IPageDefinitionDataManager pageDefinitionDataManager = DemoModuleFactory.GetDataManager<IPageDefinitionDataManager>();
        InsertOperationOutput<PageDefinitionDetails> insertOperationOutput = new InsertOperationOutput<PageDefinitionDetails>();
        insertOperationOutput.Result = InsertOperationResult.Failed;
        insertOperationOutput.InsertedObject = null;
        int pageDefinitionId = -1;

        bool insertActionSuccess = pageDefinitionDataManager.Insert(pageDefinition, out pageDefinitionId);
        if (insertActionSuccess)
        {
            pageDefinition.PageDefinitionId = pageDefinitionId;
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            insertOperationOutput.Result = InsertOperationResult.Succeeded;
            insertOperationOutput.InsertedObject = PageDefinitionDetailMapper(pageDefinition);
        }
        else
        {
            insertOperationOutput.Result = InsertOperationResult.SameExists;
        }
        return insertOperationOutput;
    }
    public PageDefinition GetPageDefinitionById(int pageDefinitionId)
    {
        var allPageDefinitions = GetCachedPageDefinitions();
        return allPageDefinitions.GetRecord(pageDefinitionId);
    }

    public UpdateOperationOutput<PageDefinitionDetails> UpdatePageDefinition(PageDefinition pageDefinition)
    {
        IPageDefinitionDataManager pageDefinitionDataManager = DemoModuleFactory.GetDataManager<IPageDefinitionDataManager>();
        UpdateOperationOutput<PageDefinitionDetails> updateOperationOutput = new UpdateOperationOutput<PageDefinitionDetails>();
        updateOperationOutput.Result = UpdateOperationResult.Failed;
        updateOperationOutput.UpdatedObject = null;
        bool updateActionSuccess = pageDefinitionDataManager.Update(pageDefinition);
        if (updateActionSuccess)
        {
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            updateOperationOutput.UpdatedObject = PageDefinitionDetailMapper(pageDefinition);
        }
        else
        {
            updateOperationOutput.Result = UpdateOperationResult.SameExists;
        }
        return updateOperationOutput;
    }
    #endregion

    #region Private Classes
    private class CacheManager : Vanrise.Caching.BaseCacheManager
    {
        IPageDefinitionDataManager pageDefinitionDataManager = DemoModuleFactory.GetDataManager<IPageDefinitionDataManager>();
        object _updateHandle;
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return pageDefinitionDataManager.ArePageDefinitionsUpdated(ref _updateHandle);
        }
    }
    #endregion

    #region Private Methods

    private Dictionary<int, PageDefinition> GetCachedPageDefinitions()
    {
        return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
           .GetOrCreateObject("GetCachedPageDefinitions", () =>
           {
               IPageDefinitionDataManager pageDefinitionDataManager = DemoModuleFactory.GetDataManager<IPageDefinitionDataManager>();
               List<PageDefinition> pageDefinitions = pageDefinitionDataManager.GetPageDefinitions();
               return pageDefinitions.ToDictionary(pageDefinition => pageDefinition.PageDefinitionId, pageDefinition => pageDefinition);
           });
    }
    #endregion

    #region Mappers
    public PageDefinitionDetails PageDefinitionDetailMapper(PageDefinition pageDefinition)
    {
        var pageDefinitionDetails = new PageDefinitionDetails
        {
            Name = pageDefinition.Name,
            PageDefinitionId = pageDefinition.PageDefinitionId,
        };
        if (pageDefinition.Details != null)
        {
              if (pageDefinition.Details.Fields != null)
              {
                  pageDefinitionDetails.Fields = new List<Field>();
                  for (var i = 0; i < pageDefinition.Details.Fields.Count; i++)
                  {
                     Field field=new Field();
                      field.Title = pageDefinition.Details.Fields[i].Title;
                      field.Name = pageDefinition.Details.Fields[i].Name;
                      pageDefinitionDetails.Fields.Add(field);
                  }
              }
              if (pageDefinition.Details.SubViews != null)
              {
                  pageDefinitionDetails.SubViews = new List<SubView>();
                  for (var i = 0; i < pageDefinition.Details.SubViews.Count; i++)
                  {
                      SubView subView = new SubView();
                      subView.Title = pageDefinition.Details.SubViews[i].Title;
                      subView.Name = pageDefinition.Details.SubViews[i].Name;
                      pageDefinitionDetails.SubViews.Add(subView);
                  }
              }
        }

          
        return pageDefinitionDetails;
    }

    public PageDefinitionInfo PageDefinitionInfoMapper(PageDefinition pageDefinition)
    {
        return new PageDefinitionInfo
        {
            Name = pageDefinition.Name,
            PageDefinitionId = pageDefinition.PageDefinitionId,
        };

    }
    #endregion
    
}