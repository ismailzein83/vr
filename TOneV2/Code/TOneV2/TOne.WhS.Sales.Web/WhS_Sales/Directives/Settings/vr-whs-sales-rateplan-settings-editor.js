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
            $scope.scopeModel.hintText = "Rate's BED of the subscribers can either follow the same BED of their publisher, or can be calculated according to the system parameters for increased and decreased rates.";

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
                    costCalculationMethods = payload.data.CostCalculationsMethods;
                    tqiPeriodValue = payload.data.TQIPeriodValue;
                    tqiPeriodType = payload.data.TQIPeriodType;
                    $scope.scopeModel.followPublisherRatesBED = payload.data.FollowPublisherRatesBED;
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
                    CostCalculationsMethods: costColumnDirectiveAPI.getData()
                };
                var period = periodSelectorAPI.getData();
                if (period != undefined) {
                    data.TQIPeriodValue = period.periodValue;
                    data.TQIPeriodType = period.periodType;
                }
                data.FollowPublisherRatesBED = $scope.scopeModel.followPublisherRatesBED;
                return data;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);