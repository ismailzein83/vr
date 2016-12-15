(function (app) {

    "use strict";

    billingTransactionManagementController.$inject = ['$scope', 'VRNavigationService'];

    function billingTransactionManagementController($scope, VRNavigationService) {
        var gridAPI;
        accountTypeId;
        loadParameters();
        defineScope();
        load();

        var filter = {};
        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != null) {
                accountTypeId = parameters.accountTypeId;
            }
        }
        
        function defineScope() {           
            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid(getFilterObject());
            };
        }

        function load() {
        }

        function getFilterObject() {
            return {
                FromTime: $scope.fromTime,
                ToTime: $scope.toTime,
                AccountTypeId: accountTypeId
            };
        }
    }

    app.controller('VR_AccountBalance_BillingTransactionManagementController', billingTransactionManagementController);
})(app);