'use strict';
app.directive('retailInvoiceFinancialaccountSelector', ['Retail_BE_AccountAPIService', 'VRUIUtilsService', 'UtilsService',
function (Retail_BE_AccountAPIService, VRUIUtilsService, UtilsService) {

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
                var accountCtor = new AccountCtor(ctrl, $scope, $attrs);
                accountCtor.initializeController();
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
                return getAccountTemplate(attrs);
            }

        };


        function getAccountTemplate(attrs) {
            var label = "Account";
            var multipleselection = "";
            if (attrs.ismultipleselection != undefined) {
                label = "Accounts";
                multipleselection = "ismultipleselection";
            }

            return '<vr-columns colnum="{{ctrl.normalColNum}}">'
                   + '<retail-be-account-selector on-ready="ctrl.onSelectorReady"'
                   + ' ' + multipleselection
                   + '  isrequired="ctrl.isrequired"'
                   + '  label="' + label + '"'
                   + '  >'
                   + '</retail-be-account-selector>'
                   + '</vr-columns>';
        }

        function AccountCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var filter;
            var selectorAPI;

            function initializeController() {

                ctrl.onSelectorReady = function (api) {
                    selectorAPI = api;

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                        ctrl.onReady(defineAPI());
                    }
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {

                    var selectedIds;
                    var accountSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    if (payload != undefined) {
                        selectedIds = payload.selectedIds
                    }

                    if (selectedIds != undefined) {
                        var selectedAccountIds = [];
                        if (attrs.ismultipleselection != undefined)
                            selectedAccountIds = selectedIds;
                        else
                            selectedAccountIds.push(selectedIds);

                        var financailAccountsPayload = {
                            selectedIds: selectedIds,
                            filter: getAccountSelectorFilter
                        }

                        VRUIUtilsService.callDirectiveLoad(selectorAPI, financailAccountsPayload, accountSelectorLoadDeferred);
                    }


                };

                api.getSelectedIds = function () {
                    return VRUIUtilsService.getIdSelectedIds('AccountId', attrs, ctrl);
                };

                api.getData = function () {
                    selectorAPI.getSelectedIds();
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);

                return api;

            }

            function getAccountSelectorFilter() {
                var filter = {};

                filter.Filters = [];

                var financialAccounts = {
                    $type: 'Retail.Invoice.Business.InvoiceEnabledAccountFilter, Retail.Invoice.Business',
                    test: "Test"
                };
                filter.Filters.push(financialAccounts);


                return filter;
            }
        }

        return directiveDefinitionObject;

    }]);