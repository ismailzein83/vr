(function (appControllers) {

    "use strict";

    StatusDefinitionManagementController.$inject = ['$scope', 'Retail_BE_StatusDefinitionService', 'UtilsService', 'VRUIUtilsService'];

    function StatusDefinitionManagementController($scope, Retail_BE_StatusDefinitionService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        var entityTypeAPI;
        var entityTypeAPISelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.add = function () {
                var onStatusDefinitionAdded = function (addedStatusDefinition) {
                    gridAPI.onStatusDefinitionAdded(addedStatusDefinition);
                }
                Retail_BE_StatusDefinitionService.addStatusDefinition(onStatusDefinitionAdded);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.scopeModel.onEntityTypeSelectorReady = function (api) {
                entityTypeAPI = api;
                entityTypeAPISelectorReadyDeferred.resolve();
            }
        }

        function load(){
            $scope.scopeModel.isloading = true;
            loadAllControls().finally(function () {
                $scope.scopeModel.isloading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.scopeModel.isloading = false;
            })
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadEntityTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadEntityTypeSelector() {
            var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            entityTypeAPISelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(entityTypeAPI, undefined, statusDefinitionSelectorLoadDeferred);
            });
            return statusDefinitionSelectorLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                EntityTypes: entityTypeAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_StatusDefinitionManagementController', StatusDefinitionManagementController);
})(appControllers);