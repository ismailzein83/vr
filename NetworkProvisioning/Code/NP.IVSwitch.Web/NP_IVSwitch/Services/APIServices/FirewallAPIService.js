
(function (appControllers) {

    "use strict";
    FirewallAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig'];

    function FirewallAPIService(baseApiService, utilsService, npIvSwitchModuleConfig) {

        var controllerName = "Firewall";

        function GetFilteredFirewalls(input) {
            return baseApiService.post(utilsService.getServiceURL(npIvSwitchModuleConfig.moduleName, controllerName, 'GetFilteredFirewalls'), input);
        }

        return ({
            GetFilteredFirewalls: GetFilteredFirewalls
        });
    }

    appControllers.service('NP_IVSwitch_FirewallAPIService', FirewallAPIService);

})(appControllers);