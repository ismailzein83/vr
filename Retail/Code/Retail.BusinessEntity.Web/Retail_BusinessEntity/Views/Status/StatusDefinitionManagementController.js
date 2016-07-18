(function (appControllers) {

    "use strict";

    StatusDefinitionManagementController.$inject = ['$scope', 'Retail_BE_StatusDefinitionService', 'UtilsService', 'VRUIUtilsService'];

    function StatusDefinitionManagementController($scope, Retail_BE_StatusDefinitionService, UtilsService, VRUIUtilsService) {

        var gridAPI;

        var entityTypeAPI;
        var entityTypeAPISelectorReadyDeferred = UtilsService.createPromiseDeferred();


        defineScope();
        //loadEntityTypeSelector();


        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };

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

            $scope.scopeModel.statusDefinitionDirectiveReady = function (api) {
                statusDefinitionAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            }

            $scope.scopeModel.onEntityTypeSelectorReady =function (api) {
                entityTypeAPI = api;
                entityTypeAPISelectorReadyDeferred.resolve();
            }
        }

        function loadEntityTypeSelector() {
            var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            entityTypeAPISelectorReadyDeferred.promise.then(function () {
                //var statusDefinitionSelectorPayload = {
                //    selectedIds: convertSupportedOnStatusesFromObj()
                //};
                VRUIUtilsService.callDirectiveLoad(entityTypeAPI, null, statusDefinitionSelectorLoadDeferred);
            });
            return statusDefinitionSelectorLoadDeferred.promise;
        }

        function buildGridQuery() {
            return {
                Guid: null,
                Name: $scope.scopeModel.name,
                Settings: null,
                EntityType: null,
            };
        }
    }

    appControllers.controller('Retail_BE_StatusDefinitionManagementController', StatusDefinitionManagementController);
})(appControllers);