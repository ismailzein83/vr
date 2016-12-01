(function (appControllers) {

    "use strict";

    StatusDefinitionManagementController.$inject = ['$scope', 'Retail_BE_StatusDefinitionService', 'Retail_BE_StatusDefinitionAPIService', 'UtilsService', 'VRUIUtilsService'];

    function StatusDefinitionManagementController($scope, Retail_BE_StatusDefinitionService, Retail_BE_StatusDefinitionAPIService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        var entityTypeAPI;
        var entityTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function defineScope() {
            $scope.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.add = function () {
                var onStatusDefinitionAdded = function (addedStatusDefinition) {
                    gridAPI.onStatusDefinitionAdded(addedStatusDefinition);
                };
                Retail_BE_StatusDefinitionService.addStatusDefinition(onStatusDefinitionAdded);
            };
            $scope.hasAddStatusDefinitionPermission = function () {               
                return Retail_BE_StatusDefinitionAPIService.HasAddStatusDefinitionPermission();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

            $scope.onEntityTypeSelectorReady = function (api) {
                entityTypeAPI = api;
                entityTypeSelectorReadyDeferred.resolve();
            };
        }

        function load(){
            $scope.isloading = true;
            loadAllControls().finally(function () {
                $scope.isloading = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
                $scope.isloading = false;
            })
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadEntityTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }

        function loadEntityTypeSelector() {
            var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            entityTypeSelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(entityTypeAPI, undefined, statusDefinitionSelectorLoadDeferred);
            });
            return statusDefinitionSelectorLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                Name: $scope.name,
                EntityTypes: entityTypeAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_StatusDefinitionManagementController', StatusDefinitionManagementController);
})(appControllers);