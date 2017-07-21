"use strict";

app.directive("npIvswitchFirewallGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_FirewallAPIService", "NP_IVSwitch_FirewallService", "VRUIUtilsService",
function (utilsService, vrNotificationService, npIvSwitchFirewallApiService, npIvSwitchFirewallService, VRUIUtilsService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var firewallGrid = new FirewallGrid($scope, ctrl, $attrs);
            firewallGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/Firewall/Templates/FirewallGridTemplate.html"
    };

    function FirewallGrid($scope, ctrl, $attrs) {
        var gridAPI;
        var gridDrillDownTabsObj;
        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.menuActions = [];
            $scope.firewalls = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                var drillDownDefinitions = npIvSwitchFirewallService.getDrillDownDefinition();
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                defineAPI();
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return npIvSwitchFirewallApiService.GetFilteredFirewalls(dataRetrievalInput)
                   .then(function (response) {
                       if (response !=undefined && response.Data != undefined) {
                           for (var i = 0; i < response.Data.length; i++) {
                               gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                           }
                       }
                       onResponseReady(response);
                   })
                   .catch(function (error) {
                       vrNotificationService.notifyExceptionWithClose(error, $scope);
                   });
            };
            defineMenuActions();
        }

        function defineAPI() {
            var api = {};

            api.load = function (query) {
                return gridAPI.retrieveData(query);
            };

            api.onFirewallAdded = function (addedFirewall) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(addedFirewall);
                gridAPI.itemAdded(addedFirewall);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function defineMenuActions() {
            $scope.scopeModel.menuActions.push({
                name: 'Edit',
                clicked: editFirewall,
                haspermission: hasEditFirewallPermission
            });
        }
        function editFirewall(firewallItem) {
            var onFirewallUpdated = function (updatedFirewall) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(updatedFirewall);
                gridAPI.itemUpdated(updatedFirewall);
            };
            npIvSwitchFirewallService.editFirewall(firewallItem.Entity.Id, onFirewallUpdated);
        }
        function hasEditFirewallPermission() {
            return npIvSwitchFirewallApiService.HasEditFirewallPermission();
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;

}]);
