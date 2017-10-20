(function (appControllers) {
    "use strict";
    accountManagerManagementController.$inject = ["$scope", "UtilsService", "VRUIUtilsService", "VRNotificationService"];

    function accountManagerManagementController($scope, UtilsService, VRUIUtilsService, VRNotificationServices) {
        var gridAPI;
        var userSelectorAPI;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        loadParameters();
        defineScope();
        load();
        function loadParameters() {

        };
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(getGridFilter());
            };
            $scope.scopeModel.onUserSelectorReady = function (api) {
                userSelectorAPI = api;
                userSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.searchClicked = function () {
                gridAPI.loadGrid(getGridFilter());
            };
        };
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

    
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadUserSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function getGridFilter() {
            var query = {
                UserIds: userSelectorAPI.getSelectedIds()
            };
            return query;
        }
        function loadUserSelector() {
            var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            userSelectorReadyDeferred.promise.then(function () {
                var payload;
                VRUIUtilsService.callDirectiveLoad(userSelectorAPI, payload, userSelectorLoadDeferred);
            });
            return userSelectorLoadDeferred.promise;
        }

    }
    appControllers.controller("VR_AccountManager_AccountManagerManagementController", accountManagerManagementController);
})(appControllers);