(function (appControllers) {

    "use strict";

    StrategyManagementController.$inject = ['$scope', 'CDRAnalysis_FA_StrategyService', 'KindEnum', 'StatusEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRValidationService'];

    function StrategyManagementController($scope, CDRAnalysis_FA_StrategyService, KindEnum, StatusEnum, UtilsService, VRUIUtilsService, VRNotificationService, VRValidationService) {
        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var userSelectorAPI;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridQuery = {};

        defineScope();
        load();

        function defineScope() {
            $scope.onUserSelectorReady = function (api) {
                userSelectorAPI = api;
                userSelectorReadyDeferred.resolve();
            };

            $scope.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };

            $scope.onGridReady = function (api) {
                gridAPI = api;
                gridAPI.loadGrid(gridQuery);
            };

            $scope.search = function () {
                setGridQuery();
                return gridAPI.loadGrid(gridQuery);
            };

            $scope.addStrategy = function () {
                var onStrategyAdded = function (addedStrategy) {
                    gridAPI.itemAdded(addedStrategy);
                };
                CDRAnalysis_FA_StrategyService.addStrategy();
            };

            $scope.validateTimeRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };
        }

        function load() {
            $scope.isLoadingFilterSection = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadStaticSelectors, loadUserSelector, loadPeriodSelector]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterSection = false;
            });

            function loadStaticSelectors() {
                $scope.kinds = UtilsService.getArrayEnum(KindEnum);
                $scope.selectedKinds = [];

                $scope.statuses = UtilsService.getArrayEnum(StatusEnum);
                $scope.selectedStatuses = [];
            }

            function loadUserSelector() {
                var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                userSelectorReadyDeferred.promise.then(function () {
                    var userSelectorPayload = null;
                    VRUIUtilsService.callDirectiveLoad(userSelectorAPI, userSelectorPayload, userSelectorLoadDeferred);
                });

                return userSelectorLoadDeferred.promise;
            }

            function loadPeriodSelector() {
                var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                periodSelectorReadyDeferred.promise.then(function () {
                    var periodSelectorPayload = null;
                    VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, periodSelectorPayload, periodSelectorLoadDeferred);
                });

                return periodSelectorLoadDeferred.promise;
            }
        }

        function setGridQuery() {
            gridQuery = {
                Name: $scope.name,
                Description: $scope.description,
                UserIds: userSelectorAPI.getSelectedIds(),
                PeriodIds: periodSelectorAPI.getSelectedIds(),
                Kinds: ($scope.selectedKinds.length > 0) ? UtilsService.getPropValuesFromArray($scope.selectedKinds, "value") : null,
                Statuses: ($scope.selectedStatuses.length > 0) ? UtilsService.getPropValuesFromArray($scope.selectedStatuses, "value") : null,
                FromDate: $scope.fromDate,
                ToDate: $scope.toDate
            };
        }
    }

    appControllers.controller('CDRAnalysis_FA_StrategyManagementController', StrategyManagementController);

})(appControllers);
