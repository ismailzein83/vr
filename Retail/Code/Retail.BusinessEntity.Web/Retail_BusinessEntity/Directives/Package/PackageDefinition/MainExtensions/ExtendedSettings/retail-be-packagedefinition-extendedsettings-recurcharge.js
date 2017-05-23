'use strict';

app.directive('retailBePackagedefinitionExtendedsettingsRecurcharge', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RecurchargePackageDefinitionSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Package/PackageDefinition/MainExtensions/ExtendedSettings/Templates/RecurchargePackageDefinitionSettingsTemplate.html'
        };

        function RecurchargePackageDefinitionSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var invoiceRecurChargeEvaluatorApi;
            var invoiceRecurChargeEvaluatorPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onInvoiceRecurChargeEvaluatorReady = function (api) {
                    invoiceRecurChargeEvaluatorApi = api;
                    invoiceRecurChargeEvaluatorPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([invoiceRecurChargeEvaluatorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var extendedSettings;
                    if (payload != undefined)
                    {
                        extendedSettings = payload.extendedSettings;
                    }
                    promises.push(loadInvoiceRecurChargeEvaluator());
                    function loadInvoiceRecurChargeEvaluator() {
                        var payloadInvoiceRecurChargeEvaluator = {
                            evaluatorDefinitionSettings: extendedSettings != undefined ? extendedSettings.EvaluatorDefinitionSettings : undefined,
                        };
                        return invoiceRecurChargeEvaluatorApi.load(payloadInvoiceRecurChargeEvaluator);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: 'Retail.BusinessEntity.MainExtensions.PackageTypes.RecurChargePackageDefinitionSettings, Retail.BusinessEntity.MainExtensions',
                        EvaluatorDefinitionSettings: invoiceRecurChargeEvaluatorApi.getData(),
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);