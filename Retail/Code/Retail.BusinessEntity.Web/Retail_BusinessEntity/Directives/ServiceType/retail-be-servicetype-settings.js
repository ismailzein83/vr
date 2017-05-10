"use strict";

app.directive("retailBeServicetypeSettings", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "Retail_BE_ServiceTypeAPIService", "Retail_BE_EntityTypeEnum",
function (UtilsService, VRNotificationService, VRUIUtilsService, Retail_BE_ServiceTypeAPIService, Retail_BE_EntityTypeEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new ServiceTypeSettings($scope, ctrl);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ServiceType/Templates/ServiceTypeSettings.html'
    };

    function ServiceTypeSettings($scope, ctrl) {
        var chargingPolicyAPI;
        var chargingPolicyReadyDeferred = UtilsService.createPromiseDeferred();
        var extendedSettingsDirectiveAPI;
        var extendedSettingsDirectiveReadyDeferred;
        var ruleDefinitionSelectorAPI;
        var ruleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var statusDefinitionSelectorAPI;
        var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var extendedSettingsSelectorAPI;
        var extendedSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};
            $scope.scopeModel.selectedExtendedSettingsTemplateConfig;
            $scope.scopeModel.extendedSettingsTemplateConfigs = [];
            $scope.scopeModel.onChargingPolicyReady = function (api) {
                chargingPolicyAPI = api;
                chargingPolicyReadyDeferred.resolve();
            };
            $scope.scopeModel.onExtendedSettingsDirectiveReady = function (api) {
        
                extendedSettingsDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isExtendedSettingsDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, extendedSettingsDirectiveAPI, undefined, setLoader, extendedSettingsDirectiveReadyDeferred);
            };
            $scope.scopeModel.onRuleDefinitionSelectorReady = function (api) {
                ruleDefinitionSelectorAPI = api;
                ruleDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onExtendedSettingsSelectorReady = function (api) {
                extendedSettingsSelectorAPI = api;
                extendedSettingsSelectorReadyDeferred.resolve();
            };
            defineAPI();
        }

        function defineAPI() {
            var serviceTypeSettings;
            var api = {};
            var accountBEDefinitionId;
            api.load = function (payload) {
                if (payload != undefined) {
                    serviceTypeSettings = payload.serviceTypeSettings;
                    accountBEDefinitionId = payload.accountBEDefinitionId;
                    
                }

                function loadStaticData() {
                    if (serviceTypeSettings == undefined)
                        return;
                    $scope.scopeModel.description = serviceTypeSettings.Description;

                }

                function loadExtendedSettingsSelector() {

                    var extendedSettingsSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    extendedSettingsSelectorReadyDeferred.promise.then(function () {

                        Retail_BE_ServiceTypeAPIService.GetServiceTypeExtendedSettingsTemplateConfigs().then(function (response) {
                            if (response != null) {
                                for (var i = 0; i < response.length; i++) {
                                    $scope.scopeModel.extendedSettingsTemplateConfigs.push(response[i]);
                                }

                                var extendedSettings;
                                if (serviceTypeSettings != undefined)
                                    extendedSettings = serviceTypeSettings.ExtendedSettings;

                                if (extendedSettings != undefined && extendedSettings.ConfigId != null) {
                                    $scope.scopeModel.selectedExtendedSettingsTemplateConfig =
                                        UtilsService.getItemByVal($scope.scopeModel.extendedSettingsTemplateConfigs, extendedSettings.ConfigId, 'ExtensionConfigurationId');
                                }
                               
                            }
                            extendedSettingsSelectorLoadDeferred.resolve();
                        });
                    });

                    return extendedSettingsSelectorLoadDeferred.promise;
                }

                function loadChargingPolicy() {
                    var chargingPolicyLoadDeferred = UtilsService.createPromiseDeferred();

                    chargingPolicyReadyDeferred.promise.then(function () {
                        var chargingPolicyPayload = {};

                        if (serviceTypeSettings!= undefined) {
                            chargingPolicyPayload.chargingPolicy=serviceTypeSettings.ChargingPolicyDefinitionSettings;
                        }

                        VRUIUtilsService.callDirectiveLoad(chargingPolicyAPI, chargingPolicyPayload, chargingPolicyLoadDeferred);
                    });

                    return chargingPolicyLoadDeferred.promise;
                }

                function loadRuleDefinitionSelector() {
                    if (accountBEDefinitionId == undefined)
                        return;

                    var ruleDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

                    UtilsService.waitMultiplePromises([ruleDefinitionSelectorReadyDeferred.promise,]).then(function () {
                        var ruleDefinitionPayload = {
                            filter: {
                                Filters: [{
                                    $type: "Retail.BusinessEntity.Business.AccountMappingRuleDefinitionFilter, Retail.BusinessEntity.Business",
                                    AccountBEDefinitionId: accountBEDefinitionId //"9a427357-cf55-4f33-99f7-745206dee7cd"
                                }]
                            }
                        };
                        if (serviceTypeSettings != undefined) {
                            ruleDefinitionPayload.selectedIds = serviceTypeSettings.IdentificationRuleDefinitionId;
                        }

                        VRUIUtilsService.callDirectiveLoad(ruleDefinitionSelectorAPI, ruleDefinitionPayload, ruleDefinitionLoadDeferred);
                    });

                    return ruleDefinitionLoadDeferred.promise;
                }
                function loadStatusDefinitionSelector() {
                    var statusDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                    statusDefinitionSelectorReadyDeferred.promise.then(function () {
                        var statusDefinitionSelectorPayload = {
                            filter: { EntityType: Retail_BE_EntityTypeEnum.AccountService.value },
                            selectedIds: serviceTypeSettings != undefined ? serviceTypeSettings.InitialStatusId : undefined
                        };
                        VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, statusDefinitionSelectorLoadDeferred);
                    });
                    return statusDefinitionSelectorLoadDeferred.promise;
                }
                function loadExtendedSettingsDirectiveWrapper() {
                    if (serviceTypeSettings == undefined ||
                        serviceTypeSettings.ExtendedSettings == undefined || serviceTypeSettings.ExtendedSettings.ConfigId == undefined)
                        return;
                    extendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

                    var extendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    extendedSettingsDirectiveReadyDeferred.promise.then(function () {
                        extendedSettingsDirectiveReadyDeferred = undefined;
   
                        var extendedSettingsDirectivePayload;
                        if (serviceTypeSettings != undefined && serviceTypeSettings.ExtendedSettings) {

                            extendedSettingsDirectivePayload = {
                                extendedSettings: serviceTypeSettings.ExtendedSettings
                            };
                            
                        }
                        VRUIUtilsService.callDirectiveLoad(extendedSettingsDirectiveAPI, extendedSettingsDirectivePayload, extendedSettingsDirectiveLoadDeferred);
                    });

                    return extendedSettingsDirectiveLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([loadStaticData, loadChargingPolicy, loadExtendedSettingsSelector, loadExtendedSettingsDirectiveWrapper, loadStatusDefinitionSelector, loadRuleDefinitionSelector])
                  .catch(function (error) {
                      VRNotificationService.notifyExceptionWithClose(error, $scope);
                  }).finally(function () {

                  });

            };
            api.getData = function () {
               
                var data = {
                    Description: $scope.scopeModel.description,
                    IdentificationRuleDefinitionId: ruleDefinitionSelectorAPI.getSelectedIds(),
                    ChargingPolicyDefinitionSettings: chargingPolicyAPI.getData(),
                    InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                    ExtendedSettings: extendedSettingsDirectiveAPI.getData()

                };
                return data;
            };
            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
        
    }




    return directiveDefinitionObject;

}]);