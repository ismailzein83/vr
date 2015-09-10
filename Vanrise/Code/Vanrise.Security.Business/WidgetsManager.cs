﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class WidgetsManager
    {
        public List<WidgetDefinition> GetWidgetsDefinition()
        {
            IWidgetsDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
            return dataManager.GetWidgetsDefinition();
        }
        public Vanrise.Entities.InsertOperationOutput<WidgetDetails> AddWidget(Widget widget)
        {
            InsertOperationOutput<WidgetDetails> insertOperationOutput = new InsertOperationOutput<WidgetDetails>();

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
                WidgetDetails widgetDetail = dataManager.GetWidgetById(widgetId);
                insertOperationOutput.InsertedObject = widgetDetail;
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }
            return insertOperationOutput;

        }
        public Vanrise.Entities.UpdateOperationOutput<WidgetDetails> UpdateWidget(Widget widget)
        {
            UpdateOperationOutput<WidgetDetails> updateOperationOutput = new UpdateOperationOutput<WidgetDetails>();

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
                WidgetDetails widgetDetail = dataManager.GetWidgetById(widget.Id);
                updateOperationOutput.UpdatedObject = widgetDetail;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;

        }

        public Vanrise.Entities.DeleteOperationOutput<WidgetDetails> DeleteWidget(int widgetId)
        {
            DeleteOperationOutput<WidgetDetails> deleteOperationOutput = new DeleteOperationOutput<WidgetDetails>();

            deleteOperationOutput.Result = DeleteOperationResult.Failed;
            IViewDataManager viewdataManager = SecurityDataManagerFactory.GetDataManager<IViewDataManager>();

            List<View>  dynamicViews = viewdataManager.GetDynamicPages();
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
        public List<WidgetDetails> GetAllWidgets()
        {
            IWidgetsDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
            return dataManager.GetAllWidgets();
        }

        public Vanrise.Entities.IDataRetrievalResult<WidgetDetails> GetFilteredWidgets(Vanrise.Entities.DataRetrievalInput<WidgetFilter> filter) 
        {
            IWidgetsDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(filter, dataManager.GetFilteredWidgets(filter));
        }
      
    }
}
