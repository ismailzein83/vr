'use strict';

app.directive('retailBeAccountbalancenotificationtypeSearcheditor', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AccountBalanceNotificationTypeSearchEditorCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/AccountBalanceNotification/Templates/AccountBalanceNotificationTypeSearchEditorTemplate.html'
        };

        function AccountBalanceNotificationTypeSearchEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var financialAccountSelectorDirectiveAPI;
            var financialAccountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onFinancialAccountSelectorReady = function (api) {
                    financialAccountSelectorDirectiveAPI = api;
                    financialAccountSelectorReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var accountBalanceNotificationTypeExtendedSettings;

                    if (payload != undefined) {
                        accountBalanceNotificationTypeExtendedSettings = payload.accountBalanceNotificationTypeExtendedSettings;
                    }

                    //Loading Account Selector
                    var financialAccountSelectorLoadPromise = getFinancialAccountSelectorLoadPromise();
                    promises.push(financialAccountSelectorLoadPromise);


                    function getFinancialAccountSelectorLoadPromise() {
                        var financialAccountSelectorLoadPromise = UtilsService.createPromiseDeferred();

                        financialAccountSelectorReadyPromiseDeferred.promise.then(function () {

                            var financialAccountSelectorPayload;
                            if (accountBalanceNotificationTypeExtendedSettings != undefined) {
                                financialAccountSelectorPayload = {
                                    AccountBEDefinitionId: accountBalanceNotificationTypeExtendedSettings.AccountBEDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(financialAccountSelectorDirectiveAPI, financialAccountSelectorPayload, financialAccountSelectorLoadPromise);
                        });

                        return financialAccountSelectorLoadPromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.RetailAccountBalanceNotificationExtendedQuery, Retail.BusinessEntity.Business",
                        AccountIds: financialAccountSelectorDirectiveAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
