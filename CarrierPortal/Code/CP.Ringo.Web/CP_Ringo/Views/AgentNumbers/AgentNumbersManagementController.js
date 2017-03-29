(function (appControllers) {

    "use strict";

    AgentNumbersManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'CP_Ringo_AgentNumbersService'];

    function AgentNumbersManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, CP_Ringo_AgentNumbersService) {
        var viewId;

        var invoiceViewerTypeRuntimeEntity;
        var gridAPI;
        loadParameters();
        defineScope();
        load();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
        }

        function defineScope() {

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.addNumbers = function () {
                var onAgentNumbersAdded = function (addedNumbers) {
                    gridAPI.onAgentNumbersAdded(addedNumbers);
                };

                CP_Ringo_AgentNumbersService.addAgentNumbers(onAgentNumbersAdded);
            };
        }

        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }
        function getFilterObject() {
            var filter = {
            };
            return filter;
        }
        function loadAllControls() {
            $scope.isLoadingFilters = false;
        }
    }

    appControllers.controller('CP_Ringo_AgentNumbersManagementController', AgentNumbersManagementController);
})(appControllers);