(function (appControllers) {

    "use strict";

    operatorAccountManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'Demo_OperatorAccountTypeEnum', 'VRUIUtilsService', 'Demo_OperatorAccountService'];

    function operatorAccountManagementController($scope, UtilsService, VRNotificationService, Demo_OperatorAccountTypeEnum, VRUIUtilsService, Demo_OperatorAccountService) {
        var gridAPI;
        var operatorProfileDirectiveAPI;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        defineScope();
        load();

        function defineScope() {
            $scope.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                var filter = {};
                api.loadGrid(filter);
            }

            $scope.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveAPI = api;
                operatorProfileReadyPromiseDeferred.resolve();
            }

            $scope.selectedOperatorAccountTypes = [];
            $scope.AddNewOperatorAccount = AddNewOperatorAccount;

            function getFilterObject() {
                var data = {
                    AccountsTypes: UtilsService.getPropValuesFromArray($scope.selectedOperatorAccountTypes, "value"),
                    OperatorProfilesIds: operatorProfileDirectiveAPI.getSelectedIds(),
                    Name: $scope.name
                };
                return data;
            }
        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOperatorAccountType, loadOperatorProfiles])
                .catch(function (error) {
                    $scope.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
               .finally(function () {
                   $scope.isLoading = false;
               });
        }

        function loadOperatorAccountType() {
            $scope.operatorAccountTypes = UtilsService.getArrayEnum(Demo_OperatorAccountTypeEnum);
        }

        function loadOperatorProfiles() {
            var loadOperatorProfilePromiseDeferred = UtilsService.createPromiseDeferred();

            operatorProfileReadyPromiseDeferred.promise
                .then(function () {
                    var directivePayload = undefined;

                    VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveAPI, directivePayload, loadOperatorProfilePromiseDeferred);
                });

            return loadOperatorProfilePromiseDeferred.promise;
        }

        function AddNewOperatorAccount() {
            var onOperatorAccountAdded = function (operatorAccountObj) {
                gridAPI.onOperatorAccountAdded(operatorAccountObj);
            };

            Demo_OperatorAccountService.addOperatorAccount(onOperatorAccountAdded);
        }
    }

    appControllers.controller('Demo_OperatorAccountManagementController', operatorAccountManagementController);
})(appControllers);