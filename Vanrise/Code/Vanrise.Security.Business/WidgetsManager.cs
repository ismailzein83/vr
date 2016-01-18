using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Common;
namespace Vanrise.Security.Business
{
    public class WidgetsManager
    {
        private WidgetDefinitionManager _widgetDefinitionManager;
        public WidgetsManager()
        {
            _widgetDefinitionManager = new WidgetDefinitionManager();
        }

        public Vanrise.Entities.IDataRetrievalResult<WidgetDetail> GetFilteredWidgets(Vanrise.Entities.DataRetrievalInput<WidgetFilter> input)
        {
            var allWidgets = GetCachedWidgets().Values;

            Func<Widget, bool> filterExpression = (prod) =>
                 (input.Query.WidgetName == null || prod.Name.ToLower().Contains(input.Query.WidgetName.ToLower()))
                 &&

                 (input.Query.WidgetTypes == null  || input.Query.WidgetTypes.Contains(prod.WidgetDefinitionId));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allWidgets.ToBigResult(input, filterExpression, WidgetDetailMapper));
        }


        public Vanrise.Entities.InsertOperationOutput<WidgetDetail> AddWidget(Widget widget)
        {
            InsertOperationOutput<WidgetDetail> insertOperationOutput = new InsertOperationOutput<WidgetDetail>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int widgetId = -1;
            IWidgetsDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();

            int checkSetting = dataManager.CheckWidgetSetting(widget);
            if (checkSetting == 1)
            {
                insertOperationOutput.Message = "Widget with same settings exist";
                return insertOperationOutput;
            }

            bool insertActionSucc = dataManager.AddWidget(widget, out widgetId);
            if (insertActionSucc)
            {

                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                widget.Id = widgetId;
                WidgetDetail widgetDetail = WidgetDetailMapper(widget);
                insertOperationOutput.InsertedObject = widgetDetail;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }
            return insertOperationOutput;

        }
        public Vanrise.Entities.UpdateOperationOutput<WidgetDetail> UpdateWidget(Widget widget)
        {
            UpdateOperationOutput<WidgetDetail> updateOperationOutput = new UpdateOperationOutput<WidgetDetail>();

            updateOperationOutput.Result = UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IWidgetsDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
            int checkSetting = dataManager.CheckWidgetSetting(widget);
            if (checkSetting == 1)
            {
                updateOperationOutput.Message = "Widget with same settings exist";
                return updateOperationOutput;
            }
            bool updateActionSucc = dataManager.UpdateWidget(widget);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = UpdateOperationResult.Succeeded;
                WidgetDetail widgetDetail = WidgetDetailMapper(widget);
                updateOperationOutput.UpdatedObject = widgetDetail;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;

        }

        public Vanrise.Entities.DeleteOperationOutput<WidgetDetail> DeleteWidget(int widgetId)
        {
            DeleteOperationOutput<WidgetDetail> deleteOperationOutput = new DeleteOperationOutput<WidgetDetail>();

            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            ViewManager viewManager = new ViewManager();

            IEnumerable<View> dynamicViews = viewManager.GetDynamicViews();
            foreach (View dynamicView in dynamicViews)
            {
                foreach (ViewContentItem bodyContent in dynamicView.ViewContent.BodyContents)
                {
                    if (bodyContent.WidgetId == widgetId)
                    {
                        deleteOperationOutput.Result = DeleteOperationResult.InUse;
                        return deleteOperationOutput;
                    }
                }
                foreach (ViewContentItem summaryContent in dynamicView.ViewContent.SummaryContents)
                {
                    if (summaryContent.WidgetId == widgetId)
                    {
                        deleteOperationOutput.Result = DeleteOperationResult.InUse;
                        return deleteOperationOutput;
                    }
                }
                
            }
            IWidgetsDataManager widgetdataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
            bool deleteActionSucc = widgetdataManager.DeleteWidget(widgetId);

            if (deleteActionSucc)
            {
                deleteOperationOutput.Result = DeleteOperationResult.Succeeded;
            }

            return deleteOperationOutput;

        }

        public IEnumerable<WidgetDetail> GetAllWidgets()
        {
            var allWidgets = GetCachedWidgets().Values;

            return allWidgets.MapRecords(WidgetDetailMapper);
        }

        public Widget GetWidgetById(int widgetId)
        {
            var allWidgets = GetCachedWidgets();

            return allWidgets.GetRecord(widgetId);
        }    

        #region Private Members

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IWidgetsDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreAllWidgetsUpdated(ref _updateHandle);
            }
        }
        private Dictionary<int, Widget> GetCachedWidgets()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetWidgets",
               () =>
               {
                   IWidgetsDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
                   IEnumerable<Widget> widgets = dataManager.GetAllWidgets();
                   return widgets.ToDictionary(kvp => kvp.Id, kvp => kvp);
               });
        }
      
        #endregion

        #region  Mappers
        private WidgetDetail WidgetDetailMapper(Widget widget)
        {
            WidgetDetail widgetDetails = new WidgetDetail();
            WidgetDefinition widgetDefinition = _widgetDefinitionManager.GetWidgetDefinitionById(widget.WidgetDefinitionId);
            widgetDetails.Entity = widget;
            if(widgetDefinition != null)
            {
                 widgetDetails.WidgetDefinitionName = widgetDefinition.Name;
                 widgetDetails.DirectiveName = widgetDefinition.DirectiveName;
                 widgetDetails.WidgetDefinitionSetting = widgetDefinition.Setting;
            }
            return widgetDetails;
        }

        #endregion
    }
}
