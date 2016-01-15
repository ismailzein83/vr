(function (appControllers) {

    "use strict";
    widgetAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function widgetAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {

        function GetWidgetsDefinition() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widgets", "GetWidgetsDefinition"));
        }
        function AddWidget(widget) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widgets", "AddWidget"), widget);
        }
        function UpdateWidget(widget) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widgets", "UpdateWidget"), widget);
        }
        function GetAllWidgets() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widgets", "GetAllWidgets"));
        }
        function GetFilteredWidgets(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widgets", "GetFilteredWidgets"), filter);
        }
        function DeleteWidget(widgetId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, "Widgets", "DeleteWidget"), {
                widgetId: widgetId
            });
        }
        return ({
            AddWidget: AddWidget,
            GetAllWidgets: GetAllWidgets,
            UpdateWidget: UpdateWidget,
            GetFilteredWidgets: GetFilteredWidgets,
            DeleteWidget: DeleteWidget
        });
    }

    appControllers.service('WidgetAPIService', widgetAPIService);

})(appControllers);