(function (app) {

    'use strict';

    RecurchargeEvaluatorPeriodicDirective.$inject = ['UtilsService', 'VRNotificationService','Retail_BE_PeriodicRecurringChargePeriodTypeEnum'];

    function RecurchargeEvaluatorPeriodicDirective(UtilsService, VRNotificationService, Retail_BE_PeriodicRecurringChargePeriodTypeEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new RecurChargePackageSettings($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/MainExtensions/Templates/PeriodicEvaluatorTemplate.html'
        };

        function RecurChargePackageSettings($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var currencySelectorAPI;
            var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.periodicRecurringChargePeriodTypes = UtilsService.getArrayEnum(Retail_BE_PeriodicRecurringChargePeriodTypeEnum);
                $scope.scopeModel.selectedPeriodicRecurringChargePeriodType = Retail_BE_PeriodicRecurringChargePeriodTypeEnum.Monthly;

                $scope.scopeModel.onPeriodTypeSelectionChanged = function(value)
                {
                    if(value != undefined && value.value == Retail_BE_PeriodicRecurringChargePeriodTypeEnum.Days.value)
                    {
                        $scope.scopeModel.showNumberOfDays = true;
                    } else
                    {
                        $scope.scopeModel.showNumberOfDays = false;
                    }
                }

                $scope.scopeModel.onCurrencyDirectiveReady = function (api) {
                    currencySelectorAPI = api;
                    currencySelectorReadyPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([currencySelectorReadyPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {

                    var promises = [];
                    var evaluatorSettings;
                    if (payload != undefined) {
                        evaluatorSettings = payload.evaluatorSettings;
                        if (evaluatorSettings != undefined) {
                            ctrl.price = evaluatorSettings.Price;
                            if (evaluatorSettings.PeriodType != undefined) {
                                $scope.scopeModel.selectedPeriodicRecurringChargePeriodType = UtilsService.getItemByVal($scope.scopeModel.periodicRecurringChargePeriodTypes, evaluatorSettings.PeriodType, "value");
                            }
                            $scope.scopeModel.numberOfDays = evaluatorSettings.NumberOfDays;
                        }
                    }
                    var currencySelectorPayload = evaluatorSettings != undefined ? { selectedIds: evaluatorSettings.CurrencyId } : undefined;
                    promises.push(currencySelectorAPI.load(currencySelectorPayload));
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.RecurringChargeEvaluators.PeriodicRecurringChargeEvaluator, Retail.BusinessEntity.MainExtensions",
                        Price: ctrl.price,
                        CurrencyId: currencySelectorAPI.getSelectedIds(),
                        PeriodType: $scope.scopeModel.selectedPeriodicRecurringChargePeriodType.value,
                        NumberOfDays : $scope.scopeModel.numberOfDays
                    };
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }


        }
    }

    app.directive('retailBePackagesettingsRecurchargeEvaluatorPeriodic', RecurchargeEvaluatorPeriodicDirective);

})(app);