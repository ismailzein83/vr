"use strict";

app.directive("vrCdrFraudanalysisExecutestrategyManual", ["VRUIUtilsService", "UtilsService", "StrategyAPIService", "VRValidationService", "CDRAnalysis_FA_PeriodEnum", 'VRDateTimeService',
function (VRUIUtilsService, UtilsService, StrategyAPIService, VRValidationService, CDRAnalysis_FA_PeriodEnum, VRDateTimeService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            }
        },
        templateUrl: "/Client/Modules/FraudAnalysis/Directives/MainExtensions/ProcessInput/Normal/Templates/ExecuteStrategyManualTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        this.initializeController = initializeController;
        var strategySelectorAPI;
        var strategySelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            $scope.toDate = VRDateTimeService.getNowDateTime();
            $scope.fromDate = VRDateTimeService.getNowDateTime();
            $scope.toDateHour = VRDateTimeService.getNowDateTime();
            $scope.fromDateHour = VRDateTimeService.getNowDateTime();
            $scope.showDateHour = false;
            $scope.onStrategySelectorReady = function (api) {
                strategySelectorAPI = api;
                strategySelectorReadyDeferred.resolve();
            };

            $scope.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };

            $scope.validateDateRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDate, $scope.toDate);
            };

            $scope.validateHourRange = function () {
                return VRValidationService.validateTimeRange($scope.fromDateHour, $scope.toDateHour);
            };

            $scope.createProcessInputObjects = [];

            $scope.onPeriodSelectionChanged = function (selectedPeriod) {
                if (selectedPeriod != undefined) {
                    $scope.isLoadingData = true;
                    $scope.showDateHour = (selectedPeriod.Id == CDRAnalysis_FA_PeriodEnum.Hourly.value);
                    $scope.toDate = VRDateTimeService.getNowDateTime();
                    $scope.fromDate = VRDateTimeService.getNowDateTime();
                    $scope.toDateHour = VRDateTimeService.getNowDateTime();
                    $scope.fromDateHour = VRDateTimeService.getNowDateTime();
                    loadStrategySelector(selectedPeriod.Id);
                }
            };


            defineAPI();
        }

        function defineAPI() {
            var api = {};
            api.getData = function () {

                $scope.createProcessInputObjects.length = 0;
                if (periodSelectorAPI.getSelectedIds() == CDRAnalysis_FA_PeriodEnum.Hourly.value) {
                    var runningDate = new Date($scope.fromDateHour);
                    while (runningDate < $scope.toDateHour) {
                        var fromDateHour = new Date(runningDate);
                        var toDateHour = new Date(runningDate.setHours(runningDate.getHours() + 1));
                        createProcessInputObjects(fromDateHour, toDateHour);
                        runningDate = new Date(toDateHour);
                    }
                }

                else if (periodSelectorAPI.getSelectedIds() == CDRAnalysis_FA_PeriodEnum.Daily.value) {
                    var runningDate = new Date($scope.fromDate);
                    while (runningDate < $scope.toDate) {
                        var fromDate = new Date(runningDate);
                        var toDate = new Date(runningDate.setHours(runningDate.getHours() + 24));
                        createProcessInputObjects(fromDate, toDate);
                        runningDate = new Date(toDate);
                    }
                }

                return $scope.createProcessInputObjects;
            };

            api.load = function (payload) {
                var promises = [];
                promises.push(loadPeriodSelector());
                return UtilsService.waitMultiplePromises(promises);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        function createProcessInputObjects(fromDate, toDate) {
            $scope.createProcessInputObjects.push({
                InputArguments: {
                    $type: "Vanrise.Fzero.FraudAnalysis.BP.Arguments.ExecuteStrategyProcessInput, Vanrise.Fzero.FraudAnalysis.BP.Arguments",
                    StrategyIds: strategySelectorAPI.getSelectedIds(),
                    PeriodId: periodSelectorAPI.getSelectedIds(),
                    FromDate: new Date(fromDate),
                    ToDate: new Date(toDate),
                    IncludeWhiteList: $scope.includeWhiteList
                }
            });
        }


        function loadPeriodSelector() {
            var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            periodSelectorReadyDeferred.promise.then(function () {
                var selectorPayload = { selectedIds: CDRAnalysis_FA_PeriodEnum.Daily.value };
                VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, selectorPayload, periodSelectorLoadDeferred);
            });

            return periodSelectorLoadDeferred.promise;
        }


        function loadStrategySelector(periodId) {
            var strategySelectorLoadDeferred = UtilsService.createPromiseDeferred();
            var payload = {
            };
            var filter = {};
            filter.PeriodId = periodId;

            payload.filter = filter;

            strategySelectorReadyDeferred.promise.then(function () {
                VRUIUtilsService.callDirectiveLoad(strategySelectorAPI, payload, strategySelectorLoadDeferred);
            }).finally(function () { $scope.isLoadingData = false; });

            return strategySelectorLoadDeferred.promise;
        }


    }

    return directiveDefinitionObject;
}]);
