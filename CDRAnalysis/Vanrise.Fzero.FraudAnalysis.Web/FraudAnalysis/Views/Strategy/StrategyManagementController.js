(function (appControllers) {

    "use strict";

    StrategyManagementController.$inject = ['$scope', 'CDRAnalysis_FA_StrategyService', 'CDRAnalysis_FA_KindEnum', 'CDRAnalysis_FA_StatusEnum', 'UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'VRValidationService', "StrategyAPIService", "PeriodEnum"];

    function StrategyManagementController($scope, CDRAnalysis_FA_StrategyService, CDRAnalysis_FA_KindEnum, CDRAnalysis_FA_StatusEnum, UtilsService, VRUIUtilsService, VRNotificationService, VRValidationService, StrategyAPIService, PeriodEnum) {
        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var userSelectorAPI;
        var userSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var gridAPI;
        var gridQuery = {};

        defineScope();
        load();

        function defineScope() {
            $scope.fromDate = new Date(new Date().setHours(0, 0, 0, 0));
            $scope.toDate;

            $scope.hasAddStrategyPermission = function () {
                return StrategyAPIService.HasAddStrategyPermission();
            };

            $scope.onTimeRangeDirectiveReady = function (api) {
                timeRangeDirectiveAPI = api;
                timeRangeDirectiveReadyDeferred.resolve();
            };

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
                setGridQuery();
                gridAPI.loadGrid(gridQuery);
            };

            $scope.search = function () {
                setGridQuery();
                return gridAPI.loadGrid(gridQuery);
            };

            $scope.addStrategy = function () {
                var onStrategyAdded = function (addedStrategy) {
                    gridAPI.onStrategyAdded(addedStrategy);
                };
                CDRAnalysis_FA_StrategyService.addStrategy(onStrategyAdded);
            };

        }

        function load() {
            $scope.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([loadStaticSelectors, loadUserSelector, loadPeriodSelector]).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });

            function loadStaticSelectors() {
                $scope.kinds = UtilsService.getArrayEnum(CDRAnalysis_FA_KindEnum);
                $scope.selectedKinds = [];

                $scope.statuses = UtilsService.getArrayEnum(CDRAnalysis_FA_StatusEnum);
                $scope.selectedStatuses = [];
            }

            function loadUserSelector() {
                var userSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                userSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(userSelectorAPI, undefined, userSelectorLoadDeferred);
                });

                return userSelectorLoadDeferred.promise;
            }

            function loadPeriodSelector() {
                var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                periodSelectorReadyDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, undefined, periodSelectorLoadDeferred);
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
