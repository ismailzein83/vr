(function (appControllers) {
    "use strict";
    caseManagementController.$inject = ["$scope", "UtilsService", "VRUIUtilsService", "VRNotificationService"];

    function caseManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationService) {
        loadParameters();
        defineScope();
        load();
        function loadParameters() {

        };
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.searchClicked = function () {
                gridAPI.loadGrid(getGridFilter());
            };
        };
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            });
        }
        function getGridFilter() {
            var gridPayload = {
                query: {
                },
                accountManagerDefinitionId: accountManagerDefinitionId
            };
            return gridPayload;
        }
    }
    appControllers.controller("VR_CaseManagement_CaseManagementController", caseManagementController);
})(appControllers);