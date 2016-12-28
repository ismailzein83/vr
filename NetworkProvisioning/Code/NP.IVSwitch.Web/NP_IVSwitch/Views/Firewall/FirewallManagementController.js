(function (appControllers) {

    'use strict';

    FirewallManagementController.$inject = ['$scope', 'VRNotificationService', 'UtilsService', 'VRNavigationService', 'NP_IVSwitch_FirewallService', 'NP_IVSwitch_FirewallAPIService'];

    function FirewallManagementController($scope, utilsService, vruiUtilsService, vrNotificationService, npIvSwitchFirewallService, npIvSwitchFirewallApiService) {

        var gridAPI;

        defineScope();

        function defineScope() {
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.add = function () {
                var onFirewallAdded = function (addedFirewall) {
                    gridAPI.onFirewallAdded(addedFirewall);
                };
                npIvSwitchFirewallService.addFirewall(onFirewallAdded);
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
            $scope.hasAddFirewallPermission = function () {
                return npIvSwitchFirewallApiService.HasAddFirewallPermission();
            };
        }

        function buildGridQuery() {
            return {
                Host: $scope.host,
                Description: $scope.description
            };
        }
    }

    appControllers.controller('NP_IVSwitch_FirewallManagementController', FirewallManagementController);

})(appControllers);