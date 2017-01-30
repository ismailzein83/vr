(function (app) {

    "use strict";

    VRConnectionManagementController.$inject = ['$scope', 'VRCommon_VRConnectionAPIService', 'VRCommon_VRConnectionService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function VRConnectionManagementController($scope, VRCommon_VRConnectionAPIService, VRCommon_VRConnectionService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var connectionTypeAPI;
        var connectionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;


        defineScope();
        load();


        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.search = function () {
                loadGrid(buildGridQuery());
            };

            $scope.scopeModel.onConnectionTypeConfigReady = function (api) {
                connectionTypeAPI = api;
                connectionTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.add = function () {
                var onVRConnectionAdded = function (addedItem) {
                    gridAPI.onVRConnectionAdded(addedItem);
                };
                VRCommon_VRConnectionService.addVRConnection(onVRConnectionAdded);
            };

            $scope.scopeModel.hasAddVRConnectionPermission = function () {
                return VRCommon_VRConnectionAPIService.HasAddVRConnectionPermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load(buildGridQuery());
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadVRConnectionsConfigType]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadGrid() {
            if (gridAPI != undefined) {
                var query = buildGridQuery();
                $scope.scopeModel.isLoading = true;
                return gridAPI.load(query).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            }
        }
        function loadVRConnectionsConfigType() {
            var loadComnnectionConfigTypePromiseDeferred = UtilsService.createPromiseDeferred();
            connectionTypeSelectorReadyDeferred.promise.then(function () {
                var payloadDirective;
                VRUIUtilsService.callDirectiveLoad(connectionTypeAPI, payloadDirective, loadComnnectionConfigTypePromiseDeferred);
            });
            return loadComnnectionConfigTypePromiseDeferred.promise;
        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                ExtensionConfigIds: connectionTypeAPI.getSelectedIds()
            };
        }
    }

    app.controller('VRCommon_VRConnectionManagementController', VRConnectionManagementController);

})(app);