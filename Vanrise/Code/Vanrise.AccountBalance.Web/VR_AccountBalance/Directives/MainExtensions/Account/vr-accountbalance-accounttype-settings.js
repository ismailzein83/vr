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

            var accountTypeSettingsAPI;
            var accountTypeSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            var accountUsagePeriodSettingsAPI;
            var accountUsagePeriodSettingsReadyDeferred = UtilsService.createPromiseDeferred();

            var billingTransactionTypeSelectorAPI;
            var billingTransactionTypeReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.selectedBusinessEntity;

                $scope.scopeModel.accountTypeSettingsReady = function (api) {
                    accountTypeSettingsAPI = api;
                    accountTypeSettingsReadyDeferred.resolve();
                };
                $scope.scopeModel.accountUsagePeriodSettingsReady = function (api) {
                    accountUsagePeriodSettingsAPI = api;
                    accountUsagePeriodSettingsReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};
                api.load = function (payload) {
                    $scope.scopeModel.isLoading = true;
                    if (payload != undefined) {
                        accountTypeEntity = payload.componentType;
                        if (accountTypeEntity != undefined && accountTypeEntity.Settings != undefined)
                        {
                            $scope.scopeModel.timeOffset = accountTypeEntity.Settings.TimeOffset;
                        }

                    }
                    return UtilsService.waitMultipleAsyncOperations([ loadAccountTypeSettings, loadAllControls, loadAccountUsagePeriodSettings]).catch(function (error) {
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
                    ExtendedSettings: accountTypeSettingsAPI.getData(),
                    BalancePeriodSettings: getBalancePeriodSettings(),
                    AccountUsagePeriodSettings: accountUsagePeriodSettingsAPI.getData(),
                    TimeOffset:$scope.scopeModel.timeOffset
                };
            }

            function loadAllControls() {
                if (accountTypeEntity != undefined) {
                    $scope.scopeModel.name = accountTypeEntity.Name;
                }
            }

            function loadAccountTypeSettings() {
                var accountTypeSettingsDeferred = UtilsService.createPromiseDeferred();
                accountTypeSettingsReadyDeferred.promise.then(function () {
                    var accountTypeSettingsPayload;
                    if (accountTypeEntity != undefined) {
                        accountTypeSettingsPayload = { extendedSettingsEntity: accountTypeEntity.Settings.ExtendedSettings };
                 
                    }
                    VRUIUtilsService.callDirectiveLoad(accountTypeSettingsAPI, accountTypeSettingsPayload, accountTypeSettingsDeferred);
                });
                return accountTypeSettingsDeferred.promises;
            }
            function loadAccountUsagePeriodSettings() {
                var accountUsagePeriodSettingsDeferred = UtilsService.createPromiseDeferred();
                accountUsagePeriodSettingsReadyDeferred.promise.then(function () {
                    var accountUsagePeriodSettingsPayload;
                    if (accountTypeEntity != undefined) {
                        accountUsagePeriodSettingsPayload = { periodSettingsEntity: accountTypeEntity.Settings.AccountUsagePeriodSettings };

                    }
                    VRUIUtilsService.callDirectiveLoad(accountUsagePeriodSettingsAPI, accountUsagePeriodSettingsPayload, accountUsagePeriodSettingsDeferred);
                });
                return accountUsagePeriodSettingsDeferred.promises;
            }

            function getBalancePeriodSettings() {
                return {
                    $type: " Vanrise.AccountBalance.MainExtensions.BalancePeriod.MonthlyBalancePeriodSettings,  Vanrise.AccountBalance.MainExtensions",
                    DayOfMonth: 1
                };
            }
        }
    }]);
