(function (appControllers) {

    "use strict";

    VRComponentTypeManagementController.$inject = ['$scope', 'VRCommon_VRComponentTypeAPIService', 'VRCommon_VRComponentTypeService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function VRComponentTypeManagementController($scope, VRCommon_VRComponentTypeAPIService, VRCommon_VRComponentTypeService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var gridAPI;

        var componentTypeSelectorAPI;
        var componentTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();


        function defineScope() {

            $scope.scopeModel = {};
            $scope.scopeModel.selectedComponentType;

            $scope.scopeModel.search = function () {
                var query = buildGridQuery();
                return gridAPI.load(query);
            };

            $scope.scopeModel.onComponentTypeConfigSelectorReady = function (api) {
                componentTypeSelectorAPI = api;
                componentTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.add = function () {
                var onVRObjectTypeDefinitionAdded = function (addedItem) {
                    gridAPI.onVRComponentTypeAdded(addedItem);
                }
                VRCommon_VRComponentTypeService.addVRComponentType($scope.scopeModel.selectedComponentType.ExtensionConfigurationId, onVRObjectTypeDefinitionAdded);
            };

            $scope.scopeModel.hasAddVRComponentTypePermission = function () {
                return VRCommon_VRComponentTypeAPIService.HasAddVRComponentTypePermission();
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            return UtilsService.waitMultipleAsyncOperations([loadVRComponentTypes]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadVRComponentTypes() {
            var loadComponentTypesPromiseDeferred = UtilsService.createPromiseDeferred();
            componentTypeSelectorReadyDeferred.promise.then(function () {
                var payloadDirective;
                VRUIUtilsService.callDirectiveLoad(componentTypeSelectorAPI, payloadDirective, loadComponentTypesPromiseDeferred);
            });
            return loadComponentTypesPromiseDeferred.promise;
        }

        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
            };
        }
    }

    appControllers.controller('VRCommon_VRComponentTypeManagementController', VRComponentTypeManagementController);

})(appControllers);