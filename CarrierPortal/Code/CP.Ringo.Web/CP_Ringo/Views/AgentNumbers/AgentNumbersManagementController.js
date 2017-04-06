(function (appControllers) {

    "use strict";

    AgentNumbersManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'CP_Ringo_AgentNumbersService'];

    function AgentNumbersManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, CP_Ringo_AgentNumbersService) {
        var viewId;

        var invoiceViewerTypeRuntimeEntity;
        var gridAPI;
        loadParameters();
        defineScope();


        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
        }

        function defineScope() {

            $scope.statuses = [];
            $scope.selectedStatuses = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                load();
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
            return gridAPI.loadGrid(getFilterObject()).then(function () {
                $scope.isLoadingFilters = false;
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilters = false;
            });
        }
        function getFilterObject() {
            var filter = {
                Number: $scope.number,
                Status: $scope.selectedStatuses != undefined && $scope.selectedStatuses.length > 0 ? UtilsService.getPropValuesFromArray($scope.selectedStatuses, "value") : undefined
            };
            console.log(filter);
            return filter;
        }
        function loadAllControls() {
            $scope.isLoadingFilters = false;

            $scope.statuses = [
                { value: 0, description: 'Pending' },
                { value: 1, description: 'Accepted' }
            ];
            $scope.selectedStatuses = [
                { value: 0, description: 'Pending' }
            ];

        }
    }

    appControllers.controller('CP_Ringo_AgentNumbersManagementController', AgentNumbersManagementController);
})(appControllers);