(function (appControllers) {

    "use strict";

    BusinessProcess_BPDefinitionManagementController.$inject = ['$scope', 'BusinessProcess_BPDefinitionService', 'BusinessProcess_BPDefinitionAPIService'];

    function BusinessProcess_BPDefinitionManagementController($scope, BusinessProcess_BPDefinitionService, BusinessProcess_BPDefinitionAPIService) {
        var gridAPI;
        defineScope();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid({});
            };

            $scope.add = function () {
                var onBPTechnicalDefinitionAdded = function (addedBPTechnicalDefinition) {
                    gridAPI.onBPTechnicalDefinitionAdded(addedBPTechnicalDefinition);
                };

                BusinessProcess_BPDefinitionService.addBusinessProcessDefinition(onBPTechnicalDefinitionAdded);
            };

            $scope.hasAddBPDefinitionPermission = function () {
                return BusinessProcess_BPDefinitionAPIService.HasAddBPDefinitionPermission()
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
        }

        function getFilterObject() {
            var filter = {};
            filter.Title = $scope.title;
            return filter;
        }
    }

    appControllers.controller('BusinessProcess_BP_TechnicalDefinitionManagementController', BusinessProcess_BPDefinitionManagementController);
})(appControllers);