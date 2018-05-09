'use strict';

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

            var classificationSelectorAPI;
            var classificationSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var selectedBusinessEntityDefinitionDeferred;


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.datasource = [];

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

                $scope.scopeModel.onClassificationSelectorReady = function (api) {
                    classificationSelectorAPI = api;
                    classificationSelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.onBusinessEntityDefinitionSelectorChanged = function (value) {
                    if (value != undefined) {
                        if (selectedBusinessEntityDefinitionDeferred != undefined)
                            selectedBusinessEntityDefinitionDeferred.resolve();
                        else {
                            $scope.scopeModel.datasource.length = 0;
                            var classificationSelectorPayload = { AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds() };
                            var setLoader = function (value) {
                                $scope.scopeModel.isClassificationSelectorLoading = value;
                            };
                            VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, classificationSelectorAPI, classificationSelectorPayload, setLoader);
                        }
                    }
                };

                $scope.scopeModel.onExtendedSettingsDirectiveReady = function (api) {
                    extendedSettingsDirectiveAPI = api;
                    extendedSettingsDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddInvoiceTypes = function () {
               
                   var dataItem = {
                       id: $scope.scopeModel.datasource.length + 1,
                   };

                    dataItem.onInvoiceTypeSelectorReady = function (api) {
                        dataItem.invoiceTypeSelectorAPI = api;
                        var setLoader = function (value) { $scope.scopeModel.isLoadingDirective = value; };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.invoiceTypeSelectorAPI, undefined, setLoader);
                    };

                    dataItem.onApplicableClassificationsSelectorReady = function (api) {
                        dataItem.applicableClassificationsSelectorAPI = api;
                        var dataItemApplicableClassificationSelectorPayload = {
                            AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds()
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isClassificationSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, dataItem.applicableClassificationsSelectorAPI, dataItemApplicableClassificationSelectorPayload, setLoader);
                       
                    };

                    $scope.scopeModel.datasource.push(dataItem);
                };

                $scope.scopeModel.removeRow = function (dataItem) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.id, 'id');
                    $scope.scopeModel.datasource.splice(index, 1);
                };

                UtilsService.waitMultiplePromises([invoiceTypeSelectorReadyDeferred.promise, balanceAccountTypeSelectorReadyDeferred.promise,
                    extendedSettingsDirectiveReadyDeferred.promise]).then(function () {
                        defineAPI();
                    });

            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    
                    var financialAccountDefinitionSettings;
                    if (payload != undefined) {
                        financialAccountDefinitionSettings = payload.componentType;

                        if (financialAccountDefinitionSettings != undefined) {
                            $scope.scopeModel.name = financialAccountDefinitionSettings.Name;
                            if (financialAccountDefinitionSettings.Settings != undefined) {
                                selectedBusinessEntityDefinitionDeferred = UtilsService.createPromiseDeferred();
                                promises.push(loadClassificationSelector());

                                if (financialAccountDefinitionSettings.Settings.InvoiceTypes != undefined) {

                                    for (var i = 0; i < financialAccountDefinitionSettings.Settings.InvoiceTypes.length ; i++) {
                                        var invoiceType = financialAccountDefinitionSettings.Settings.InvoiceTypes[i];
                                        var gridItem = {
                                            payload: invoiceType,
                                            readyInvoiceTypeSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            loadInvoiceTypeSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            readyApplicableClassificationsSelectorPromiseDeferred: UtilsService.createPromiseDeferred(),
                                            loadApplicableClassificationsSelectorPromiseDeferred: UtilsService.createPromiseDeferred()
                                        };
                                        promises.push(gridItem.loadInvoiceTypeSelectorPromiseDeferred.promise);
                                        promises.push(gridItem.loadApplicableClassificationsSelectorPromiseDeferred.promise);
                                        addItemToGrid(gridItem);
                                    }
                                }
                            }
                        }
                    }

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

                    function loadClassificationSelector() {
                        var classificationSelectorPayload;
                        var classificationSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([selectedBusinessEntityDefinitionDeferred.promise, classificationSelectorReadyDeferred.promise]).then(function () {
                            classificationSelectorPayload = {
                                AccountBEDefinitionId: financialAccountDefinitionSettings.Settings.AccountBEDefinitionId,
                                selectedIds: financialAccountDefinitionSettings.Settings.ApplicableClassifications
                            };
                            VRUIUtilsService.callDirectiveLoad(classificationSelectorAPI, classificationSelectorPayload, classificationSelectorLoadDeferred);
                        });
                        return classificationSelectorLoadDeferred.promise;
                    }

                    function loadExtendedSettingsDirective() {
                        var extendedSettingsDirectivePayload;
                        if (financialAccountDefinitionSettings != undefined && financialAccountDefinitionSettings.Settings != undefined) {
                            extendedSettingsDirectivePayload = { extendedSettings: financialAccountDefinitionSettings.Settings.ExtendedSettings };
                        }
                        return extendedSettingsDirectiveAPI.load(extendedSettingsDirectivePayload);
                    }
                    promises.push(loadExtendedSettingsDirective());

                    function addItemToGrid(gridItem) {
                        var dataItem = {
                            id: $scope.scopeModel.datasource.length + 1,
                        };
                        dataItem.onInvoiceTypeSelectorReady = function (api) {
                            dataItem.invoiceTypeSelectorAPI = api;
                            gridItem.readyInvoiceTypeSelectorPromiseDeferred.resolve();
                        };
                        UtilsService.waitMultiplePromises([selectedBusinessEntityDefinitionDeferred.promise, gridItem.readyInvoiceTypeSelectorPromiseDeferred.promise]).then(function () {
                            var dataItemPayload = { selectedIds: gridItem.payload.InvoiceTypeId };
                            VRUIUtilsService.callDirectiveLoad(dataItem.invoiceTypeSelectorAPI, dataItemPayload, gridItem.loadInvoiceTypeSelectorPromiseDeferred);
                        });

                        dataItem.onApplicableClassificationsSelectorReady = function (api) {
                            dataItem.applicableClassificationsSelectorAPI = api;
                            gridItem.readyApplicableClassificationsSelectorPromiseDeferred.resolve();
                        };
                        

                        UtilsService.waitMultiplePromises([selectedBusinessEntityDefinitionDeferred.promise, gridItem.readyApplicableClassificationsSelectorPromiseDeferred.promise]).then(function () {

                            var dataItemApplicableClassificationSelectorPayload = {
                                AccountBEDefinitionId: financialAccountDefinitionSettings.Settings.AccountBEDefinitionId,
                                selectedIds: gridItem.payload.ApplicableClassifications
                            };
                            VRUIUtilsService.callDirectiveLoad(dataItem.applicableClassificationsSelectorAPI, dataItemApplicableClassificationSelectorPayload, gridItem.loadApplicableClassificationsSelectorPromiseDeferred);
                        });

                        $scope.scopeModel.datasource.push(dataItem);

                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        selectedBusinessEntityDefinitionDeferred = undefined;
                    });
                };

                api.getData = function () {
                    return {
                        Name: $scope.scopeModel.name,
                        Settings: {
                            $type: "Retail.BusinessEntity.Entities.FinancialAccountDefinitionSettings, Retail.BusinessEntity.Entities",
                            InvoiceTypeId: invoiceTypeSelectorAPI.getSelectedIds(),
                            BalanceAccountTypeId: balanceAccountTypeSelectorAPI.getSelectedIds(),
                            AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds(),
                            InvoiceTypes: getInvoiceTypes(),
                            ApplicableClassifications: classificationSelectorAPI.getSelectedIds(),
                            ExtendedSettings: extendedSettingsDirectiveAPI.getData()
                        }
                    };
                };

                function getInvoiceTypes() {
                    var invoiceTypes;
                    if ($scope.scopeModel.datasource != undefined && $scope.scopeModel.datasource.length > 0) {
                        invoiceTypes = [];
                        for (var i = 0; i < $scope.scopeModel.datasource.length; i++) {
                            var dataItem = $scope.scopeModel.datasource[i];
                            var financialAccountInvoiceType = {
                                InvoiceTypeId: dataItem.invoiceTypeSelectorAPI.getSelectedIds(),
                                ApplicableClassifications: dataItem.applicableClassificationsSelectorAPI.getSelectedIds()
                            };
                            invoiceTypes.push(financialAccountInvoiceType);
                        }
                    }
                    return invoiceTypes;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);