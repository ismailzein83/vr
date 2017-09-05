﻿'use strict';
app.directive('whsInvoiceInvoiceaccountSelector', ['VRUIUtilsService', 'UtilsService',
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
            return '<whs-be-financialaccount-selector on-ready="scopeModel.onFinancialAccountSelectorReady" ' + multipleselection + ' onselectionchanged="scopeModel.onAccountSelected" isrequired="ctrl.isrequired" normal-col-num = "{{ctrl.normalColNum}}"> </whs-be-financialaccount-selector>';
        }

        function InvoiceAccountSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var financialAccountSelectorAPI;
            var financialAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var status;
            var effectiveDate;
            var isEffectiveInFuture;
            var context;
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
                    if (context.reloadCustomSectionPayload != undefined) {
                        var customSectionPayload = {
                            financialAccountId: selectedIds
                        };
                        context.reloadCustomSectionPayload(customSectionPayload);
                    }
                    if (context.reloadBillingPeriod != undefined) {
                        context.reloadBillingPeriod();
                    }
                }
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var filter;
                    if (payload != undefined) {
                        context = payload.context;
                        filter = payload.filter;
                        if (filter == undefined)
                            filter = {};
                        filter.InvoiceTypeId = payload.invoiceTypeId;
                        selectedIds = payload.selectedIds;
                    }

                    var promises = [];

                    promises.push(loadFinancialAccountSelector());

                    function loadFinancialAccountSelector() {
                        var financialAccountPayload = {
                            selectedIds: selectedIds,
                            filter: filter,
                            context:getContext()
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
                function getContext()
                {
                    var currentContext = context;
                    if(currentContext == undefined)
                    {
                        currentContext = {};
                    }
                    return currentContext;
                }
                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;
            }
        }

        return directiveDefinitionObject;

    }]);