(function (appControllers) {

    "use strict";

    StatusChargingSetManagementController.$inject = ['$scope', 'Retail_BE_StatusChargingSetService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function StatusChargingSetManagementController($scope, retailBeStatusChargingSetService, utilsService, vruiUtilsService, vrNotificationService) {

        var gridAPI;

        var entityTypeAPI;
        var entityTypeSelectorReadyDeferred = utilsService.createPromiseDeferred();

        defineScope();
        load();

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name//,
                //EntityTypes: entityTypeAPI.getSelectedIds()
            };
        }

        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };
            $scope.scopeModel.add = function () {
                var onStatusChargingSetAdded = function (addedStatusChargingSet) {
                    gridAPI.onStatusChargingSetAdded(addedStatusChargingSet);
                };
                retailBeStatusChargingSetService.addStatusChargingSet(onStatusChargingSetAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.onEntityTypeSelectorReady = function (api) {
                entityTypeAPI = api;
                entityTypeSelectorReadyDeferred.resolve();
            };
        }

        function loadEntityTypeSelector() {
            var statusDefinitionSelectorLoadDeferred = utilsService.createPromiseDeferred();
            entityTypeSelectorReadyDeferred.promise.then(function () {
                vruiUtilsService.callDirectiveLoad(entityTypeAPI, undefined, statusDefinitionSelectorLoadDeferred);
            });
            return statusDefinitionSelectorLoadDeferred.promise;
        }

        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([loadEntityTypeSelector]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function load() {
            $scope.scopeModel.isloading = true;
            loadAllControls().finally(function () {
                $scope.scopeModel.isloading = false;
            }).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isloading = false;
            });
        }
    }

    appControllers.controller('Retail_BE_StatusChargingSetManagementController', StatusChargingSetManagementController);
})(appControllers);