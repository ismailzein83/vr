﻿'use strict';

app.directive('retailBeAccountbedefinitionsEditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBeDefinitionsSettingsEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountDefinition/Templates/AccountBEDefinitionEditorTemplate.html'
        };

        function AccountBeDefinitionsSettingsEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var statusBEDefinitionSelectorAPI;
            var statusBEDefinitionSelectorDeferred = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorDeferred = UtilsService.createPromiseDeferred();

            var financialAccountLocatorAPI;
            var financialAccountLocatorDeferred = UtilsService.createPromiseDeferred();

            var accountGridDefinitionDirectiveAPI;
            var accountGridDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountGridDefinitionExportExcelDirectiveAPI;
            var accountGridDefinitionExportExcelDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountViewDefinitionDirectiveAPI;
            var accountViewDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountActionDefinitionDirectiveAPI;
            var accountActionDefinitionDirectiveDeferred = UtilsService.createPromiseDeferred();

            var accountExtraFieldDefinitionsDirectiveAPI;
            var accountExtraFieldDefinitionsDirectiveDeferred = UtilsService.createPromiseDeferred();

            var securityDefinitionsDirectiveAPI;
            var securityDefinitionsDirectiveDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                    statusBEDefinitionSelectorAPI = api;
                    statusBEDefinitionSelectorDeferred.resolve();
                };

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorDeferred.resolve();
                };

                $scope.scopeModel.onFinancialAccountLocatorReady = function (api) {
                    financialAccountLocatorAPI = api;
                    financialAccountLocatorDeferred.resolve();
                };

                $scope.scopeModel.onAccountGridDefinitionReady = function (api) {
                    accountGridDefinitionDirectiveAPI = api;
                    accountGridDefinitionDirectiveDeferred.resolve();
                };

                $scope.scopeModel.onAccountGridDefinitionExportExcelReady = function (api) {
                    accountGridDefinitionExportExcelDirectiveAPI = api;
                    accountGridDefinitionExportExcelDirectiveDeferred.resolve();
                };

                $scope.scopeModel.onAccountViewDefinitionsReady = function (api) {
                    accountViewDefinitionDirectiveAPI = api;
                    accountViewDefinitionDirectiveDeferred.resolve();
                };

                $scope.scopeModel.onAccountActionDefinitionsReady = function (api) {
                    accountActionDefinitionDirectiveAPI = api;
                    accountActionDefinitionDirectiveDeferred.resolve();
                };

                $scope.scopeModel.onAccountExtraFieldDefinitionsReady = function (api) {
                    accountExtraFieldDefinitionsDirectiveAPI = api;
                    accountExtraFieldDefinitionsDirectiveDeferred.resolve();
                };

                $scope.scopeModel.onAccountSecurityDefinitionsReady = function (api) {
                    securityDefinitionsDirectiveAPI = api;
                    securityDefinitionsDirectiveDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([statusBEDefinitionSelectorDeferred.promise, accountTypeSelectorDeferred.promise, financialAccountLocatorDeferred.promise,
                    accountGridDefinitionDirectiveDeferred.promise, accountViewDefinitionDirectiveDeferred.promise, accountActionDefinitionDirectiveDeferred.promise,
                    accountGridDefinitionExportExcelDirectiveDeferred.promise]).then(function () {
                        defineAPI();
                    });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBEDefinitionId;
                    var statusBEDefinitionId;
                    var localServiceAccountTypeId;
                    var financialAccountLocator;
                    var accountGridDefinition;
                    var accountGridDefinitionExportExcel;
                    var accountViewDefinitions;
                    var accountActionDefinitions;
                    var accountExtraFieldDefinitions;
                    var securityDefinition;

                    if (payload != undefined) {
                        accountBEDefinitionId = payload.businessEntityDefinitionId;

                        if (payload.businessEntityDefinitionSettings != undefined) {
                            statusBEDefinitionId = payload.businessEntityDefinitionSettings.StatusBEDefinitionId;
                            localServiceAccountTypeId = payload.businessEntityDefinitionSettings.LocalServiceAccountTypeId;
                            financialAccountLocator = payload.businessEntityDefinitionSettings.FinancialAccountLocator;
                            accountGridDefinition = payload.businessEntityDefinitionSettings.GridDefinition.ColumnDefinitions;
                            accountGridDefinitionExportExcel = payload.businessEntityDefinitionSettings.GridDefinition.ExportColumnDefinitions;
                            accountViewDefinitions = payload.businessEntityDefinitionSettings.AccountViewDefinitions;
                            accountActionDefinitions = payload.businessEntityDefinitionSettings.ActionDefinitions;
                            accountExtraFieldDefinitions = payload.businessEntityDefinitionSettings.AccountExtraFieldDefinitions;
                            securityDefinition = payload.businessEntityDefinitionSettings.Security;

                            $scope.scopeModel.useRemoteSelector = payload.businessEntityDefinitionSettings.UseRemoteSelector;
                        }
                    }

                    //Loading Status Definition Directive
                    var statusBEDefinitionSelectorLoadPromise = getStatusDefinitionSelectorLoadPromise();
                    promises.push(statusBEDefinitionSelectorLoadPromise);

                    //Loading AccountType Selector
                    if (accountBEDefinitionId != undefined) {
                        var accountTypeSelectorLoadPromise = getAccountTypeSelectorLoadPromise();
                        promises.push(accountTypeSelectorLoadPromise);
                    }

                    //Loading FinancialAccountLocator Directive
                    var financialAccountLocatorLoadPromise = getFinancialAccountLocatorLoadPromise();
                    promises.push(financialAccountLocatorLoadPromise);

                    //Loading AccountGridDefinition Directive
                    var accountGridDefinitionLoadPromise = getAccountGridDefinitionLoadPromise();
                    promises.push(accountGridDefinitionLoadPromise);

                    //Loading AccountGridDefinitionExportExcel Directive
                    var accountGridDefinitionExportExcelLoadPromise = getAccountGridDefinitionExportExcelLoadPromise();
                    promises.push(accountGridDefinitionExportExcelLoadPromise);

                    //Loading AccountViewDefinition Directive
                    var accountViewDefinitionLoadPromise = getAccountViewDefinitionLoadPromise();
                    promises.push(accountViewDefinitionLoadPromise);

                    //Loading AccountActionDefinition Directive
                    var accountActionDefinitionLoadPromise = getAccountActionDefinitionLoadPromise();
                    promises.push(accountActionDefinitionLoadPromise);

                    //Loading AccountExtraFieldDefinition Directive
                    var accountExtraFieldDefinitionsLoadPromise = getAccountExtraFieldDefinitionsLoadPromise();
                    promises.push(accountExtraFieldDefinitionsLoadPromise);

                    //Loading AccountSecurityDefinition Directive
                    var accountSecurityDefinitionsLoadPromise = getAccountSecurityDefinitionsLoadPromise();
                    promises.push(accountSecurityDefinitionsLoadPromise);


                    function getStatusDefinitionSelectorLoadPromise() {
                        var accountActionDefinitionPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Vanrise.Common.Business.StatusDefinitionBEFilter, Vanrise.Common.Business"
                                }]
                            },
                            selectedIds: statusBEDefinitionId
                        };
                        return statusBEDefinitionSelectorAPI.load(accountActionDefinitionPayload);
                    }

                    function getAccountTypeSelectorLoadPromise() {

                        var accountTypeSelectorPayload = {
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId
                            },
                            selectedIds: localServiceAccountTypeId
                        };
                        return accountTypeSelectorAPI.load(accountTypeSelectorPayload);
                    }

                    function getFinancialAccountLocatorLoadPromise() {
                        var financialAccountLocatorPayload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            financialAccountLocator: financialAccountLocator
                        };
                        return financialAccountLocatorAPI.load(financialAccountLocatorPayload);
                    }

                    function getAccountGridDefinitionLoadPromise() {
                        var accountGridDefinitionPayload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            accountGridDefinition: accountGridDefinition
                        };
                        return accountGridDefinitionDirectiveAPI.load(accountGridDefinitionPayload);
                    }

                    function getAccountGridDefinitionExportExcelLoadPromise() {
                        var accountGridDefinitionExportExcelPayload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            accountGridDefinitionExportExcel: accountGridDefinitionExportExcel
                        };
                        return accountGridDefinitionExportExcelDirectiveAPI.load(accountGridDefinitionExportExcelPayload);
                    }

                    function getAccountViewDefinitionLoadPromise() {
                        var accountViewDefinitionPayload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            accountViewDefinitions: accountViewDefinitions
                        };
                        return accountViewDefinitionDirectiveAPI.load(accountViewDefinitionPayload);
                    }

                    function getAccountActionDefinitionLoadPromise() {
                        var accountActionDefinitionPayload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            accountActionDefinitions: accountActionDefinitions
                        };
                        return accountActionDefinitionDirectiveAPI.load(accountActionDefinitionPayload);
                    }

                    function getAccountExtraFieldDefinitionsLoadPromise() {
                        var accountExtraFieldDefinitionsDefinitionPayload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            accountExtraFieldDefinitions: accountExtraFieldDefinitions
                        };
                        return accountExtraFieldDefinitionsDirectiveAPI.load(accountExtraFieldDefinitionsDefinitionPayload);
                    }

                    function getAccountSecurityDefinitionsLoadPromise() {
                        return securityDefinitionsDirectiveAPI.load(securityDefinition);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var gridDefinition = {
                        ColumnDefinitions: accountGridDefinitionDirectiveAPI.getData(),
                        ExportColumnDefinitions: accountGridDefinitionExportExcelDirectiveAPI.getData()
                    };

                    var obj = {
                        $type: "Retail.BusinessEntity.Entities.AccountBEDefinitionSettings, Retail.BusinessEntity.Entities",
                        StatusBEDefinitionId: statusBEDefinitionSelectorAPI.getSelectedIds(),
                        LocalServiceAccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        FinancialAccountLocator: financialAccountLocatorAPI.getData(),
                        UseRemoteSelector: $scope.scopeModel.useRemoteSelector,
                        GridDefinition: gridDefinition,
                        AccountViewDefinitions: accountViewDefinitionDirectiveAPI.getData(),
                        ActionDefinitions: accountActionDefinitionDirectiveAPI.getData(),
                        AccountExtraFieldDefinitions: accountExtraFieldDefinitionsDirectiveAPI.getData(),
                        Security: securityDefinitionsDirectiveAPI.getData()
                    };
                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }]);