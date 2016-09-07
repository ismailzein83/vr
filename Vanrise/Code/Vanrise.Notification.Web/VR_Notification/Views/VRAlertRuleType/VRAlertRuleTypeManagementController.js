(function (appControllers) {

    "use strict";

    VRAlertRuleTypeManagementController.$inject = ['$scope', 'VR_Notification_VRAlertRuleTypeService', 'VR_Notification_VRAlertRuleTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRAlertRuleTypeManagementController($scope, VR_Notification_VRAlertRuleTypeService, VR_Notification_VRAlertRuleTypeAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        defineScope();
        load();


        function defineScope() {

            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.add = function () {
                var onVRAlertRuleTypeAdded = function (addedVRAlertRuleType) {
                    gridAPI.onVRAlertRuleTypeAdded(addedVRAlertRuleType);
                }

                VR_Notification_VRAlertRuleTypeService.addVRAlertRuleType(onVRAlertRuleTypeAdded);
            };
            //$scope.hasAddVRAlertRuleTypePermission = function () {
            //    return VR_Notification_VRAlertRuleTypeAPIService.HasAddVRAlertRuleTypePermission()
            //}
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

    appControllers.controller('VR_Notification_VRAlertRuleTypeManagementController', VRAlertRuleTypeManagementController);

})(appControllers);