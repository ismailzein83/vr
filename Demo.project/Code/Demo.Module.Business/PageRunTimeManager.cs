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
using Vanrise.Common.Business;

public class PageRunTimeManager
{
    #region Public Methods

    PageDefinitionManager pageDefinitionManager = new PageDefinitionManager();

    public IDataRetrievalResult<PageRunTimeDetails> GetFilteredPageRunTimes(DataRetrievalInput<PageRunTimeQuery> input)
    {
        var allPageRunTimes = GetCachedPageRunTimes();
        Func<PageRunTime, bool> filterExpression = (pageRunTime) =>
        {
            if (input.Query.PageDefinitionId.HasValue && pageRunTime.PageDefinitionId != input.Query.PageDefinitionId.Value)
                 return false;

            PageDefinition pageDefinition = pageDefinitionManager.GetPageDefinitionById(pageRunTime.PageDefinitionId);
            if (input.Query.Filters != null)
            {
                foreach (var filter in input.Query.Filters)
                {
                    foreach (var field in pageDefinition.Details.Fields)
                    {
                        if (field.Name == filter.Key)
                        {
                            foreach (var item in pageRunTime.Details.FieldValues)
                            {
                                if (field.Name == item.Key)
                                {
                                    if (!field.FieldType.IsMatch(pageRunTime.Details.FieldValues[field.Name], filter.Value) /*|| pageRunTime.Details.FieldValues[field.Name]==null*/)
                                        return false; break;
                                }

                            }
                           
                        }
                    }
                }
            }
            return true;
        };
        return DataRetrievalManager.Instance.ProcessResult(input, allPageRunTimes.ToBigResult(input, filterExpression, PageRunTimeDetailMapper));

    }

    public InsertOperationOutput<PageRunTimeDetails> AddPageRunTime(PageRunTime pageRunTime)
    {
        IPageRunTimeDataManager pageRunTimeDataManager = DemoModuleFactory.GetDataManager<IPageRunTimeDataManager>();
        InsertOperationOutput<PageRunTimeDetails> insertOperationOutput = new InsertOperationOutput<PageRunTimeDetails>();
        insertOperationOutput.Result = InsertOperationResult.Failed;
        insertOperationOutput.InsertedObject = null;
        int pageRunTimeId = -1;
        
        PageDefinition pageDefinition = pageDefinitionManager.GetPageDefinitionById(pageRunTime.PageDefinitionId);

        foreach(var field in pageDefinition.Details.Fields){
           
            if (!pageRunTime.Details.FieldValues.ContainsKey(field.Name))  
                pageRunTime.Details.FieldValues.Add(field.Name, null);
        }

        bool insertActionSuccess = pageRunTimeDataManager.Insert(pageRunTime, out pageRunTimeId);
        if (insertActionSuccess)
        {
            pageRunTime.PageRunTimeId = pageRunTimeId;
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            insertOperationOutput.Result = InsertOperationResult.Succeeded;
            insertOperationOutput.InsertedObject = PageRunTimeDetailMapper(pageRunTime);
        }
        else
        {
            insertOperationOutput.Result = InsertOperationResult.SameExists;
        }
        return insertOperationOutput;
    }

    public PageRunTime GetPageRunTimeById(int pageRunTimeId)
    {
        var allPageRunTimes = GetCachedPageRunTimes();
        return allPageRunTimes.GetRecord(pageRunTimeId);
    }

    public UpdateOperationOutput<PageRunTimeDetails> UpdatePageRunTime(PageRunTime pageRunTime)
    {
        IPageRunTimeDataManager pageRunTimeDataManager = DemoModuleFactory.GetDataManager<IPageRunTimeDataManager>();
        UpdateOperationOutput<PageRunTimeDetails> updateOperationOutput = new UpdateOperationOutput<PageRunTimeDetails>();
        updateOperationOutput.Result = UpdateOperationResult.Failed;
        updateOperationOutput.UpdatedObject = null;
        bool updateActionSuccess = pageRunTimeDataManager.Update(pageRunTime);
        if (updateActionSuccess)
        {
            CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            updateOperationOutput.Result = UpdateOperationResult.Succeeded;
            updateOperationOutput.UpdatedObject = PageRunTimeDetailMapper(pageRunTime);
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
        IPageRunTimeDataManager pageRunTimeDataManager = DemoModuleFactory.GetDataManager<IPageRunTimeDataManager>();
        object _updateHandle;
       protected override bool ShouldSetCacheExpired(object parameter)
        {
            return pageRunTimeDataManager.ArePageRunTimesUpdated(ref _updateHandle);
        }
    }
    #endregion

    #region Private Methods

    private Dictionary<int, PageRunTime> GetCachedPageRunTimes()
    {
        return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
           .GetOrCreateObject("GetCachedPageRunTimes", () =>
           {
               IPageRunTimeDataManager pageRunTimeDataManager = DemoModuleFactory.GetDataManager<IPageRunTimeDataManager>();
               List<PageRunTime> pageRunTimes = pageRunTimeDataManager.GetPageRunTimes();
               return pageRunTimes.ToDictionary(pageRunTime => pageRunTime.PageRunTimeId, pageRunTime => pageRunTime);
           });
    }
    #endregion

    #region Mappers
    public PageRunTimeDetails PageRunTimeDetailMapper(PageRunTime pageRunTime)
    {
        var pageRunTimeDetails = new PageRunTimeDetails
        {
           PageDefinitionId = pageRunTime.PageDefinitionId,
           PageRunTimeId = pageRunTime.PageRunTimeId,
        };
        if (pageRunTime.Details != null)
        {
            if (pageRunTime.Details.FieldValues != null)
              {
                  pageRunTimeDetails.FieldValues= new Dictionary<string,object>();
                  foreach (var field in pageRunTime.Details.FieldValues)
                  {
                      pageRunTimeDetails.FieldValues.Add(field.Key, field.Value);
                  }
              }
        }


        return pageRunTimeDetails;
    }

   
    #endregion
    
}