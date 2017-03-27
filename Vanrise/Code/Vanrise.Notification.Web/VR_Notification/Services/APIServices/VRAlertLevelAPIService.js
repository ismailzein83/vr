
(function (appControllers) {

    "use strict";
    VRAlertLevelAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VR_Notification_ModuleConfig', 'SecurityService'];

    function VRAlertLevelAPIService(BaseAPIService, UtilsService, VR_Notification_ModuleConfig, SecurityService) {

        var controllerName = "VRAlertLevel";


        function GetFilteredAlertLevels(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetFilteredAlertLevels'), input);
        }

        function AddAlertLevel(alertLevelItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'AddAlertLevel'), alertLevelItem);
        }

        function UpdateAlertLevel(alertLevelItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'UpdateAlertLevel'), alertLevelItem);
        }
        
       
        function GetAlertLevel(alertLevelId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, 'GetAlertLevel'), {
                alertLevelId: alertLevelId
            });
        }
       
        function GetAlertLevelsInfo(filter) {
            return BaseAPIService.get(UtilsService.getServiceURL(VR_Notification_ModuleConfig.moduleName, controllerName, "GetAlertLevelsInfo"), {
                filter: filter
            });
        }
        return ({
            GetFilteredAlertLevels: GetFilteredAlertLevels,
            AddAlertLevel: AddAlertLevel,
            UpdateAlertLevel: UpdateAlertLevel,
            GetAlertLevel: GetAlertLevel,
            GetAlertLevelsInfo: GetAlertLevelsInfo,
           
        });
    }

    appControllers.service('VR_Notification_AlertLevelAPIService', VRAlertLevelAPIService);

})(appControllers);