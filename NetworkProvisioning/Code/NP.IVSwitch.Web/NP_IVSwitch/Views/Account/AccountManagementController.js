(function (appControllers) {

    "use strict";

    AccountManagementController.$inject = ['$scope', 'NP_IVSwitch_AccountService', 'NP_IVSwitch_AccountAPIService', 'VRUIUtilsService', 'UtilsService'];
    function AccountManagementController($scope, NP_IVSwitch_AccountService, NP_IVSwitch_AccountAPIService, VRUIUtilsService, UtilsService) {

        var gridAPI;
        var selectorDirectiveAPI;
        var selectorDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
        defineScope();

        load();



        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.search = function () {
                var query = buildGridQuery();

                return gridAPI.load(query);
            };
            $scope.scopeModel.onSelectorDirectiveReady = function (api) {
                selectorDirectiveAPI = api;
                selectorDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.add = function () {
                var onAccountAdded = function (addedAccount) {
                    gridAPI.onAccountAdded(addedAccount);
                }
                NP_IVSwitch_AccountService.addAccount(onAccountAdded);
            };

            $scope.scopeModel.hasAddAccountPermission = function () {
                return NP_IVSwitch_AccountAPIService.HasAddAccountPermission()
            }

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.load({});

            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadSelectorDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function loadSelectorDirective() {
            var selectorDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            selectorDirectiveReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(selectorDirectiveAPI, undefined, selectorDirectiveLoadDeferred);
            });

            return selectorDirectiveLoadDeferred.promise;
        }
        function buildGridQuery() {
            return {
                Name: $scope.scopeModel.name,
                AccountTypes: selectorDirectiveAPI.getSelectedIds(),
            };
        }
    }

    appControllers.controller('NP_IVSwitch_AccountManagementController', AccountManagementController);

})(appControllers);