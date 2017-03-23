
(function (appControllers) {

    "use strict";
    VRAlertLevelAPIService.$inject = ['BaseAPIService', 'UtilsService', 'VRNotification_ModuleConfig', 'SecurityService'];

    function VRAlertLevelAPIService(BaseAPIService, UtilsService, VRNotification_ModuleConfig, SecurityService) {

        var controllerName = "VRAlertLevel";


        function GetFilteredAlertLevels(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRNotification_ModuleConfig.moduleName, controllerName, 'GetFilteredAlertLevels'), input);
        }

        function AddAlertLevel(alertLevelItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRNotification_ModuleConfig.moduleName, controllerName, 'AddAlertLevel'), alertLevelItem);
        }

        function UpdateAlertLevel(alertLevelItem) {
            return BaseAPIService.post(UtilsService.getServiceURL(VRNotification_ModuleConfig.moduleName, controllerName, 'UpdateAlertLevel'), alertLevelItem);
        }

       
        function GetAlertLevel(alertLevelId) {
            return BaseAPIService.get(UtilsService.getServiceURL(VRNotification_ModuleConfig.moduleName, controllerName, 'GetAlertLevel'), {
                alertLevelId: alertLevelId
            });
        }
       

        return ({
            GetFilteredAlertLevels: GetFilteredAlertLevels,
            AddAlertLevel: AddAlertLevel,
            UpdateAlertLevel: UpdateAlertLevel,
            GetAlertLevel: GetAlertLevel,
           
        });
    }

    appControllers.service('VR_Notifacation_AlertLevetAPIService', VRAlertLevelAPIService);

})(appControllers);