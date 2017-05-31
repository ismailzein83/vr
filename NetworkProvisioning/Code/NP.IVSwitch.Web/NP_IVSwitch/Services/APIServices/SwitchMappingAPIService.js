
(function (appControllers) {
    "use strict";
    SwitchMappingAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function SwitchMappingAPIService(BaseAPIService, UtilsService, NP_IVSwitch_ModuleConfig, SecurityService) {

        var controllerName = "SwitchMapping";


        function GetFilteredSwitchMappings(input) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'GetFilteredSwitchMappings'), input);
        }
        function LinkCarrierToEndPoints(endPointLink) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'LinkCarrierToEndPoints'), endPointLink);
        }
        function HasLinkEndPointsPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['LinkCarrierToEndPoints']));
        }
        function LinkCarrierToRoutes(endPointLink) {
            return BaseAPIService.post(UtilsService.getServiceURL(NP_IVSwitch_ModuleConfig.moduleName, controllerName, 'LinkCarrierToRoutes'), endPointLink);
        }
        function HasLinkRoutesPermission() {
            return SecurityService.HasPermissionToActions(UtilsService.getSystemActionNames(NP_IVSwitch_ModuleConfig.moduleName, controllerName, ['LinkCarrierToRoutes']));
        }
        return ({
            GetFilteredSwitchMappings: GetFilteredSwitchMappings,
            LinkCarrierToEndPoints: LinkCarrierToEndPoints,
            LinkCarrierToRoutes: LinkCarrierToRoutes,
            HasLinkEndPointsPermission: HasLinkEndPointsPermission,
            HasLinkRoutesPermission: HasLinkRoutesPermission
        });
    }
    appControllers.service('NP_IVSwitch_SwitchMappingAPIService', SwitchMappingAPIService);

})(appControllers);