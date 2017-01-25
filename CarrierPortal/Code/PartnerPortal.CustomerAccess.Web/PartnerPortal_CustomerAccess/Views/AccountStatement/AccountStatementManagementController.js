(function (appControllers) {

    "use strict";

    accountStatementManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'PartnerPortal_CustomerAccess_AccountStatementAPIService', 'VRNotificationService'];

    function accountStatementManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, PartnerPortal_CustomerAccess_AccountStatementAPIService, VRNotificationService) {
        var viewId;

        var gridAPI;
        defineScope();
        loadParameters();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                viewId = parameters.viewId;
            }
        };

        function defineScope() {
            $scope.scopeModel = {};
            var date = new Date();

            $scope.scopeModel.fromDate = new Date(date.getFullYear(), date.getMonth() - 1, 1, 0, 0, 0, 0);

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
        };


        function getFilterObject() {
            var query = {
                FromDate: $scope.scopeModel.fromDate,
                ViewId: viewId
            };
            return query;
        };
    }

    appControllers.controller('PartnerPortal_CustomerAccess_AccountStatementManagementController', accountStatementManagementController);
})(appControllers);