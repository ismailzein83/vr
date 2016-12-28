
(function (appControllers) {

    "use strict";

    FirewallService.$inject = ['VRModalService'];

    function FirewallService(NPModalService) {

        function addFirewall(onFirewallAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onFirewallAdded = onFirewallAdded;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Firewall/FirewallEditor.html', null, settings);
        };

        function editFirewall(firewallId, onFirewallUpdated) {
            var settings = {};
            var parameters = {
                Id: firewallId
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onFirewallUpdated = onFirewallUpdated;
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Firewall/FirewallEditor.html', parameters, settings);
        }

        return {
            addFirewall: addFirewall,
            editFirewall: editFirewall
        };
    }

    appControllers.service('NP_IVSwitch_FirewallService', FirewallService);

})(appControllers);