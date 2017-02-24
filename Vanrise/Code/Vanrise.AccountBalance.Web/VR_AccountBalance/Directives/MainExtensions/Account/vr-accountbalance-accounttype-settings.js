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

            var viewPermissionAPI;
            var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

            var addPermissionAPI;
            var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

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

                $scope.scopeModel.onViewRequiredPermissionReady = function (api) {
                    viewPermissionAPI = api;
                    viewPermissionReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddRequiredPermissionReady = function (api) {
                    addPermissionAPI = api;
                    addPermissionReadyDeferred.resolve();
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
                    return UtilsService.waitMultipleAsyncOperations([loadAccountTypeSettings, loadAllControls, loadAccountUsagePeriodSettings, loadViewRequiredPermission, loadAddRequiredPermission]).catch(function (error) {
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
                    TimeOffset: $scope.scopeModel.timeOffset,
                    Security: {
                        ViewRequiredPermission: viewPermissionAPI.getData(),
                        AddRequiredPermission: addPermissionAPI.getData(),
                    }
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

            function loadViewRequiredPermission() {
                var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                viewPermissionReadyDeferred.promise.then(function () {
                    var dataPayload = {
                        data: accountTypeEntity &&  accountTypeEntity.Settings && accountTypeEntity.Settings.Security && accountTypeEntity.Settings.Security.ViewRequiredPermission || undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(viewPermissionAPI, dataPayload, viewSettingPermissionLoadDeferred);
                });
                return viewSettingPermissionLoadDeferred.promise;
            }


            function loadAddRequiredPermission() {
                var addPermissionLoadDeferred = UtilsService.createPromiseDeferred();
                addPermissionReadyDeferred.promise.then(function () {
                    var dataPayload = {
                        data: accountTypeEntity && accountTypeEntity.Settings && accountTypeEntity.Settings.Security && accountTypeEntity.Settings.Security.AddRequiredPermission || undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(addPermissionAPI, dataPayload, addPermissionLoadDeferred);
                });
                return addPermissionLoadDeferred.promise;
            }

            function getBalancePeriodSettings() {
                return {
                    $type: " Vanrise.AccountBalance.MainExtensions.BalancePeriod.MonthlyBalancePeriodSettings,  Vanrise.AccountBalance.MainExtensions",
                    DayOfMonth: 1
                };
            }


        }
    }]);
