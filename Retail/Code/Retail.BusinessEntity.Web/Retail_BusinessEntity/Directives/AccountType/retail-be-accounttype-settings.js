"use strict";

app.directive("retailBeAccounttypeSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_AccountPartAvailabilityOptionsEnum", "Retail_BE_AccountPartRequiredOptionsEnum",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_AccountPartAvailabilityOptionsEnum, Retail_BE_AccountPartRequiredOptionsEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new AccountTypeSettings($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl:  '/Client/Modules/Retail_BusinessEntity/Directives/AccountType/Templates/AccountTypeSettings.html'
    };

    function AccountTypeSettings($scope, ctrl) {
        var accountTypeSelectorAPI;
        var accountTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var statusDefinitionSelectorAPI;
        var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var partDefinitionSelectorAPI;
        var partDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.datasource = [];
            $scope.scopeModel.accountPartAvailability = UtilsService.getArrayEnum(Retail_BE_AccountPartAvailabilityOptionsEnum);
            $scope.scopeModel.accountPartRequiredOptions = UtilsService.getArrayEnum(Retail_BE_AccountPartRequiredOptionsEnum);
            $scope.scopeModel.onAccountTypeSelectorReady = function (api) {
                accountTypeSelectorAPI = api;
                accountTypeSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onPartDefinitionSelectorReady = function (api) {
                partDefinitionSelectorAPI = api;
                partDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.removeFilter = function (dataItem) {
                var index = UtilsService.getItemIndexByVal($scope.scopeModel.selectedPartDefinitions, dataItem.AccountPartDefinitionId, 'AccountPartDefinitionId');
                $scope.scopeModel.selectedPartDefinitions.splice(index, 1);

                var datasourceIndex = $scope.scopeModel.datasource.indexOf(dataItem);
                $scope.scopeModel.datasource.splice(datasourceIndex, 1);
            };

            $scope.scopeModel.onSelectItem = function (dataItem) {
                addAccountPart(dataItem);
            };
            $scope.scopeModel.onDeselectItem = function (dataItem) {
                var datasourceIndex = UtilsService.getItemIndexByVal($scope.scopeModel.datasource, dataItem.AccountPartDefinitionId, 'AccountPartDefinitionId');
                $scope.scopeModel.datasource.splice(datasourceIndex, 1);
            };
            defineAPI();
        }

        function defineAPI() {
            var api = {};
            var accountTypeSettings;
            var accountBEDefinitionId;
            api.load = function (payload) {
                if (payload != undefined) {
                    accountTypeSettings = payload.accountTypeSettings;
                    accountBEDefinitionId = payload.AccountBEDefinitionId;
                    if (accountBEDefinitionId != undefined)
                    {
                        $scope.scopeModel.showSettings = true;
                    }
                }
                function loadStaticData() {
                    if (accountTypeSettings != undefined) {
                        $scope.scopeModel.canBeRootAccount = accountTypeSettings.CanBeRootAccount;
                        $scope.scopeModel.showConcatenatedName = accountTypeSettings.ShowConcatenatedName;
                    }

                }

                function loadAccountTypeSelector() {
                    var accountTypeSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    accountTypeSelectorReadyDeferred.promise.then(function () {

                        var accountTypeSelectorPayload = {
                            filter: {
                                AccountBEDefinitionId: accountBEDefinitionId
                            }
                        };
                        if (accountTypeSettings != undefined) {
                            accountTypeSelectorPayload.selectedIds = accountTypeSettings.SupportedParentAccountTypeIds;
                        }
                        VRUIUtilsService.callDirectiveLoad(accountTypeSelectorAPI, accountTypeSelectorPayload, accountTypeSelectorLoadDeferred);
                    });

                    return accountTypeSelectorLoadDeferred.promise;
                }

                function loadPartDefinitionSection() {
                    var partDefinitionIds;
                    if (accountTypeSettings != undefined && accountTypeSettings.PartDefinitionSettings!=null) {
                        partDefinitionIds = [];
                        for (var i = 0; i < accountTypeSettings.PartDefinitionSettings.length; i++) {
                            var partDefinitionSetting = accountTypeSettings.PartDefinitionSettings[i];
                            partDefinitionIds.push(partDefinitionSetting.PartDefinitionId);
                        }
                    }
                    var partDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    partDefinitionSelectorReadyDeferred.promise.then(function () {
                        var partDefinitionSelectorPayload = partDefinitionIds != undefined ? { selectedIds: partDefinitionIds } : undefined;
                        VRUIUtilsService.callDirectiveLoad(partDefinitionSelectorAPI, partDefinitionSelectorPayload, partDefinitionSelectorLoadDeferred);
                    });

                    return partDefinitionSelectorLoadDeferred.promise.then(function () {
                        if (accountTypeSettings != undefined && accountTypeSettings.PartDefinitionSettings != undefined) {
                            for (var i = 0; i < accountTypeSettings.PartDefinitionSettings.length; i++) {
                                var selectedPartDefinition = $scope.scopeModel.selectedPartDefinitions[i];
                                var partDefinitionSetting = accountTypeSettings.PartDefinitionSettings[i];
                                addAccountPart(selectedPartDefinition, partDefinitionSetting);
                            }
                        }
                    });
                }

                function loadStatusDefinitionSelector() {
                    var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    statusDefinitionSelectorReadyDeferred.promise.then(function () {
                        var statusDefinitionSelectorPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountBEStatusDefinitionFilter,Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: accountBEDefinitionId
                                }]
                            },
                            selectedIds: accountTypeSettings != undefined ? accountTypeSettings.InitialStatusId : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, statusDefinitionSelectorLoadDeferred);
                    });
                    return statusDefinitionSelectorLoadDeferred.promise;
                }
                return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadAccountTypeSelector, loadPartDefinitionSection, loadStatusDefinitionSelector])
                  .catch(function (error) {
                      VRNotificationService.notifyExceptionWithClose(error, $scope);
                  }).finally(function () {
                     
                  });

            };
            api.getData = function () {
                var partDefinitionSettings;
                if ($scope.scopeModel.datasource.length > 0) {
                    partDefinitionSettings = [];
                    for (var i = 0 ; i < $scope.scopeModel.datasource.length ; i++) {
                        var dataItem = $scope.scopeModel.datasource[i];
                        partDefinitionSettings.push({
                            PartDefinitionId: dataItem.AccountPartDefinitionId,
                            AvailabilitySettings: dataItem.selectedAccountPartAvailability.value,
                            RequiredSettings: dataItem.selectedAccountPartRequiredOptions.value,
                        });
                    }
                }
                var data = {
                    CanBeRootAccount: $scope.scopeModel.canBeRootAccount,
                    SupportedParentAccountTypeIds: accountTypeSelectorAPI.getSelectedIds(),
                    PartDefinitionSettings: partDefinitionSettings,
                    InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                    ShowConcatenatedName: $scope.scopeModel.showConcatenatedName,
                };
                return data;
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
        function addAccountPart(part, payload) {
            var dataItem = {
                AccountPartDefinitionId: part.AccountPartDefinitionId,
                title: part.Title,
                selectedAccountPartAvailability: payload != undefined ? UtilsService.getItemByVal($scope.scopeModel.accountPartAvailability, payload.AvailabilitySettings, "value") : $scope.scopeModel.accountPartAvailability[0],
                selectedAccountPartRequiredOptions: payload != undefined ? UtilsService.getItemByVal($scope.scopeModel.accountPartRequiredOptions, payload.RequiredSettings, "value") : $scope.scopeModel.accountPartRequiredOptions[0]
            };
            $scope.scopeModel.datasource.push(dataItem);
        }
    }




    return directiveDefinitionObject;

}]);