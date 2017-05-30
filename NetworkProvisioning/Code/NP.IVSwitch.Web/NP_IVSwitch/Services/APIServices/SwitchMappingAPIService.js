
(function (appControllers) {
    "use strict";
    SwitchMappingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function SwitchMappingAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "SwitchMapping";


        function GetFilteredSwitchMappings(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredSwitchMappings'), input);
        }
        return ({
            GetFilteredSwitchMappings: GetFilteredSwitchMappings
        });
    }
    appControllers.service('NP_IVSwitch_SwitchMappingAPIService', SwitchMappingAPIService);

})(appControllers);