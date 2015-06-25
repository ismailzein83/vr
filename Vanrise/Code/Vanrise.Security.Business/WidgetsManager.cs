using System;
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
        public Vanrise.Entities.InsertOperationOutput<Widget> SaveWidget(Widget widget)
        {
            InsertOperationOutput<Widget> insertOperationOutput = new InsertOperationOutput<Widget>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int widgetId = -1;
            IWidgetsDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
            bool insertActionSucc = dataManager.SaveWidget(widget, out widgetId);

            if (insertActionSucc)
            {

                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                widget.Id = widgetId;
                insertOperationOutput.InsertedObject = widget;
            }

            return insertOperationOutput;

        }
        public List<Widget> GetAllWidgets()
        {
            IWidgetsDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IWidgetsDataManager>();
            return dataManager.GetAllWidgets();
        }
      
    }
}
