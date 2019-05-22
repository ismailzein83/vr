(function (appControllers) {

    "use strict";

    VRComponentTypeManagementController.$inject = ['$scope', 'VRCommon_VRComponentTypeAPIService', 'VRCommon_VRComponentTypeService', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function VRComponentTypeManagementController($scope,VRCommon_VRComponentTypeAPIService, VRCommon_VRComponentTypeService, UtilsService, VRUIUtilsService, VRNotificationService) {

        var selectedComponentType;

        var gridAPI;

        var componentTypeSelectorAPI;
        var componentTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var devProjectDirectiveApi;
        var devProjectPromiseReadyDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                //gridAPI.load({});
            };
            $scope.scopeModel.onComponentTypeConfigSelectorReady = function (api) {
                componentTypeSelectorAPI = api;
                componentTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onComponentTypeConfigSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    selectedComponentType = selectedItem;
                    loadGrid();
                }
            };

            $scope.scopeModel.search = function () {
                loadGrid();
            };

            $scope.scopeModel.add = function () {
                var onVRObjectTypeDefinitionAdded = function (addedItem) {
                    gridAPI.onVRComponentTypeAdded(addedItem);
                };

                VRCommon_VRComponentTypeService.addVRComponentType(selectedComponentType.ExtensionConfigurationId, onVRObjectTypeDefinitionAdded);
            };

            $scope.scopeModel.hasAddVRComponentTypePermission = function () {
                return VRCommon_VRComponentTypeAPIService.HasAddVRComponentTypePermission();
            };
            $scope.onDevProjectSelectorReady = function (api) {
                devProjectDirectiveApi = api;
                devProjectPromiseReadyDeferred.resolve();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            return UtilsService.waitMultipleAsyncOperations([loadVRComponentTypes, loadDevProjectSelector]).catch(function (error) {
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
        function loadVRComponentTypes() {
            var loadComponentTypesPromiseDeferred = UtilsService.createPromiseDeferred();

            componentTypeSelectorReadyDeferred.promise.then(function () {
                var payloadDirective;
                VRUIUtilsService.callDirectiveLoad(componentTypeSelectorAPI, payloadDirective, loadComponentTypesPromiseDeferred);
            });

            return loadComponentTypesPromiseDeferred.promise;
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
                Name: $scope.scopeModel.name,
                ExtensionConfigId: selectedComponentType != undefined ? selectedComponentType.ExtensionConfigurationId : undefined,
                DevProjectIds: devProjectDirectiveApi.getSelectedIds()
            };
        }
    }

    appControllers.controller('VRCommon_VRComponentTypeManagementController', VRComponentTypeManagementController);

})(appControllers);