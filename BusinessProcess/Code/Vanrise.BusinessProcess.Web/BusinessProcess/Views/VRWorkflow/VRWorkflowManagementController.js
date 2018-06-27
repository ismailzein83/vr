(function (appControllers) {

    "use strict";

    BusinessProcess_VRWorkflowManagementController.$inject = ['$scope', 'BusinessProcess_VRWorkflowService'];

    function BusinessProcess_VRWorkflowManagementController($scope, BusinessProcess_VRWorkflowService) {
        var gridAPI;
        var filter = {};
        defineScope();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(filter);
            };

            $scope.searchClicked = function () {
                getFilterObject();
                return gridAPI.loadGrid(filter);
            };

            $scope.addClicked = function () {
                var onWorkflowAdded = function (addedWorkflow) {
                    gridAPI.onWorkflowAdded(addedWorkflow);
                };
                BusinessProcess_VRWorkflowService.addWorkflow(onWorkflowAdded);
            };
        }

        function getFilterObject() {
            filter.Name = $scope.name;
            filter.Title = $scope.title;
        }
    }

    appControllers.controller('BusinessProcess_VR_WorkflowManagementController', BusinessProcess_VRWorkflowManagementController);
})(appControllers);