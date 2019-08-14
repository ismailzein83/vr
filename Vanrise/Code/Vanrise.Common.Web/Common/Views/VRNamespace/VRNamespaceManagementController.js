(function (appControllers) {

    "use strict";

    VRNamespaceManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRCommon_VRNamespaceService', 'VRCommon_VRNamespaceAPIService'];

    function VRNamespaceManagementController($scope, UtilsService, VRUIUtilsService, VRCommon_VRNamespaceService, VRCommon_VRNamespaceAPIService) {

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
                var onVRNamespaceAdded = function (addedVRNamespace) {
                    gridAPI.onVRNamespaceAdded(addedVRNamespace);
                };
                VRCommon_VRNamespaceService.addVRNamespace(onVRNamespaceAdded);
            };

            $scope.hasAddVRNamespacePermission = function () {
                return VRCommon_VRNamespaceAPIService.HasAddVRNamespacePermission();
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
                DevProjectIds: devProjectDirectiveApi.getSelectedIds()

            };
        }
    }

    appControllers.controller('VRCommon_VRNamespaceManagementController', VRNamespaceManagementController);

})(appControllers);