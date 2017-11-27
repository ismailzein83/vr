'use strict';
app.directive('retailInvoiceFinancialaccountSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '=',
                ismultipleselection: "@"
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new carriersCtor(ctrl, $scope, $attrs);
                ctor.initializeController();
                ctrl.selectedvalues = ($attrs.ismultipleselection != undefined) ? [] : undefined;

            },
            controllerAs: 'financialAccountCtrl',
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
            return '<retail-be-account-selector isrequired="financialAccountCtrl.isrequired" ' + multipleselection + ' normal-col-num="{{financialAccountCtrl.normalColNum}}" on-ready="financialAccountCtrl.onDirectiveReady" hideremoveicon onselectionchanged="financialAccountCtrl.onSelectionChanged"></retail-be-account-selector>';
        }

        function carriersCtor(ctrl, $scope, attrs) {

            var directiveReadyAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;
            var extendedSettings;
            var partnerInvoiceFilters;
            function initializeController() {

                ctrl.onDirectiveReady = function (api) {
                    directiveReadyAPI = api;
                    directiveReadyPromiseDeferred.resolve();
                };

                ctrl.onSelectionChanged = function (selectedAccount) {
                    if (selectedAccount != undefined && context != undefined) {
                    
                        if (context.reloadBillingPeriod != undefined) {
                            context.reloadBillingPeriod();
                        }
                        if (context.reloadPregeneratorActions != undefined) {
                            context.reloadPregeneratorActions();
                        }
                    }
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    var filter;
                    if (payload != undefined) {
                        context = payload.context;
                        selectedIds = payload.selectedIds;
                        extendedSettings = payload.extendedSettings;
                        partnerInvoiceFilters = payload.partnerInvoiceFilters;
                        
                    }
                    var promises = [];

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = { filter: getAccountSelectorFilter(), AccountBEDefinitionId: extendedSettings.AccountBEDefinitionId };
                        if (selectedIds != undefined) {
                            selectorPayload.selectedIds = selectedIds;
                        }
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, selectorPayload, directiveLoadPromiseDeferred);
                    });
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var selectedIds = directiveReadyAPI.getSelectedIds();
                    return {
                        partnerPrefix: selectedIds,
                        selectedIds: selectedIds,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getAccountSelectorFilter() {
                var filter = {};

                filter.Filters = [];
                if (partnerInvoiceFilters != undefined) {
                    for (var i = 0; i < partnerInvoiceFilters.length; i++) {
                        var partnerInvoiceFilter = partnerInvoiceFilters[i];
                        filter.Filters.push(partnerInvoiceFilter);
                    }
                }
                var financialAccounts = {
                    $type: 'Retail.Invoice.Business.InvoiceEnabledAccountFilter, Retail.Invoice.Business',
                };
                filter.Filters.push(financialAccounts);
                return filter;
            }

            function getContext() {
                var currentContext = context;
                return currentContext;
            }
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);