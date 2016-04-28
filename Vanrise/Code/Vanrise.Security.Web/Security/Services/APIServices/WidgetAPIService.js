(function (appControllers) {

    'use strict';

    WidgetAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Sec_ModuleConfig','SecurityService'];

    function WidgetAPIService(BaseAPIService, UtilsService, VR_Sec_ModuleConfig, SecurityService) {
        var controllerName = "Widget";
        return ({
            GetFilteredWidgets: GetFilteredWidgets,
            GetAllWidgets: GetAllWidgets,
            GetWidgetById: GetWidgetById,
            AddWidget: AddWidget,
            UpdateWidget: UpdateWidget,
            DeleteWidget: DeleteWidget,
            HasAddWidgetPermission: HasAddWidgetPermission,
            HasUpdateWidgetPermission: HasUpdateWidgetPermission,
            HasDeleteWidgetPermission: HasDeleteWidgetPermission
        });
        
        function GetFilteredWidgets(filter) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetFilteredWidgets'), filter);
        }

        function GetAllWidgets() {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetAllWidgets'));
        }

        function GetWidgetById(widgetId)
        {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'GetWidgetById'), {
                widgetId: widgetId
            });
        }

        function AddWidget(widget) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'AddWidget'), widget);
        }

        function UpdateWidget(widget) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'UpdateWidget'), widget);
        }
        
        function DeleteWidget(widgetId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Sec_ModuleConfig.moduleName, controllerName, 'DeleteWidget'), {
                widgetId: widgetId
            });
        }

        function HasAddWidgetPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['AddWidget']));
        }

        function HasUpdateWidgetPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['UpdateWidget']));
        }
        function HasDeleteWidgetPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(VR_Sec_ModuleConfig.moduleName, controllerName, ['DeleteWidget']));
        }

    }

    appControllers.service('VR_Sec_WidgetAPIService', WidgetAPIService);

})(appControllers);
