'use strict';
app.directive('retailInvoiceFinancialaccountSelector', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new carriersCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

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
            return '<retail-be-account-selector isrequired="financialAccountCtrl.isrequired" normal-col-num="{{financialAccountCtrl.normalColNum}}" on-ready="financialAccountCtrl.onDirectiveReady"hideremoveicon></retail-be-account-selector>';
        }

        function carriersCtor(ctrl, $scope, attrs) {

            var directiveReadyAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            var context;
            var extendedSettings;
            function initializeController() {

                ctrl.onDirectiveReady = function (api) {
                    directiveReadyAPI = api;
                    directiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var selectedIds;
                    if (payload != undefined) {
                        context = payload.context;
                        selectedIds = payload.selectedIds;
                        extendedSettings = payload.extendedSettings;
                    }
                    var promises = [];

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = { filter: getAccountSelectorFilter(), AccountBEDefinitionId: extendedSettings.AcountBEDefinitionId };
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