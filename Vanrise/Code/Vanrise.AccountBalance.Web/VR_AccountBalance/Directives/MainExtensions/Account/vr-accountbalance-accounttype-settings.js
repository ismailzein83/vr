'use strict';

app.directive('vrAccountbalanceAccounttypeSettings', ['UtilsService', 'VRUIUtilsService', 'VR_AccountBalance_AccountTypeAPIService',
function (UtilsService, VRUIUtilsService, VR_AccountBalance_AccountTypeAPIService) {
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

        var relationDefinitionSelectorAPI;
        var relationDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountTypeSourcesAPI;
        var accountTypeSourcesReadyDeferred = UtilsService.createPromiseDeferred();

        var accountTypeGridColumnsAPI;
        var accountTypeGridColumnsReadyDeferred = UtilsService.createPromiseDeferred();

        var accountUsagePeriodSettingsAPI;
        var accountUsagePeriodSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var billingTransactionTypeSelectorAPI;
        var billingTransactionTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var viewPermissionAPI;
        var viewPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var addPermissionAPI;
        var addPermissionReadyDeferred = UtilsService.createPromiseDeferred();

        var fieldsBySourceId;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.selectedBusinessEntity;

            $scope.scopeModel.relationDefinitionSelectorReady = function (api) {
                relationDefinitionSelectorAPI = api;
                relationDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.accountTypeSourcesReady = function (api) {
                accountTypeSourcesAPI = api;
                accountTypeSourcesReadyDeferred.resolve();
            };
            $scope.scopeModel.accountTypeGridColumnsReady = function (api) {
                accountTypeGridColumnsAPI = api;
                accountTypeGridColumnsReadyDeferred.resolve();
            };
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
            $scope.scopeModel.onBillingTransactionTypeSelectorReady = function (api) {
                billingTransactionTypeSelectorAPI = api;
                billingTransactionTypeSelectorReadyDeferred.resolve();
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                $scope.scopeModel.isLoading = true;
                if (payload != undefined) {
                    accountTypeEntity = payload.componentType;
                    if (accountTypeEntity != undefined && accountTypeEntity.Settings != undefined) {
                        $scope.scopeModel.timeOffset = accountTypeEntity.Settings.TimeOffset;
                        $scope.scopeModel.shouldGroupUsagesByTransactionType = accountTypeEntity.Settings.ShouldGroupUsagesByTransactionType;
                    }

                }
                return UtilsService.waitMultipleAsyncOperations([loadAccountTypeSettings, loadAllControls, loadAccountUsagePeriodSettings, loadRelationDefinitionSelector, loadSourcesFields, loadAccountTypeGridColumns, loadViewRequiredPermission, loadAccountTypeSources, loadAddRequiredPermission, loadBillingTransactionTypeSelector]).catch(function (error) {
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

        function loadAllControls() {
            if (accountTypeEntity != undefined) {
                $scope.scopeModel.name = accountTypeEntity.Name;
            }
        }
        function loadRelationDefinitionSelector() {
            var relationDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
            relationDefinitionSelectorReadyDeferred.promise.then(function () {
                var relationDefinitionSelectorPayload;
                if (accountTypeEntity != undefined) {
                    relationDefinitionSelectorPayload = { selectedIds: accountTypeEntity.Settings.InvToAccBalanceRelationId };
                }
                VRUIUtilsService.callDirectiveLoad(relationDefinitionSelectorAPI, relationDefinitionSelectorPayload, relationDefinitionSelectorLoadDeferred);
            });
            return relationDefinitionSelectorLoadDeferred.promises;
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
        function loadAccountTypeSources() {
            var accountTypeSourcesLoadDeferred = UtilsService.createPromiseDeferred();
            accountTypeSourcesReadyDeferred.promise.then(function () {
                var accountTypeSourcesPayload = { context: getContext() };
                if (accountTypeEntity != undefined) {
                    accountTypeSourcesPayload.sources = accountTypeEntity.Settings.Sources;

                }
                VRUIUtilsService.callDirectiveLoad(accountTypeSourcesAPI, accountTypeSourcesPayload, accountTypeSourcesLoadDeferred);
            });
            return accountTypeSourcesLoadDeferred.promises;
        }
        function loadAccountTypeGridColumns() {
            var accountTypeGridColumnsLoadDeferred = UtilsService.createPromiseDeferred();
            accountTypeGridColumnsReadyDeferred.promise.then(function () {
                var accountTypeGridColumnsPayload = { context: getContext() };
                if (accountTypeEntity != undefined && accountTypeEntity.Settings != undefined && accountTypeEntity.Settings.AccountBalanceGridSettings != undefined) {
                    accountTypeGridColumnsPayload.gridColumns = accountTypeEntity.Settings.AccountBalanceGridSettings.GridColumns;
                }
                VRUIUtilsService.callDirectiveLoad(accountTypeGridColumnsAPI, accountTypeGridColumnsPayload, accountTypeGridColumnsLoadDeferred);
            });
            return accountTypeGridColumnsLoadDeferred.promises;
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
        function loadSourcesFields() {
            if (accountTypeEntity != undefined && accountTypeEntity.Settings != undefined && accountTypeEntity.Settings.Sources != undefined) {
                return VR_AccountBalance_AccountTypeAPIService.GetAccountTypeSourcesFields({ Sources: accountTypeEntity.Settings.Sources, AccountTypeSettings: accountTypeEntity.Settings }).then(function (response) {
                    fieldsBySourceId = response;
                });
            }
        }
        function loadViewRequiredPermission() {
            var viewSettingPermissionLoadDeferred = UtilsService.createPromiseDeferred();
            viewPermissionReadyDeferred.promise.then(function () {
                var dataPayload = {
                    data: accountTypeEntity && accountTypeEntity.Settings && accountTypeEntity.Settings.Security && accountTypeEntity.Settings.Security.ViewRequiredPermission || undefined
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
        function loadBillingTransactionTypeSelector() {
            var billingTransactionTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            billingTransactionTypeSelectorReadyDeferred.promise.then(function () {
                var billingTransactionTypeSelectorPayload;
                if (accountTypeEntity != undefined && accountTypeEntity.Settings != null) {
                    billingTransactionTypeSelectorPayload = {
                        selectedIds: accountTypeEntity.Settings.AllowedBillingTransactionTypeIds
                    };
                }
                VRUIUtilsService.callDirectiveLoad(billingTransactionTypeSelectorAPI, billingTransactionTypeSelectorPayload, billingTransactionTypeSelectorLoadDeferred);
            });

            return billingTransactionTypeSelectorLoadDeferred.promise;
        }

        function GetAccountSettings() {
            return {
                $type: "Vanrise.AccountBalance.Entities.AccountTypeSettings,  Vanrise.AccountBalance.Entities",
                ExtendedSettings: accountTypeSettingsAPI.getData(),
                BalancePeriodSettings: getBalancePeriodSettings(),
                AccountUsagePeriodSettings: accountUsagePeriodSettingsAPI.getData(),
                AccountBalanceGridSettings: {
                    GridColumns: accountTypeGridColumnsAPI.getData()
                },
                Sources: accountTypeSourcesAPI.getData(),
                TimeOffset: $scope.scopeModel.timeOffset,
                Security: {
                    ViewRequiredPermission: viewPermissionAPI.getData(),
                    AddRequiredPermission: addPermissionAPI.getData(),
                },
                InvToAccBalanceRelationId: relationDefinitionSelectorAPI.getSelectedIds(),
                AllowedBillingTransactionTypeIds: billingTransactionTypeSelectorAPI.getSelectedIds(),
                ShouldGroupUsagesByTransactionType: $scope.scopeModel.shouldGroupUsagesByTransactionType
            };
        }
        function getBalancePeriodSettings() {
            return {
                $type: " Vanrise.AccountBalance.MainExtensions.BalancePeriod.MonthlyBalancePeriodSettings,  Vanrise.AccountBalance.MainExtensions",
                DayOfMonth: 1
            };
        }
        function getContext() {
            var context = {
                getSourceFieldsInfo: function (sourceId) {
                    var fieldsInfo = [];
                    var fields = fieldsBySourceId[sourceId];
                    if (fields != undefined) {
                        for (var i = 0; i < fields.length; i++) {
                            var field = fields[i];
                            fieldsInfo.push({
                                FieldName: field.Name,
                                FieldTitle: field.Title
                            });
                        }
                    }
                    return fieldsInfo;
                },
                loadSourceFields: function (source) {
                    var settings = accountTypeSettingsAPI.getData();
                    if (source != undefined && settings != undefined) {
                        var input = {
                            Source: source,
                            AccountTypeSettings: GetAccountSettings()
                        };
                        return VR_AccountBalance_AccountTypeAPIService.GetAccountTypeSourceFields(input).then(function (response) {
                            if (fieldsBySourceId == undefined)
                                fieldsBySourceId = {};
                            fieldsBySourceId[source.AccountBalanceFieldSourceId] = response;
                        });
                    }
                },
                getSourcesInfo: function () {
                    var sourcesInfo = [];
                    var sources = accountTypeSourcesAPI.getData();
                    if (sources != undefined) {
                        for (var i = 0; i < sources.length; i++) {
                            var source = sources[i];
                            sourcesInfo.push({
                                SourceId: source.AccountBalanceFieldSourceId,
                                Name: source.Name
                            });
                        }
                    }
                    return sourcesInfo;
                }
            };
            return context;
        }
    }
}]);
