'use strict';

app.directive('retailZajilAccountConvertorEditor', ['UtilsService', 'VRUIUtilsService', 'Retail_BE_EntityTypeEnum',
    function (UtilsService, VRUIUtilsService, Retail_BE_EntityTypeEnum) {

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

            var accountDefinitionSelectorApi;
            var accountDefinitionSelectorPromiseDeferred = UtilsService.createPromiseDeferred();
            var businessEntityDefinitionSelectionChangedDeferred;

            var statusDefinitionSelectorAPI;
            var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var companyProfileDefinitionSelectorAPI;
            var companyProfileDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var financialDefinitionSelectorAPI;
            var financialDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var orderDetailsDefinitionSelectorAPI;
            var orderDetailsDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onAccountDefinitionSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    accountBEDefinitionId = selectedItem.BusinessEntityDefinitionId;

                    var accountTypeSelectorPayload = {
                        filter: {
                            AccountBEDefinitionId: accountBEDefinitionId
                        }
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isAccountTypeSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, accountTypeSelectorAPI, accountTypeSelectorPayload, setLoader, businessEntityDefinitionSelectionChangedDeferred);
                }
            };

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    promises.push(getAccountDefinitionSelectorLoad());
                    promises.push(loadStatusDefinitionSelector());
                    promises.push(loadAccountTypeSelector());
                    promises.push(loadCompanyProfileDefinitionSelector());
                    promises.push(loadFinancialDefinitionSelector());
                    promises.push(loadOrderDetailsDefinitionSelector());

                    function getAccountDefinitionSelectorLoad() {
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

                    function loadStatusDefinitionSelector() {
                        var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                        statusDefinitionSelectorReadyDeferred.promise.then(function () {
                            var statusDefinitionSelectorPayload = {
                                filter: { EntityType: Retail_BE_EntityTypeEnum.Account.value },
                                selectedIds: payload != undefined ? payload.InitialStatusId : undefined
                            };
                            VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, statusDefinitionSelectorLoadDeferred);
                        });
                        return statusDefinitionSelectorLoadDeferred.promise;
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

                    function loadAccountTypeSelector() {

                        if (businessEntityDefinitionSelectionChangedDeferred == undefined)
                            businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                        var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        UtilsService.waitMultiplePromises([accountTypeSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {
                            businessEntityDefinitionSelectionChangedDeferred = undefined;

                            var accountTypeSelectorPayload = {
                                filter: {
                                    AccountBEDefinitionId: accountBEDefinitionId
                                },
                                selectedIds: payload != undefined ? payload.AccountTypeId : undefined
                            };

                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                        });

                        return accountTypeSelectorLoadDeferred.promise;
                    };

                    return UtilsService.waitMultiplePromises(promises);
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
                        OrderDetailsPartDefinitionId: orderDetailsDefinitionSelectorAPI.getSelectedIds()
                    };
                    return data;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getAccountPartDefinitionIds(id) {
                var partDefinitionIds = [];
                partDefinitionIds.push(id);
                return partDefinitionIds;
            }
        }

        return directiveDefinitionObject;
    }]);