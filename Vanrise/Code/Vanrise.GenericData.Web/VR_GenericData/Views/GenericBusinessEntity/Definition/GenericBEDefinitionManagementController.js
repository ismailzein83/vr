(function (appControllers) {
    'use strict';

    GenericBEDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_BusinessEntityDefinitionService', 'VR_GenericData_BusinessEntityDefinitionAPIService','UtilsService'];

    function GenericBEDefinitionManagementController($scope, VR_GenericData_BusinessEntityDefinitionService, businessEntityDefinitionAPIService, UtilsService) {

        var gridAPI;
        var filter = {};

        defineScope();
        load();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.search = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };
            $scope.hasAddGenericBEPermission = function () {
                return businessEntityDefinitionAPIService.HasAddBusinessEntityDefinition();
            };
            $scope.addGenericBE = function () {
                var onBusinessEntityDefinitionAdded = function (onGenericBusinessEntityDefinitionObj) {
                    gridAPI.onGenericBusinessEntityDefinitionAdded(onGenericBusinessEntityDefinitionObj);
                };

                VR_GenericData_BusinessEntityDefinitionService.addBusinessEntityDefinition(onBusinessEntityDefinitionAdded);
            };
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
            function loadAllControls() {
                return UtilsService.waitMultipleAsyncOperations([]).then(function () {
                }).finally(function () {
                    $scope.isLoading = false;
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
        }

        function getFilterObject() {
            filter = {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericBEDefinitionManagementController', GenericBEDefinitionManagementController);

})(appControllers);
