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

            var accountSelectorDirectiveAPI;
            var accountSelectorReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onAccountSelectorReady = function (api) {
                    accountSelectorDirectiveAPI = api;
                    accountSelectorReadyPromiseDeferred.resolve();
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
                    var accountSelectorLoadPromise = getAccountSelectorLoadPromise();
                    promises.push(accountSelectorLoadPromise);


                    function getAccountSelectorLoadPromise() {
                        var accountSelectorLoadPromise = UtilsService.createPromiseDeferred();

                        accountSelectorReadyPromiseDeferred.promise.then(function () {

                            var accountSelectorPayload;
                            if (accountBalanceNotificationTypeExtendedSettings != undefined) {
                                accountSelectorPayload = {
                                    AccountBEDefinitionId: accountBalanceNotificationTypeExtendedSettings.AccountBEDefinitionId
                                };
                            }
                            VRUIUtilsService.callDirectiveLoad(accountSelectorDirectiveAPI, accountSelectorPayload, accountSelectorLoadPromise);
                        });

                        return accountSelectorLoadPromise.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.RetailAccountBalanceNotificationExtendedQuery, Retail.BusinessEntity.Business",
                        AccountIds: accountSelectorDirectiveAPI.getSelectedIds()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
