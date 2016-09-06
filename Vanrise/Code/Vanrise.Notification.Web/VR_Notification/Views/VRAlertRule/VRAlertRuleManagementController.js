(function (appControllers) {

    "use strict";

    VRAlertRuleManagementController.$inject = ['$scope', 'VR_Notification_VRAlertRuleService', 'VR_Notification_VRAlertRuleAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRAlertRuleManagementController($scope, VR_Notification_VRAlertRuleService, VR_Notification_VRAlertRuleAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.add = function () {
                var onVRAlertRuleAdded = function (addedVRAlertRule) {
                    gridAPI.onVRAlertRuleAdded(addedVRAlertRule);
                }

                VR_Notification_VRAlertRuleService.addVRAlertRule(onVRAlertRuleAdded);
            };
            $scope.hasAddVRAlertRulePermission = function () {
                return VR_Notification_VRAlertRuleAPIService.HasAddVRAlertRulePermission()
            }
            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {

        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('VR_Notification_VRAlertRuleManagementController', VRAlertRuleManagementController);

})(appControllers);