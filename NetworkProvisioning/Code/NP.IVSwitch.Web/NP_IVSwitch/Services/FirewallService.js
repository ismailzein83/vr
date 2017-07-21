
(function (appControllers) {

    "use strict";

    FirewallService.$inject = ['VRModalService', 'VRCommon_ObjectTrackingService', 'UtilsService'];

    function FirewallService(NPModalService, VRCommon_ObjectTrackingService, UtilsService) {
        var drillDownDefinitions = [];
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


        function viewHistoryFirewall(context) {
            var modalParameters = {
                context: context
            };
            var modalSettings = {
            };
            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);
            };
            NPModalService.showModal('/Client/Modules/NP_IVSwitch/Views/Firewall/FirewallEditor.html', modalParameters, modalSettings);
        };

        function registerHistoryViewAction() {

            var actionHistory = {
                actionHistoryName: "NP_IVSwitch_Firewall_ViewHistoryItem",
                actionMethod: function (payload) {

                    var context = {
                        historyId: payload.historyId
                    };

                    viewHistoryFirewall(context);
                }
            };
            VRCommon_ObjectTrackingService.registerActionHistory(actionHistory);
        }


        function getEntityUniqueName() {
            return "NP_IVSwitch_Firewall";
        }

        function registerObjectTrackingDrillDownToFirewall() {
            var drillDownDefinition = {};

            drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
            drillDownDefinition.directive = "vr-common-objecttracking-grid";


            drillDownDefinition.loadDirective = function (directiveAPI, fireWallItem) {

                fireWallItem.objectTrackingGridAPI = directiveAPI;
                var query = {
                    ObjectId: fireWallItem.Entity.Id,
                    EntityUniqueName: getEntityUniqueName(),

                };
                return fireWallItem.objectTrackingGridAPI.load(query);
            };


            addDrillDownDefinition(drillDownDefinition);

        }
        function addDrillDownDefinition(drillDownDefinition) {
            drillDownDefinitions.push(drillDownDefinition);
        }

        function getDrillDownDefinition() {
            return drillDownDefinitions;
        }

        return {
            addFirewall: addFirewall,
            editFirewall: editFirewall,
            getDrillDownDefinition: getDrillDownDefinition,
            registerObjectTrackingDrillDownToFirewall: registerObjectTrackingDrillDownToFirewall,
            registerHistoryViewAction: registerHistoryViewAction
        };
    }

    appControllers.service('NP_IVSwitch_FirewallService', FirewallService);

})(appControllers);