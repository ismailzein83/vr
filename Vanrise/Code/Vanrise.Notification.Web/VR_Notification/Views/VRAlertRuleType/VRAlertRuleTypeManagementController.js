(function (appControllers) {

    "use strict";

    VRAlertRuleTypeManagementController.$inject = ['$scope', 'VR_Notification_VRAlertRuleTypeService', 'VR_Notification_VRAlertRuleTypeAPIService', 'UtilsService', 'VRUIUtilsService'];

    function VRAlertRuleTypeManagementController($scope, VR_Notification_VRAlertRuleTypeService, VR_Notification_VRAlertRuleTypeAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

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
                };

                VR_Notification_VRAlertRuleTypeService.addVRAlertRuleType(onVRAlertRuleTypeAdded);
            };

            $scope.hasAddVRAlertRuleTypePermission = function () {
                return VR_Notification_VRAlertRuleTypeAPIService.HasAddVRAlertRuleTypePermission();
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitPromiseNode({ promises: [loadDevProjectSelector()] })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.isLoading = false;
                });
        }
        function loadDevProjectSelector() {
            var devProjectPromiseLoadDeferred = UtilsService.createPromiseDeferred();
            devProjectPromiseReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(devProjectDirectiveApi, undefined, devProjectPromiseLoadDeferred);
            });
            return devProjectPromiseLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
                DevProjectIds: devProjectDirectiveApi != undefined ? devProjectDirectiveApi.getSelectedIds() : undefined
            };
        }
    }

    appControllers.controller('VR_Notification_VRAlertRuleTypeManagementController', VRAlertRuleTypeManagementController);

})(appControllers);