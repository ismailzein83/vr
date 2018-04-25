'use strict';
app.directive('retailBeExtendedsettingsAccountSelector', ['UtilsService', 'VRUIUtilsService',
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
                var ctor = new accountsCtor(ctrl, $scope, $attrs);
                ctor.initializeController();

            },
            controllerAs: 'accountCtrl',
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
            var multipleselection;
            if (attrs.ismultipleselection != undefined) {
                multipleselection = 'ismultipleselection="true"';
            }
            return '<retail-be-account-selector isrequired="accountCtrl.isrequired" normal-col-num="{{accountCtrl.normalColNum}}" ' + multipleselection + ' on-ready="accountCtrl.onDirectiveReady" ></retail-be-account-selector>';
        }

        function accountsCtor(ctrl, $scope, attrs) {

            var directiveReadyAPI;
            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.onDirectiveReady = function (api) {
                    directiveReadyAPI = api;
                    directiveReadyPromiseDeferred.resolve();
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var accountTypeId;
                    var extendedSettings;
                    var selectedIds;

                    if (payload != undefined) {
                        accountTypeId = payload.accountTypeId;
                        extendedSettings = payload.extendedSettings;
                        selectedIds = payload.selectedIds;
                    }

                    var promises = [];

                    var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
                    promises.push(directiveLoadPromiseDeferred.promise);

                    directiveReadyPromiseDeferred.promise.then(function () {
                        var selectorPayload = {
                            filter: getAccountSelectorFilter(),
                            selectedIds: selectedIds
                        };
                        if (extendedSettings != undefined)
                            selectorPayload.AccountBEDefinitionId = extendedSettings.AccountBEDefinitionId;
                        VRUIUtilsService.callDirectiveLoad(directiveReadyAPI, selectorPayload, directiveLoadPromiseDeferred);
                    });

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    var data = directiveReadyAPI.getSelectedIds();
                    return {
                        selectedIds: data,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function getAccountSelectorFilter() {
                var filter = {};

                filter.Filters = [];

                var financialAccounts = {
                    $type: 'Retail.BusinessEntity.Business.AccountBalanceEnabledAccountFilter, Retail.BusinessEntity.Business',
                };
                filter.Filters.push(financialAccounts);
                return filter;
            }
            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }]);