'use strict';
app.directive('retailBeAccountInvoiceSelector', ['VRUIUtilsService', 'UtilsService',
    function (VRUIUtilsService, UtilsService) {

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
                var ctor = new InvoiceAccountSelectorCtor(ctrl, $scope, $attrs);
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
            return '<retail-be-account-financialaccount-selector on-ready="scopeModel.onFinancialAccountSelectorReady" ' + multipleselection + ' onselectionchanged="scopeModel.onAccountSelected" isrequired="ctrl.isrequired" normal-col-num = "{{ctrl.normalColNum}}"> </retail-be-account-financialaccount-selector>';
        }

        function InvoiceAccountSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var financialAccountSelectorAPI;
            var financialAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountBEDefinitionId;

            var status;
            var effectiveDate;
            var isEffectiveInFuture;
            var context;
            var invoiceTypeId;

            function initializeController() {
                $scope.scopeModel = {};
                
                $scope.scopeModel.onFinancialAccountSelectorReady = function (api) {
                    financialAccountSelectorAPI = api;
                    financialAccountSelectorPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAccountSelected = function (selectedAccount) {
                    if (financialAccountSelectorAPI != undefined) {
                        var selectedIds = financialAccountSelectorAPI.getSelectedIds();
                        if (selectedIds != undefined) {
                            reloadContextFunctions(selectedIds);
                        }
                    }
                };
                UtilsService.waitMultiplePromises([financialAccountSelectorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function reloadContextFunctions(selectedIds) {
                if (context != undefined) {
                    if (context.onAccountSelected != undefined) {
                        context.onAccountSelected(selectedIds);
                    }
                    if (context.reloadPregeneratorActions != undefined) {
                        context.reloadPregeneratorActions();
                    }
                    //if (context.setTimeZone != undefined) {
                    //    var timeZoneId = selectedAccount != undefined ? selectedAccount.TimeZoneId : undefined;
                    //    context.setTimeZone(timeZoneId);
                    //}
                    if (context.reloadBillingPeriod != undefined) {
                        context.reloadBillingPeriod();
                    }
                }
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var partnerInvoiceFilters;
                    if (payload != undefined) {
                        invoiceTypeId = payload.invoiceTypeId;
                        context = payload.context;
                        var filters = [];
                        if (payload.extendedSettings != undefined) {
                            accountBEDefinitionId = payload.extendedSettings.AccountBEDefinitionId;
                        }
                        if (payload.filter != undefined) {
                            if (payload.filter.Filters != undefined) {
                                filters = payload.filter.Filters;
                            }
                            status = payload.filter.Status;
                            effectiveDate = payload.filter.EffectiveDate;
                            isEffectiveInFuture = payload.filter.IsEffectiveInFuture;
                        }
                        selectedIds = payload.selectedIds;
                        partnerInvoiceFilters = payload.partnerInvoiceFilters;

                    }

                    var promises = [];

                    promises.push(loadFinancialAccountSelector());

                    function loadFinancialAccountSelector() {

                        var accountSelectorFilter =[];
                        if (filters != undefined) {
                            for (var i = 0; i < filters.length; i++) {
                                accountSelectorFilter.push(filters[i]);
                            }
                        }

                        accountSelectorFilter.push({
                            $type: "Retail.BusinessEntity.Business.Filters.AccountInvoiceStatusFilter ,Retail.BusinessEntity.Business",
                            Status: status
                        });

                        if (filters == undefined)
                            filters = [];
                        filters.push({
                            $type: "Retail.BusinessEntity.Business.FinancialAccountInvoiceTypeFilter ,Retail.BusinessEntity.Business",
                            InvoiceTypeId: invoiceTypeId
                        });
                        var financialAccountPayload = {
                            AccountBEDefinitionId: accountBEDefinitionId,
                            selectedIds: selectedIds,
                            status: status,
                            effectiveDate: effectiveDate,
                            isEffectiveInFuture: isEffectiveInFuture,
                            filter: {
                                Filters: accountSelectorFilter
                            },
                            financialAccountFilters: filters,
                            partnerInvoiceFilters: partnerInvoiceFilters
                        };
                        return financialAccountSelectorAPI.load(financialAccountPayload);
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        selectedIds: financialAccountSelectorAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }
        }

        return directiveDefinitionObject;

    }]);