﻿'use strict';

app.directive('retailBeFinancialaccountdefinitionSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new financialAccountDefinitionSettingsnCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/FinancialAccount/Templates/FinancialAccountDefinitionSettings.html"
        };

        function financialAccountDefinitionSettingsnCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var invoiceTypeSelectorAPI;
            var invoiceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var balanceAccountTypeSelectorAPI;
            var balanceAccountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var extendedSettingsDirectiveAPI;
            var extendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var businessEntityDefinitionSelectorAPI;
            var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onInvoiceTypeSelectorReady = function (api) {
                    invoiceTypeSelectorAPI = api;
                    invoiceTypeSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                    businessEntityDefinitionSelectorAPI = api;
                    businessEntityDefinitionSelectorReadyDeferred.resolve();
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
                    var financialAccountDefinitionSettings;
                    if (payload != undefined) {
                        financialAccountDefinitionSettings = payload.componentType;

                        if (financialAccountDefinitionSettings != undefined) {
                            $scope.scopeModel.name = financialAccountDefinitionSettings.Name;
                        }
                    }

                    var promises = [];

                    var businessEntityDefinitionSelectorLoadPromise = loadBusinessEntityDefinitionSelector();
                    promises.push(businessEntityDefinitionSelectorLoadPromise);

                    function loadBusinessEntityDefinitionSelector() {
                        var payloadSelector = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business",
                                }]
                            },
                            selectedIds: financialAccountDefinitionSettings != undefined && financialAccountDefinitionSettings.Settings != undefined ? financialAccountDefinitionSettings.Settings.AccountBEDefinitionId : undefined
                        };
                        return businessEntityDefinitionSelectorAPI.load(payloadSelector);
                    }

                    function loadInvoiceTypeSelector() {
                        var invoiceTypeSelectorPayload;
                        if (financialAccountDefinitionSettings != undefined && financialAccountDefinitionSettings.Settings != undefined) {
                            invoiceTypeSelectorPayload = { selectedIds: financialAccountDefinitionSettings.Settings.InvoiceTypeId };
                        }
                        return invoiceTypeSelectorAPI.load(invoiceTypeSelectorPayload);
                    }
                    promises.push(loadInvoiceTypeSelector());

                    function loadBalanceAccountTypeSelector() {
                        var balanceAccountTypeSelectorPayload;
                        if (financialAccountDefinitionSettings != undefined && financialAccountDefinitionSettings.Settings != undefined) {
                            balanceAccountTypeSelectorPayload = { selectedIds: financialAccountDefinitionSettings.Settings.BalanceAccountTypeId };
                        }
                        return balanceAccountTypeSelectorAPI.load(balanceAccountTypeSelectorPayload);
                    }
                    promises.push(loadBalanceAccountTypeSelector());

                    function loadExtendedSettingsDirective() {
                        var extendedSettingsDirectivePayload;
                        if (financialAccountDefinitionSettings != undefined && financialAccountDefinitionSettings.Settings != undefined) {
                            extendedSettingsDirectivePayload = { extendedSettings: financialAccountDefinitionSettings.Settings.ExtendedSettings };
                        }
                        return extendedSettingsDirectiveAPI.load(extendedSettingsDirectivePayload);
                    }
                    promises.push(loadExtendedSettingsDirective());

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Retail.BusinessEntity.Entities.FinancialAccountDefinitionSettings, Retail.BusinessEntity.Entities",
                            InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                            BalanceAccountTypeId: balanceAccountTypeSelectorAPI.getSelectedIds(),
                            AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                            ExtendedSettings: extendedSettingsDirectiveAPI.getData()
                        }
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);