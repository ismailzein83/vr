(function (appControllers) {

    "use strict";
    widgetAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function widgetAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {

        function GetWidgetById(widgetId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widget", "GetWidgetById"), {
                widgetId: widgetId
            });
        }
        function AddWidget(widget) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widget", "AddWidget"), widget);
        }
        function UpdateWidget(widget) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widget", "UpdateWidget"), widget);
        }
        function GetAllWidgets() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widget", "GetAllWidgets"));
        }
        function GetFilteredWidgets(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widget", "GetFilteredWidgets"), filter);
        }
        function DeleteWidget(widgetId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widget", "DeleteWidget"), {
                widgetId: widgetId
            });
        }
        return ({
            AddWidget: AddWidget,
            GetAllWidgets: GetAllWidgets,
            UpdateWidget: UpdateWidget,
            GetFilteredWidgets: GetFilteredWidgets,
            DeleteWidget: DeleteWidget,
            GetWidgetById: GetWidgetById
        });
    }

    appControllers.service('WidgetAPIService', widgetAPIService);

})(appControllers);