(function (appControllers) {

    'use strict';

    WidgetAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig'];

    function WidgetAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig) {
        return ({
            GetFilteredWidgets: GetFilteredWidgets,
            GetAllWidgets: GetAllWidgets,
            GetWidgetById: GetWidgetById,
            AddWidget: AddWidget,
            UpdateWidget: UpdateWidget,
            DeleteWidget: DeleteWidget
        });
        
        function GetFilteredWidgets(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Widget', 'GetFilteredWidgets'), filter);
        }

        function GetAllWidgets() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Widget', 'GetAllWidgets'));
        }

        function GetWidgetById(widgetId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Widget', 'GetWidgetById'), {
                widgetId: widgetId
            });
        }

        function AddWidget(widget) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Widget', 'AddWidget'), widget);
        }

        function UpdateWidget(widget) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Widget', 'UpdateWidget'), widget);
        }
        
        function DeleteWidget(widgetId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, 'Widget', 'DeleteWidget'), {
                widgetId: widgetId
            });
        }
    }

    appControllers.service('VR_Sec_WidgetAPIService', WidgetAPIService);

})(appControllers);
