(function (appControllers) {

    "use strict";

    operatorAccountManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'InterConnect_BE_OperatorAccountService'];

    function operatorAccountManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, InterConnect_BE_OperatorAccountService) {
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

            $scope.addNewOperatorAccount = function () {
                var onOperatorAccountAdded = function (operatorAccountObj) {
                    gridAPI.onOperatorAccountAdded(operatorAccountObj);
                };

                InterConnect_BE_OperatorAccountService.addOperatorAccount(onOperatorAccountAdded);
            };
        }


        function getFilterObject() {
            filter = {
                Suffix: $scope.suffix
            };
        }
    }

    appControllers.controller('InterConnect_BE_OperatorAccountManagementController', operatorAccountManagementController);
})(appControllers);