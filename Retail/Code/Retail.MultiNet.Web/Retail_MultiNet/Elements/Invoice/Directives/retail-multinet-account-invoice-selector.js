'use strict';
app.directive('retailMultinetAccountInvoiceSelector', ['VRUIUtilsService', 'UtilsService',
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
            return '<retail-be-account-financialaccount-selector on-ready="scopeModel.onFinancialAccountSelectorReady" ' + multipleselection + ' isrequired="ctrl.isrequired" normal-col-num = "{{ctrl.normalColNum}}"> </retail-be-account-financialaccount-selector>';
        }

        function InvoiceAccountSelectorCtor(ctrl, $scope, attrs) {

            this.initializeController = initializeController;

            var financialAccountSelectorAPI;
            var financialAccountSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountBEDefinitionId;

            var status;
            var effectiveDate;
            var isEffectiveInFuture;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFinancialAccountSelectorReady = function (api) {
                    financialAccountSelectorAPI = api;
                    financialAccountSelectorPromiseDeferred.resolve();
                };

                UtilsService.waitMultiplePromises([financialAccountSelectorPromiseDeferred.promise]).then(function () {
                    defineAPI();
                });
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        if (payload.extendedSettings != undefined)
                        {
                            accountBEDefinitionId = payload.extendedSettings.AccountBEDefinitionId;
                        }
                        if (payload.filter != undefined)
                        {
                            status = payload.filter.Status;
                            effectiveDate = payload.filter.EffectiveDate;
                            isEffectiveInFuture = payload.filter.IsEffectiveInFuture;
                        }
                        selectedIds = payload.selectedIds;
                       
                    }

                    var promises = [];

                    promises.push(loadFinancialAccountSelector());

                    function loadFinancialAccountSelector() {
                        var financialAccountPayload = {
                            AccountBEDefinitionId: accountBEDefinitionId,
                            selectedIds: selectedIds,
                            status : status,
                            effectiveDate :effectiveDate,
                            isEffectiveInFuture : isEffectiveInFuture,
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