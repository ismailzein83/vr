
(function (appControllers) {

    "use strict";
    FirewallAPIService.$inject = ['BaseAPIService', 'UtilsService', 'NP_IVSwitch_ModuleConfig', 'SecurityService'];

    function FirewallAPIService(baseApiService, utilsService, npIvSwitchModuleConfig, securityService) {

        var controllerName = "Firewall";

        function GetFilteredFirewalls(input) {
            return baseApiService.post(utilsService.getServiceURL(npIvSwitchModuleConfig.moduleName, controllerName, 'GetFilteredFirewalls'), input);
        }

        function GetFirewall(firewallId) {
            return baseApiService.get(utilsService.getServiceURL(npIvSwitchModuleConfig.moduleName, controllerName, 'GetFirewall'), {
                firewallId: firewallId
            });
        }

        function AddFirewall(firewallItem) {
            return baseApiService.post(utilsService.getServiceURL(npIvSwitchModuleConfig.moduleName, controllerName, 'AddFirewall'), firewallItem);
        }

        function UpdateFirewall(firewallItem) {
            return baseApiService.post(utilsService.getServiceURL(npIvSwitchModuleConfig.moduleName, controllerName, 'UpdateFirewall'), firewallItem);
        }
        function HasAddFirewallPermission() {
            return SecurityService.HasPermissionToActions(utilsService.getSystemActionNames(npIvSwitchModuleConfig.moduleName, controllerName, ['AddFirewall']));
        }

        function HasEditFirewallPermission() {
            return securityService.HasPermissionToActions(utilsService.getSystemActionNames(npIvSwitchModuleConfig.moduleName, controllerName, ['UpdateFirewall']));
        }
        return ({
            GetFilteredFirewalls: GetFilteredFirewalls,
            GetFirewall: GetFirewall,
            AddFirewall: AddFirewall,
            UpdateFirewall: UpdateFirewall,
            HasEditFirewallPermission: HasEditFirewallPermission,
            HasAddFirewallPermission: HasAddFirewallPermission

        });
    }

    appControllers.service('NP_IVSwitch_FirewallAPIService', FirewallAPIService);

})(appControllers);