﻿'use strict';
app.directive('retailBeAccountFinancialaccountSelector', ['Retail_BE_FinancialAccountAPIService', 'VRUIUtilsService','UtilsService',
    function (Retail_BE_FinancialAccountAPIService, VRUIUtilsService, UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                ismultipleselection: "@",
                onselectionchanged: '=',
                isrequired: "=",
                isdisabled: "=",
                selectedvalues: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                ctrl.datasource = [];
                var ctor = new FinancialAccountSelectorCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            template: function (element, attrs) {
                return getTemplate(attrs);
            }
        };


        function getTemplate(attrs) {
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                multipleselection = "ismultipleselection";
            }
            return '<retail-be-account-selector on-ready="scopeModel.onAccountSelectorReady" isrequired="ctrl.isrequired" ' + multipleselection + '  normal-col-num = "{{ctrl.normalColNum}}" onselectionchanged="scopeModel.onAccountSelectionChanged"> </retail-be-account-selector>'
                    +'<retail-be-financialaccount-selector on-ready="scopeModel.onFinancialAccountSelectorReady" ' + multipleselection + ' isrequired="ctrl.isrequired" normal-col-num = "{{ctrl.normalColNum}}"> </retail-be-financialaccount-selector>';
        }

        function FinancialAccountSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var accountSelectorAPI;
            var accountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var financialAccountSelectorAPI;
            var financialAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountBEDefinitionId;

            var status;
            var effectiveDate;
            var isEffectiveInFuture;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountSelectorReady = function (api) {
                    accountSelectorAPI = api;
                    accountSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onFinancialAccountSelectorReady = function (api) {
                    financialAccountSelectorAPI = api;
                    financialAccountSelectorPromiseDeferred.resolve();
                };

                $scope.scopeModel.onAccountSelectionChanged = function (value) {
                  
                    var selectedIds = accountSelectorAPI.getSelectedIds();
                    if (selectedIds != undefined) {
                        var accountIds = [selectedIds];
                        if (attrs.ismultipleselection != undefined) {
                            accountIds = selectedIds
                        }

                        var selectorPayload = {
                            accountBEDefinitionId: accountBEDefinitionId,
                            filter: {
                                AccountIds: accountIds,
                                Status: status,
                                EffectiveDate: effectiveDate,
                                IsEffectiveInFuture: isEffectiveInFuture
                            },
                            setItemsSelected : true
                        };
                        var setLoader = function (value) {
                            $scope.scopeModel.isAccountTypeSelectorLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, financialAccountSelectorAPI, selectorPayload, setLoader, financialAccountSelectorPromiseDeferred);
                    }
                    if (ctrl.onselectionchanged != undefined)
                        ctrl.onselectionchanged(value);
                };

                UtilsService.waitMultiplePromises([accountSelectorPromiseDeferred.promise, financialAccountSelectorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        accountBEDefinitionId = payload.AccountBEDefinitionId;
                        selectedIds = payload.selectedIds;
                        status = payload.status;
                        effectiveDate = payload.effectiveDate;
                        isEffectiveInFuture = payload.isEffectiveInFuture;
                    }

                    var promises = [];
                    var financialAccountSelectedIds = selectedIds;
                    if (selectedIds != undefined)
                    {
                        if (attrs.ismultipleselection == undefined) {
                            financialAccountSelectedIds = [selectedIds];
                        }
                        var loadPromiseDeffered = UtilsService.createPromiseDeferred();
                        promises.push(loadPromiseDeffered.promise);

                        var accountSelectedIds;
                        Retail_BE_FinancialAccountAPIService.GetAccountIdsByFinancialAccountIds(accountBEDefinitionId, financialAccountSelectedIds).then(function (response) {
                            accountSelectedIds = response;
                            if (attrs.ismultipleselection == undefined) {
                                accountSelectedIds = response[0];
                            }

                            loadAccountSelector(accountSelectedIds).then(function () {
                                loadFinancialAccountSelector().then(function () {
                                    loadPromiseDeffered.resolve();
                                }).catch(function (error) {
                                    loadPromiseDeffered.reject(error);
                                });
                            }).catch(function (error) {
                                loadPromiseDeffered.reject(error);
                            });
                        }).catch(function (error) {
                            loadPromiseDeffered.reject(error);
                        });

                        function loadFinancialAccountSelector() {
                            var accountIds = accountSelectedIds;
                            if (attrs.ismultipleselection == undefined) {
                                accountIds = [accountSelectedIds];
                            }
                            var financialAccountPayload = {
                                accountBEDefinitionId: accountBEDefinitionId,
                                selectedIds: selectedIds,
                                filter: {
                                    AccountIds: accountIds,
                                    Status: status,
                                    EffectiveDate: effectiveDate,
                                    IsEffectiveInFuture: isEffectiveInFuture
                                },
                                setItemsSelected: true
                            };
                           return financialAccountSelectorAPI.load(financialAccountPayload);
                        }
                    }

                    if(selectedIds == undefined)
                      promises.push(loadAccountSelector());

                    function loadAccountSelector(accountSelectedIds) {
                        var selectorPayload = {
                            AccountBEDefinitionId: accountBEDefinitionId,
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.FinancialAccountBEFilter, Retail.BusinessEntity.Business",
                                    Status: status,
                                    EffectiveDate: effectiveDate,
                                    IsEffectiveInFuture: isEffectiveInFuture
                                }],
                            },
                            selectedIds:accountSelectedIds
                        };
                        return accountSelectorAPI.load(selectorPayload);
                    }

                    return UtilsService.waitMultiplePromises(promises).then(function () {
                        financialAccountSelectorPromiseDeferred = undefined;
                        accountSelectorPromiseDeferred = undefined;
                    });

                };

                api.getSelectedIds = function () {
                    return financialAccountSelectorAPI.getSelectedIds();
                };
               
                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }
        }

        return directiveDefinitionObject;

    }]);