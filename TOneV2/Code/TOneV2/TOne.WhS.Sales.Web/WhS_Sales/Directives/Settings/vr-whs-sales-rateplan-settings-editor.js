'use strict';

app.directive('vrWhsSalesRateplanSettingsEditor', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {

    return {
        restrict: 'E',
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ratePlanSettings = new RatePlanSettings(ctrl, $scope);
            ratePlanSettings.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Sales/Directives/Settings/Templates/RatePlanSettingsEditorTemplate.html'
    };

    function RatePlanSettings(ctrl, $scope) {
        this.initializeController = initializeController;

        var costColumnDirectiveAPI;
        var costColumnDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var periodSelectorAPI;
        var periodSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {

            $scope.scopeModel = {};

            $scope.scopeModel.onCostColumnDirectiveReady = function (api) {
                costColumnDirectiveAPI = api;
                costColumnDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onPeriodSelectorReady = function (api) {
                periodSelectorAPI = api;
                periodSelectorReadyDeferred.resolve();
            };

            UtilsService.waitMultiplePromises([costColumnDirectiveReadyDeferred.promise, periodSelectorReadyDeferred.promise]).then(function () {
                defineAPI();
            });
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var costCalculationMethods;
                var tqiPeriodValue;
                var tqiPeriodType;

                if (payload != undefined && payload.data != null) {
                    $scope.scopeModel.newRateDayOffset = payload.data.NewRateDayOffset;
                    $scope.scopeModel.increasedRateDayOffset = payload.data.IncreasedRateDayOffset;
                    $scope.scopeModel.decreasedRateDayOffset = payload.data.DecreasedRateDayOffset;
                    costCalculationMethods = payload.data.CostCalculationsMethods;
                    tqiPeriodValue = payload.data.TQIPeriodValue;
                    tqiPeriodType = payload.data.TQIPeriodType;
                }

                var promises = [];

                var loadCostColumnDirectivePromise = loadCostColumnDirective();
                promises.push(loadCostColumnDirectivePromise);

                var loadPeriodSelectorPromise = loadPeriodSelector();
                promises.push(loadPeriodSelectorPromise);

                function loadCostColumnDirective() {
                    var costColumnDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    var costColumnDirectivePayload = {
                        costCalculationMethods: costCalculationMethods
                    };
                    VRUIUtilsService.callDirectiveLoad(costColumnDirectiveAPI, costColumnDirectivePayload, costColumnDirectiveLoadDeferred);

                    return costColumnDirectiveLoadDeferred.promise;
                }
                function loadPeriodSelector() {
                    var periodSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    var periodSelectorPayload = {
                        period: {
                            periodValue: tqiPeriodValue,
                            periodType: tqiPeriodType
                        }
                    };
                    VRUIUtilsService.callDirectiveLoad(periodSelectorAPI, periodSelectorPayload, periodSelectorLoadDeferred);

                    return periodSelectorLoadDeferred.promise;
                }

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                var data = {
                    $type: "TOne.WhS.Sales.Entities.RatePlanSettingsData, TOne.WhS.Sales.Entities",
                    NewRateDayOffset: $scope.scopeModel.newRateDayOffset,
                    IncreasedRateDayOffset: $scope.scopeModel.increasedRateDayOffset,
                    DecreasedRateDayOffset: $scope.scopeModel.decreasedRateDayOffset,
                    CostCalculationsMethods: costColumnDirectiveAPI.getData()
                };
                var period = periodSelectorAPI.getData();
                if (period != undefined) {
                    data.TQIPeriodValue = period.periodValue;
                    data.TQIPeriodType = period.periodType;
                }
                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);