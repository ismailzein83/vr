(function (appControllers) {

    "use strict";

    accountBalanceNotificationsManagementController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService',  'VRNotificationService'];

    function accountBalanceNotificationsManagementController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var notificationTypeAPI;
        var notificationTypeReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onNotificationTypeSelectorReady = function (api) {
                notificationTypeAPI = api;
                notificationTypeReadyDeferred.resolve();
            };
            $scope.scopeModel.onAccountTypeSelectorSelectionChange = function () {
                if (notificationTypeAPI.getSelectedIds() != undefined) {
                    $scope.scopeModel.gridloadded = false;
                    loadAllControls().then(function () {
                        $scope.scopeModel.gridloadded = true;
                    });
                }
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
            };

            $scope.scopeModel.searchClicked = function () {
                return gridAPI.loadGrid(getFilterObject());
            };
        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadNotificationType();
        }

        function loadNotificationType() {
            var loadNotificationTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            notificationTypeReadyDeferred.promise.then(function () {
                var payLoad;
                VRUIUtilsService.callDirectiveLoad(notificationTypeAPI, payLoad, loadNotificationTypeSelectorPromiseDeferred);
            });
            return loadNotificationTypeSelectorPromiseDeferred.promise.then(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadAllControls() {

        }

        function getFilterObject() {
            var query = {
                NotificationTypeId: notificationTypeAPI.getSelectedIds()
            };
            return query;
        }
    }

    appControllers.controller('VR_AccountBalance_AccountBalanceNotificationsManagementController', accountBalanceNotificationsManagementController);
})(appControllers);