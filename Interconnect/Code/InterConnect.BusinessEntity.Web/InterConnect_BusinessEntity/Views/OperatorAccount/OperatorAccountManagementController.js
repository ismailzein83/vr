(function (appControllers) {

    "use strict";

    operatorAccountManagementController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRUIUtilsService', 'InterConnect_BE_OperatorAccountService'];

    function operatorAccountManagementController($scope, UtilsService, VRNotificationService, VRUIUtilsService, InterConnect_BE_OperatorAccountService) {
        var gridAPI;
        var filter = {};
        var operatorProfileDirectiveApi;
        var operatorProfileReadyPromiseDeferred = UtilsService.createPromiseDeferred();
        defineScope();
        load();
        function defineScope() {

            $scope.onOperatorProfileDirectiveReady = function (api) {
                operatorProfileDirectiveApi = api;
                operatorProfileReadyPromiseDeferred.resolve();
            }

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


        function load() {
            $scope.isLoadingFilters = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadOperatorProfiles])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoadingFilters = false;
              });
        }

        function loadOperatorProfiles() {
            var loadOperatorProfilePromiseDeferred = UtilsService.createPromiseDeferred();
            operatorProfileReadyPromiseDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(operatorProfileDirectiveApi, undefined, loadOperatorProfilePromiseDeferred);
            });

            return loadOperatorProfilePromiseDeferred.promise;
        }

        function getFilterObject() {
            console.log($scope.operatorProfiles);
            filter = {
                Suffix: $scope.suffix,
                OperatorProfileIds: operatorProfileDirectiveApi.getSelectedIds(),
            };
        }
    }

    appControllers.controller('InterConnect_BE_OperatorAccountManagementController', operatorAccountManagementController);
})(appControllers);