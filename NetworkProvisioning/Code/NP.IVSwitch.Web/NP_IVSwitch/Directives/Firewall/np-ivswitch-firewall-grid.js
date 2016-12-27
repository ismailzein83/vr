"use strict";

app.directive("npIvswitchFirewallGrid", ["UtilsService", "VRNotificationService", "NP_IVSwitch_FirewallAPIService", "NP_IVSwitch_FirewallService",
function (utilsService, vrNotificationService, npIvSwitchFirewallApiService, npIvSwitchFirewallService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var switchGrid = new FirewallGrid($scope, ctrl, $attrs);
            switchGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/NP_IVSwitch/Directives/Firewall/Templates/FirewallGridTemplate.html"
    };

    function FirewallGrid($scope, ctrl, $attrs) {
        var gridAPI;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.menuActions = [];
            $scope.firewalls = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return npIvSwitchFirewallApiService.GetFilteredFirewalls(dataRetrievalInput)
                   .then(function (response) {
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
