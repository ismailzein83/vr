'use strict';

app.directive('whsAccountbalanceNotificationtypeSearcheditor', ['UtilsService', 'VRUIUtilsService',
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
            templateUrl: "/Client/Modules/WhS_AccountBalance/Elements/AccountBalanceNotification/Directives/Templates/AccountBalanceNotificationTypeSearchEditorTemplate.html"
        };

        function AccountBalanceNotificationTypeSearchEditorCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var accountType;

            var accountTypeSelectorAPI;
            var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();
            var accountTypeSelectorSelectionChangedDeferred;

            var financialAccountDirectiveAPI;
            var financialAccountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.hideAccountTypeSelector = false;

                $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                    accountTypeSelectorAPI = api;
                    accountTypeSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onFinancialAccountDirectiveReady = function (api) {
                    financialAccountDirectiveAPI = api;
                    financialAccountDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onAccountTypeSelectorSelectionChanged = function (selectedItem) {

                    if (selectedItem != undefined) {
                        accountType = selectedItem;

                        loadAccountDirective();

                        function loadAccountDirective() {

                            financialAccountDirectiveReadyDeferred.promise.then(function () {

                                var payload = {
                                    accountTypeId: accountType.Id
                                };
                                var setLoader = function (value) {
                                    $scope.scopeModel.isFinancialAccountDirectiveLoading = value;
                                };
                                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, financialAccountDirectiveAPI, payload, setLoader, undefined);
                            });
                        }
                    }
                    else {
                        financialAccountDirectiveReadyDeferred = UtilsService.createPromiseDeferred();
                    }
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

                    //Loading AccountType Selector
                    var accountTypeSelectorLoadPromise = getAccountTypeSelectorLoadPromise();
                    promises.push(accountTypeSelectorLoadPromise);


                    function getAccountTypeSelectorLoadPromise() {
                        var loadAccountTypeSelectorPromiseDeferred = UtilsService.createPromiseDeferred();

                        accountTypeSelectorReadyDeferred.promise.then(function () {

                            var payLoad;
                            VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, payLoad, loadAccountTypeSelectorPromiseDeferred);
                        });

                        return loadAccountTypeSelectorPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var accountTypeId = accountTypeSelectorAPI != undefined ? accountTypeSelectorAPI.getSelectedIds() : undefined;
                    if (accountTypeId != undefined) {
                        var financialAccountData = financialAccountDirectiveAPI != undefined ? financialAccountDirectiveAPI.getData() : undefined;
                        var financialAccountIds = financialAccountData != undefined ? financialAccountData.selectedIds : undefined;
                    }

                    var obj = {
                        $type: "TOne.WhS.AccountBalance.Business.TOneAccountBalanceNotificationExtendedQuery, TOne.WhS.AccountBalance.Business",
                        AccountTypeId: accountTypeId,
                        FinancialAccountIds: financialAccountIds
                    };

                    return obj;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);
