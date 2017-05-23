(function (app) {

    'use strict';

    RecurchargeEvaluatorOnetimeDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function RecurchargeEvaluatorOnetimeDirective(UtilsService, VRNotificationService) {
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package\PackageRuntime/MainExtensions/PackageTypes/MainExtensions/Templates/OneTimeEvaluatorTemplate.html'
        };

        function RecurChargePackageSettings($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var currencySelectorAPI;
            var currencySelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

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
                        }
                    }
                    var currencySelectorPayload = evaluatorSettings != undefined ? { selectedIds: evaluatorSettings.CurrencyId } : undefined;
                    promises.push(currencySelectorAPI.load(currencySelectorPayload));
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.RecurringChargeEvaluators.OneTimeRecurringChargeEvaluator, Retail.BusinessEntity.MainExtensions",
                        Price: ctrl.price,
                        CurrencyId: currencySelectorAPI.getSelectedIds()
                    };
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }


        }
    }

    app.directive('retailBePackagesettingsRecurchargeEvaluatorOnetime', RecurchargeEvaluatorOnetimeDirective);

})(app);