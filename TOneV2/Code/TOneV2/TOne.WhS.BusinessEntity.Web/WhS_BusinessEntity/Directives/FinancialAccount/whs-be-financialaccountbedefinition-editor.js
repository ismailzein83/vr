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
                $scope.scopeModel.selectedInvoiceTypes = [];
                $scope.scopeModel.datasource = [];
                $scope.scopeModel.onSelectItem = function (dataItem) {
                    addInvoiceTypeAPIFunction(dataItem);
                };
                $scope.scopeModel.onDeselectItem = function (dataItem) {
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.InvoiceTypeId, 'Entity.InvoiceTypeId');
                    $scope.scopeModel.datasource.splice(datasourceIndex, 1);
                };
                function addInvoiceTypeAPIFunction(obj) {
                    var dataItem = {
                        InvoiceTypeId: obj.InvoiceTypeId,
                        InvoiceTypeName: obj.Name,
                        InvoiceSettingTitle: obj.Name,
                    };
                    $scope.scopeModel.datasource.push({ Entity: dataItem });
                }
                $scope.scopeModel.removeInvoiceType = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedInvoiceTypes, dataItem.Entity.InvoiceTypeId, 'InvoiceTypeId');
                    $scope.scopeModel.selectedInvoiceTypes.splice(index, 1);
                    var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.Entity.InvoiceTypeId, 'Entity.InvoiceTypeId');
                    $scope.scopeModel.datasource.splice(datasourceIndex, 1);
                };
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

                    var beDefinitionSettings;
                    var invoiceTypeIds = [];
                    if (payload != undefined && payload.businessEntityDefinitionSettings != undefined) {
                        beDefinitionSettings = payload.businessEntityDefinitionSettings;
                        if (beDefinitionSettings != undefined && beDefinitionSettings.FinancialAccountInvoiceTypes != undefined) {
                            for (var i = 0; i < beDefinitionSettings.FinancialAccountInvoiceTypes.length; i++) {
                                var obj = beDefinitionSettings.FinancialAccountInvoiceTypes[i];
                                invoiceTypeIds.push(obj.InvoiceTypeId);
                            }
                        }
                    }

                    
                    function loadFinancialAccountInvoiceTypesGrid() {
                        if (beDefinitionSettings != undefined && beDefinitionSettings.FinancialAccountInvoiceTypes != undefined) {
                            for (var i = 0; i < beDefinitionSettings.FinancialAccountInvoiceTypes.length; i++) {
                                var item = {
                                    payload: beDefinitionSettings.FinancialAccountInvoiceTypes[i],
                                };
                                addItemToGrid(item);
                            }
                        }
                        function addItemToGrid(item) {
                            for (var i = 0; i < $scope.scopeModel.selectedInvoiceTypes.length; i++) {
                                var invoiceType = $scope.scopeModel.selectedInvoiceTypes[i];
                                if (item.payload.InvoiceTypeId == invoiceType.InvoiceTypeId) {
                                    addAPIToDataItem(item, invoiceType);
                                }
                            }
                        }
                        function addAPIToDataItem(item, invoiceType) {

                            var dataItem = {
                                InvoiceTypeId: invoiceType.InvoiceTypeId,
                                InvoiceTypeName: invoiceType.Name,
                                IsApplicableToSupplier: item.payload.IsApplicableToSupplier,
                                IsApplicableToCustomer: item.payload.IsApplicableToCustomer,
                                InvoiceSettingTitle: item.payload.InvoiceSettingTitle,
                                IgnoreFromBalance: item.payload.IgnoreFromBalance,
                                IsSecondaryInvoiceAccount: item.payload.IsSecondaryInvoiceAccount,
                                DisableCommission: item.payload.DisableCommission,
                            };
                            $scope.scopeModel.datasource.push({ Entity: dataItem });
                        }
                    }

                    function loadInvoiceTypeSelector() {
                        var invoiceTypeSelectorPayload;
                        if (beDefinitionSettings != undefined) {
                            invoiceTypeSelectorPayload = { selectedIds: invoiceTypeIds };
                        }
                        return invoiceTypeSelectorAPI.load(invoiceTypeSelectorPayload);
                    }
                    var invoiceTypeSelectorPromise = loadInvoiceTypeSelector();
                    invoiceTypeSelectorPromise.then(function () {
                        loadFinancialAccountInvoiceTypesGrid();
                    });
                    promises.push(invoiceTypeSelectorPromise);

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
                    var financialAccountInvoiceTypes = [];
                    for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                        var item = $scope.scopeModel.datasource[i];
                        financialAccountInvoiceTypes.push({
                            InvoiceTypeId: item.Entity.InvoiceTypeId,
                            IsApplicableToCustomer: item.Entity.IsApplicableToCustomer,
                            IsApplicableToSupplier: item.Entity.IsApplicableToSupplier,
                            IgnoreFromBalance: item.Entity.IgnoreFromBalance,
                            IsSecondaryInvoiceAccount: item.Entity.IsSecondaryInvoiceAccount,
                            DisableCommission: item.Entity.DisableCommission,
                            InvoiceSettingTitle: item.Entity.InvoiceSettingTitle,
                        });
                    }
                    var obj = {
                        $type: "TOne.WhS.BusinessEntity.Business.WHSFinancialAccountDefinitionSettings, TOne.WhS.BusinessEntity.Business",
                        FinancialAccountInvoiceTypes: financialAccountInvoiceTypes,
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