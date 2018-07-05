(function (appControllers) {

    "use strict";

    BusinessProcess_VRWorkflowManagementController.$inject = ['$scope', 'BusinessProcess_VRWorkflowService', 'BusinessProcess_VRWorkflowAPIService'];

    function BusinessProcess_VRWorkflowManagementController($scope, BusinessProcess_VRWorkflowService, BusinessProcess_VRWorkflowAPIService) {


        var gridAPI;

        defineScope();

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getFilterObject());
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.addClicked = function () {
                var onVRWorkflowAdded = function (addedVRWorkflow) {
                    gridAPI.onVRWorkflowAdded(addedVRWorkflow);
                };

                BusinessProcess_VRWorkflowService.addVRWorkflow(onVRWorkflowAdded);
            };

            $scope.hasAddVRWorkflowPermission = function () {
                return BusinessProcess_VRWorkflowAPIService.HasAddVRWorkflowPermission();
            };
        }

        function getFilterObject() {
            var filter = {};
            filter.Name = $scope.name;
            filter.Title = $scope.title;
            return filter;
        }
    }

    appControllers.controller('BusinessProcess_VR_WorkflowManagementController', BusinessProcess_VRWorkflowManagementController);
})(appControllers);