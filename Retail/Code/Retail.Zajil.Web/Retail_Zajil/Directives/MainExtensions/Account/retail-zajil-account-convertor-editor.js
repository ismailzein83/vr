'use strict';

app.directive('retailZajilAccountConvertorEditor', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_EntityTypeEnum', 'VRNotificationService',
    function (UtilsService, VRUIUtilsService, Retail_BE_EntityTypeEnum, VRNotificationService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new retailZajilAccountConvertorEditorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_Zajil/Directives/MainExtensions/Account/Templates/AccountConvertorEditor.html"
        };

        function retailZajilAccountConvertorEditorCtor(ctrl, $scope, $attrs) {
            this.initializeController = initializeController;

            var accountBEDefinitionId;
            var accountTypeId;
            var statusDefinitionId;

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectionChangedDeferred;

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var siteAccountTypeSelectorAPI;
            var siteAccountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var companyProfileDefinitionSelectorAPI;
            var companyProfileDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var financialDefinitionSelectorAPI;
            var financialDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var orderDetailsDefinitionSelectorAPI;
            var orderDetailsDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var companyExtendedInfoSelectorAPI;
            var companyExtendedInfoReadyDeferred = UtilsService.createPromiseDeferred();

            var productSelectorAPI;
            var productSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();


               

            $scope.scopeModel = {};

            $scope.scopeModel.onAccountDefinitionSelectorReady = function (api) {
                accountDefinitionSelectorApi = api;
                accountDefinitionSelectorPromiseDeferred.resolve();
            };

            $scope.scopeModel.onInitialStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onSiteAccountTypeSelectorReady = function (api) {
                siteAccountTypeSelectorAPI = api;
                siteAccountTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCompanyProfileDefinitionSelectorReady = function (api) {
                companyProfileDefinitionSelectorAPI = api;
                companyProfileDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onFinancialDefinitionSelectorReady = function (api) {
                financialDefinitionSelectorAPI = api;
                financialDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onOrderDetailsDefinitionSelectorReady = function (api) {
                orderDetailsDefinitionSelectorAPI = api;
                orderDetailsDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onCompanyExtentedInfoSelectorReady = function (api) {
                companyExtendedInfoSelectorAPI = api;
                companyExtendedInfoReadyDeferred.resolve();
            };

            $scope.scopeModel.onProductSelectorReady = function (api) {
                productSelectorAPI = api;
                productSelectorReadyPromiseDeferred.resolve();
            };
            $scope.scopeModel.onAccountDefinitionSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined && businessEntityDefinitionSelectionChangedDeferred == undefined) {
                    businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                    businessEntityDefinitionSelectionChangedDeferred.resolve();

                    $scope.scopeModel.isAccountTypeSelectorLoading = true;
                    $scope.scopeModel.isStatuseSelectorLoading = true;
                    $scope.scopeModel.isProductSelectorLoading = true;

                    var promises = [];

                    var accountTypeSelectorPayload = {
                        filter: {
                            AccountBEDefinitionId: selectedItem.BusinessEntityDefinitionId
                        },
                    };
                    var statusSelectorPayload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter, Retail.BusinessEntity.Business",
                                AccountBEDefinitionId: selectedItem.BusinessEntityDefinitionId
                            }]
                        },
                    };
                    var productSelectorPayload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountDefinitionProductFilter, Retail.BusinessEntity.Business",
                                AccountBEDefinitionId: selectedItem.BusinessEntityDefinitionId
                            }]
                        },
                    };
                    promises.push(loadAccountTypeSelector(accountTypeSelectorPayload));
                    promises.push(loadStatusDefinitionSelector(statusSelectorPayload));
                    promises.push(loadProductSelector(productSelectorPayload));
                    UtilsService.waitMultiplePromises(promises).then(function () {
                        businessEntityDefinitionSelectionChangedDeferred = undefined;
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isAccountTypeSelectorLoading = false;
                        $scope.scopeModel.isStatuseSelectorLoading = false;
                        $scope.scopeModel.isProductSelectorLoading = false;
                    });
                } else if (businessEntityDefinitionSelectionChangedDeferred != undefined) {
                    businessEntityDefinitionSelectionChangedDeferred.resolve();
                }
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var accountTypeSelectorPayload;
                    var statusDefinitionSelectorPayload;
                    promises.push(loadAccountDefinitionSelectorLoad());
                    //promises.push(loadStatusDefinitionSelector());

                    if (payload != undefined) {
                        accountTypeId = payload.AccountTypeId;
                        statusDefinitionId = payload.InitialStatusId;
                        accountBEDefinitionId = payload.AccountBEDefinitionId;

                        accountTypeSelectorPayload = {
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId
                            },
                            selectedIds: payload.AccountTypeId
                        };

                        statusDefinitionSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter, Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: payload.AccountBEDefinitionId

                                }]
                            },
                            selectedIds: payload.InitialStatusId
                        };
                        var productSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountDefinitionProductFilter, Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: accountBEDefinitionId
                                }]
                            },
                            selectedIds: payload.DefaultProductId
                        };

                        businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();
                        promises.push(loadProductSelector(productSelectorPayload));
                        promises.push(loadAccountTypeSelector(accountTypeSelectorPayload));
                        promises.push(loadStatusDefinitionSelector(statusDefinitionSelectorPayload));
                    }

                    promises.push(loadCompanyProfileDefinitionSelector());
                    promises.push(loadFinancialDefinitionSelector());
                    promises.push(loadOrderDetailsDefinitionSelector());
                    promises.push(loadCompanyExtendedInfoDefinitionSelector());

                    function loadAccountDefinitionSelectorLoad() {
                        var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        accountDefinitionSelectorPromiseDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    Filters: [{
                                        $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business"
                                    }]
                                }
                            };

                            if (payload != undefined) {
                                selectorPayload.selectedIds = payload.AccountBEDefinitionId;
                            }
                            VRUIUtilsService.callDirectiveLoad(accountDefinitionSelectorApi, selectorPayload, businessEntityDefinitionSelectorLoadDeferred);
                        });

                        return businessEntityDefinitionSelectorLoadDeferred.promise;
                    };

                    function loadCompanyProfileDefinitionSelector() {
                        var companyProfileSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        companyProfileDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('1AFF2BF7-1F15-4E0B-ACCF-457EDF36A342')
                                },
                                selectedIds: payload != undefined ? payload.CompanyProfilePartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(companyProfileDefinitionSelectorAPI, selectorPayload, companyProfileSelectorLoadDeferred);
                        });
                        return companyProfileSelectorLoadDeferred.promise;
                    };

                    function loadFinancialDefinitionSelector() {
                        var financialSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        financialDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('BA425FA1-13CA-4F44-883A-2A12B7E3F988')
                                },
                                selectedIds: payload != undefined ? payload.FinancialPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(financialDefinitionSelectorAPI, selectorPayload, financialSelectorLoadDeferred);
                        });
                        return financialSelectorLoadDeferred.promise;
                    };

                    function loadOrderDetailsDefinitionSelector() {
                        var orderDetailsSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        orderDetailsDefinitionSelectorReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('2241197C-B5B0-48E5-987A-B3C1949760CB')
                                },
                                selectedIds: payload != undefined ? payload.OrderDetailsPartDefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(orderDetailsDefinitionSelectorAPI, selectorPayload, orderDetailsSelectorLoadDeferred);
                        });
                        return orderDetailsSelectorLoadDeferred.promise;
                    };
                    function loadCompanyExtendedInfoDefinitionSelector() {
                        var extendedInfoSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        companyExtendedInfoReadyDeferred.promise.then(function () {
                            var selectorPayload = {
                                filter: {
                                    AccountPartDefinitionIds: getAccountPartDefinitionIds('F6630722-4E85-4DF2-915F-F9942074743C')
                                },
                                selectedIds: payload != undefined ? payload.CompanyExtendedInfoPartdefinitionId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(companyExtendedInfoSelectorAPI, selectorPayload, extendedInfoSelectorLoadDeferred);
                        });
                        return extendedInfoSelectorLoadDeferred.promise;
                    };
                
                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        businessEntityDefinitionSelectionChangedDeferred = undefined;
                    });
                };


                api.getData = function () {
                    var data = {
                        $type: "Retail.Zajil.MainExtensions.AccountConvertor, Retail.Zajil.MainExtensions",
                        Name: "Zajil Account Convertor",
                        AccountBEDefinitionId: accountDefinitionSelectorApi.getSelectedIds(),
                        InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                        AccountTypeId: accountTypeSelectorAPI.getSelectedIds(),
                        CompanyProfilePartDefinitionId: companyProfileDefinitionSelectorAPI.getSelectedIds(),
                        FinancialPartDefinitionId: financialDefinitionSelectorAPI.getSelectedIds(),
                        OrderDetailsPartDefinitionId: orderDetailsDefinitionSelectorAPI.getSelectedIds(),
                        CompanyExtendedInfoPartdefinitionId: companyExtendedInfoSelectorAPI.getSelectedIds(),
                        DefaultProductId: productSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function loadProductSelector(productPayload) {
                var productSelectorLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                productSelectorReadyPromiseDeferred.promise.then(function () {
                    VRUIUtilsService.callDirectiveLoad(productSelectorAPI, productPayload, productSelectorLoadPromiseDeferred);
                });
                return productSelectorLoadPromiseDeferred.promise;
            }
            function loadSiteAccountTypeSelector(accountTypeSelectorPayload) {
                var siteAccountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([siteAccountTypeSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {

                    VRUIUtilsService.callDirectiveLoad(siteAccountTypeSelectorAPI, accountTypeSelectorPayload, siteAccountTypeSelectorLoadDeferred);
                });

                return siteAccountTypeSelectorLoadDeferred.promise;
            }

            function loadAccountTypeSelector(accountTypeSelectorPayload) {
                var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([accountTypeSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {

                    VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                });

                return accountTypeSelectorLoadDeferred.promise;
            }

            function loadStatusDefinitionSelector(accountTypeSelectorPayload) {
                var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([statusDefinitionSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {

                    VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                });

                return accountTypeSelectorLoadDeferred.promise;
            }

            function getAccountPartDefinitionIds(id) {
                var partDefinitionIds = [];
                partDefinitionIds.push(id);
                return partDefinitionIds;
            }
        }
        return directiveDefinitionObject;
    }]);