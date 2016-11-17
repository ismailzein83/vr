'use strict';

app.directive('vrAccountbalanceAccounttypeSettings', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var accountBalanceAccountTypeSettings = new AccountBalanceAccountTypeSettings($scope, ctrl, $attrs);
                accountBalanceAccountTypeSettings.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_AccountBalance/Directives/MainExtensions/Account/Templates/VRAccountTypeSettings.html'
        };

        function AccountBalanceAccountTypeSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var accountTypeEntity;

            var genericBESelectorAPI;
            var genericBESelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var billingTransactionTypeSelectorAPI;
            var billingTransactionTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.selectedBusinessEntity;

                $scope.scopeModel.genericBusinessEntitySelectorReady = function (api) {
                    genericBESelectorAPI = api;
                    genericBESelectorReadyDeferred.resolve();
                };

                $scope.scopeModel.BillingTransactionTypeSelectorReady = function (api) {
                    billingTransactionTypeSelectorAPI = api;
                    billingTransactionTypeReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;
                    if (payload != undefined) {
                        accountTypeEntity = payload.componentType;
                    }

                    return UtilsService.waitMultipleAsyncOperations([loadBillingTransactionTypeSelector, loadGenericBESelector, loadAllControls]).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    }).finally(function () {
                        $scope.scopeModel.isLoading = false;
                    });
                };

                api.getData = function () {

                    return {
                        Name: $scope.scopeModel.name,
                        Settings: GetAccountSettings(),
                    };
                };
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function GetAccountSettings() {
                return {
                    $type: "Vanrise.AccountBalance.Entities.AccountTypeSettings,  Vanrise.AccountBalance.Entities",
                    AccountBusinessEntityDefinitionId: genericBESelectorAPI.getSelectedIds(),
                    AccountSelector: $scope.scopeModel.AccountSelector,
                    UsageTransactionTypeId: billingTransactionTypeSelectorAPI.getSelectedIds(),
                    BalancePeriodSettings: getBalancePeriodSettings()
                };
            }

            function loadAllControls() {
                if (accountTypeEntity != undefined) {
                    $scope.scopeModel.AccountSelector = accountTypeEntity.Settings.AccountSelector;
                    $scope.scopeModel.name = accountTypeEntity.Name;
                }
            }

            function loadGenericBESelector() {
                var GenericBELoadDeferred = UtilsService.createPromiseDeferred();
                genericBESelectorReadyDeferred.promise.then(function () {
                    var selectorPayload;
                    if (accountTypeEntity != undefined) {
                        selectorPayload = {
                            selectedIds: accountTypeEntity.Settings.AccountBusinessEntityDefinitionId
                        }
                    }
                    VRUIUtilsService.callDirectiveLoad(genericBESelectorAPI, selectorPayload, GenericBELoadDeferred);
                });
                return GenericBELoadDeferred.promises;
            }

            function loadBillingTransactionTypeSelector() {
                var billingTransactionTypeLoadDeferred = UtilsService.createPromiseDeferred();
                billingTransactionTypeReadyDeferred.promise.then(function () {
                    var selectorPayload;
                    if (accountTypeEntity != undefined) {
                        selectorPayload = {
                            selectedIds: accountTypeEntity.Settings.UsageTransactionTypeId
                        }
                    }
                    VRUIUtilsService.callDirectiveLoad(billingTransactionTypeSelectorAPI, selectorPayload, billingTransactionTypeLoadDeferred);
                });
                return billingTransactionTypeLoadDeferred.promises;
            }

            function getBalancePeriodSettings() {
                return {
                    $type: " Vanrise.AccountBalance.MainExtensions.BalancePeriod.MonthlyBalancePeriodSettings,  Vanrise.AccountBalance.MainExtensions",
                    DayOfMonth: 1
                };
            }
        }
    }]);
