"use strict";

app.directive("whsBeFinancialaccountbedefinitionEditor", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var genericBusinessEntityDefinitionEditor = new FinancialAccountBEDefinitionEditor($scope, ctrl, $attrs);
                genericBusinessEntityDefinitionEditor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountBEDefinition.html"

        };

        function FinancialAccountBEDefinitionEditor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            var invoiceTypeSelectorAPI;
            var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var balanceAccountTypeSelectorAPI;
            var balanceAccountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var extendedSettingsDirectiveAPI;
            var extendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                    invoiceTypeSelectorAPI = api;
                    invoiceTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onBalanceAccountTypeSelectorReady = function (api) {
                    balanceAccountTypeSelectorAPI = api;
                    balanceAccountTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onExtendedSettingsDirectiveReady = function (api) {
                    extendedSettingsDirectiveAPI = api;
                    extendedSettingsDirectiveReadyDeferred.resolve();
                };
                UtilsService.waitMultiplePromises([invoiceTypeSelectorReadyDeferred.promise, balanceAccountTypeSelectorReadyDeferred.promise, extendedSettingsDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var beDefinitionSettings

                    if (payload != undefined && payload.businessEntityDefinitionSettings != undefined) {
                         beDefinitionSettings = payload.businessEntityDefinitionSettings;
                    }

                    function loadInvoiceTypeSelector() {
                        var invoiceTypeSelectorPayload;
                        if (beDefinitionSettings != undefined) {
                            invoiceTypeSelectorPayload = { selectedIds: beDefinitionSettings.InvoiceTypeId };
                        }
                        return invoiceTypeSelectorAPI.load(invoiceTypeSelectorPayload);
                    }
                    promises.push(loadInvoiceTypeSelector());

                    function loadBalanceAccountTypeSelector() {
                        var balanceAccountTypeSelectorPayload;
                        if (beDefinitionSettings != undefined) {
                            balanceAccountTypeSelectorPayload = { selectedIds: beDefinitionSettings.BalanceAccountTypeId };
                        }
                        return balanceAccountTypeSelectorAPI.load(balanceAccountTypeSelectorPayload);
                    }
                    promises.push(loadBalanceAccountTypeSelector());

                    function loadExtendedSettingsDirective() {
                        var extendedSettingsDirectivePayload;
                        if (beDefinitionSettings != undefined) {
                            extendedSettingsDirectivePayload = { extendedSettings: beDefinitionSettings.ExtendedSettings };
                        }
                        return extendedSettingsDirectiveAPI.load(extendedSettingsDirectivePayload);
                    }
                    promises.push(loadExtendedSettingsDirective());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Business.WHSFinancialAccountDefinitionSettings, TOne.WhS.BusinessEntity.Business",
                        InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                        BalanceAccountTypeId: balanceAccountTypeSelectorAPI.getSelectedIds(),
                        ExtendedSettings: extendedSettingsDirectiveAPI.getData()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);