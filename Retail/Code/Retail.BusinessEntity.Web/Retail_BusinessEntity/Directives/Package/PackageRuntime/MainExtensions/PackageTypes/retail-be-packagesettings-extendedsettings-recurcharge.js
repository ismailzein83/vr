(function (app) {

    'use strict';

    RecurChargePackageSettingsDirective.$inject = ['UtilsService', 'VRNotificationService', 'Retail_BE_PricingPackageSettingsService','VRUIUtilsService'];

    function RecurChargePackageSettingsDirective(UtilsService, VRNotificationService, Retail_BE_PricingPackageSettingsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceRecurChargePackageSettings($scope, ctrl);
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
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageRuntime/MainExtensions/PackageTypes/Templates/RecurChargePackageSettingsTemplate.html'
        };

        function InvoiceRecurChargePackageSettings($scope, ctrl) {
            this.initializeController = initializeController;

            var gridAPI;
            var recurringChargeEvaluatorAPI;
            var recurringChargeEvaluatorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onRecurringChargeEvaluatorDirectiveReady = function (api) {
                    recurringChargeEvaluatorAPI = api;
                    recurringChargeEvaluatorReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }
            function defineAPI() {

                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var extendedSettings;
                    var extendedSettingsDefinition;
                    if (payload != undefined) {
                        extendedSettings = payload.extendedSettings;
                        extendedSettingsDefinition = payload.extendedSettingsDefinition;
                        if (extendedSettingsDefinition != undefined && extendedSettingsDefinition.EvaluatorDefinitionSettings != undefined)
                            $scope.scopeModel.runtimeEditor = extendedSettingsDefinition.EvaluatorDefinitionSettings.RuntimeEditor;
                    }

                    var loadRecurringChargeEvaluatorPromiseDeferred = UtilsService.createPromiseDeferred();
                    recurringChargeEvaluatorReadyPromiseDeferred.promise.then(function () {
                        var recurringChargeEvaluatorPayload = extendedSettings != undefined ? { evaluatorSettings: extendedSettings.Evaluator } : undefined;
                        VRUIUtilsService.callDirectiveLoad(recurringChargeEvaluatorAPI, recurringChargeEvaluatorPayload, loadRecurringChargeEvaluatorPromiseDeferred);
                    });
                    promises.push(loadRecurringChargeEvaluatorPromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "Retail.BusinessEntity.MainExtensions.PackageTypes.RecurChargePackageSettings, Retail.BusinessEntity.MainExtensions",
                        Evaluator: recurringChargeEvaluatorAPI.getData()
                    };
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

        
        }
    }

    app.directive('retailBePackagesettingsExtendedsettingsRecurcharge', RecurChargePackageSettingsDirective);

})(app);