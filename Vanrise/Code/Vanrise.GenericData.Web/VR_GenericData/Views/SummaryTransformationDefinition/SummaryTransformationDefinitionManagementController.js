(function (appControllers) {
    'use strict';

    SummaryTransformationDefinitionManagementController.$inject = ['$scope', 'VR_GenericData_SummaryTransformationDefinitionService', 'VR_GenericData_SummaryTransformationDefinitionAPIService'];

    function SummaryTransformationDefinitionManagementController($scope, VR_GenericData_SummaryTransformationDefinitionService, summaryTransformationDefinitionAPIService) {

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
            $scope.hasAddSummaryTransformationDefinition = function () {
                return summaryTransformationDefinitionAPIService.HasAddSummaryTransformationDefinition();
            };
            $scope.addSummaryTransformationDefinition = function () {
                var onSummaryTransformationDefinitionAdded = function (onSummaryTransformationDefinitionObj) {
                    gridAPI.onSummaryTransformationDefinitionAdded(onSummaryTransformationDefinitionObj);
                };

                VR_GenericData_SummaryTransformationDefinitionService.addSummaryTransformationDefinition(onSummaryTransformationDefinitionAdded);
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

    appControllers.controller('VR_GenericData_SummaryTransformationDefinitionManagementController', SummaryTransformationDefinitionManagementController);

})(appControllers);
