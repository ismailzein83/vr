(function (appControllers) {
    'use strict';

    GenericBusinessEntityDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_GenericEditorService'];

    function GenericBusinessEntityDefinitionManagementController($scope, VR_GenericData_GenericEditorService) {

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

            $scope.addGenericBusinessEntityDefinition = function () {
                var onGenericBusinessEntityDefinitionAdded = function (onGenericBusinessEntityDefinitionObj) {
                    gridAPI.onGenericBusinessEntityDefinitionAdded(onGenericBusinessEntityDefinitionObj);
                };

                VR_GenericData_GenericEditorService.addGenericBusinessEntityDefinition(onGenericBusinessEntityDefinitionAdded);
            };
        }

        function load() {

        }

        function getFilterObject() {
            filter = {
                Name: $scope.name,
            };
        }
    }

    appControllers.controller('VR_GenericData_GenericBusinessEntityDefinitionManagementController', GenericBusinessEntityDefinitionManagementController);

})(appControllers);
