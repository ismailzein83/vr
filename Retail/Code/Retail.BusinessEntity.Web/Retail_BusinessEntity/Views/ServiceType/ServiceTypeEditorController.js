(function (appControllers) {

    'use strict';

    ServiceTypeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_ServiceTypeAPIService', 'Retail_BE_EntityTypeEnum'];

    function ServiceTypeEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_ServiceTypeAPIService, Retail_BE_EntityTypeEnum) {
        var isEditMode;

        var serviceTypeId;
        var serviceTypeEntity;

        var extendedSettingsSelectorAPI;
        var extendedSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var chargingPolicyAPI;
        var chargingPolicyReadyDeferred = UtilsService.createPromiseDeferred();

        var ruleDefinitionSelectorAPI;
        var ruleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var statusDefinitionSelectorAPI;
        var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var extendedSettingsDirectiveAPI;
        var extendedSettingsDirectiveReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                serviceTypeId = parameters.serviceTypeId;
            }

            isEditMode = (serviceTypeId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.extendedSettingsTemplateConfigs = [];
            $scope.scopeModel.selectedExtendedSettingsTemplateConfig;

            $scope.scopeModel.onExtendedSettingsSelectorReady = function (api) {
                extendedSettingsSelectorAPI = api;
                extendedSettingsSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onChargingPolicyReady = function (api) {
                chargingPolicyAPI = api;
                chargingPolicyReadyDeferred.resolve();
            };
            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRuleDefinitionSelectorReady = function (api) {
                ruleDefinitionSelectorAPI = api;
                ruleDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onExtendedSettingsDirectiveReady = function (api) {
                extendedSettingsDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isExtendedSettingsDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, extendedSettingsDirectiveAPI, undefined, setLoader, extendedSettingsDirectiveReadyDeferred);
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateServiceType() : insertServiceType();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getServiceType().then(function () {
                    loadAllControls().finally(function () {
                        serviceTypeEntity = undefined;
                    });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getServiceType() {
            return Retail_BE_ServiceTypeAPIService.GetServiceType(serviceTypeId).then(function (response) {
                serviceTypeEntity = response;
            });
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadExtendedSettingsSelector, loadChargingPolicy,
                                    loadRuleDefinitionSelector, loadStatusDefinitionSelector, loadExtendedSettingsDirectiveWrapper])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function setTitle() {
            if (isEditMode) {
                var serviceTypeTitle = (serviceTypeEntity != undefined) ? serviceTypeEntity.Title : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(serviceTypeTitle, 'ServiceType');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('ServiceType');
            }
        }
        function loadStaticData() {
            if (serviceTypeEntity == undefined)
                return;
            $scope.scopeModel.title = serviceTypeEntity.Title;
            $scope.scopeModel.description = serviceTypeEntity.Settings.Description;

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
                        if (serviceTypeEntity != undefined && serviceTypeEntity.Settings != undefined)
                            extendedSettings = serviceTypeEntity.Settings.ExtendedSettings;

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
                var chargingPolicyPayload;

                if (serviceTypeEntity != undefined && serviceTypeEntity.Settings != undefined) {
                    chargingPolicyPayload = { chargingPolicy: serviceTypeEntity.Settings.ChargingPolicyDefinitionSettings }
                }

                VRUIUtilsService.callDirectiveLoad(chargingPolicyAPI, chargingPolicyPayload, chargingPolicyLoadDeferred);
            });

            return chargingPolicyLoadDeferred.promise;
        }
        function loadRuleDefinitionSelector() {
            var ruleDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

            ruleDefinitionSelectorReadyDeferred.promise.then(function () {
                var ruleDefinitionPayload = { filter: { Filters: [{ $type: "Retail.BusinessEntity.Business.AccountMappingRuleDefinitionFilter,Retail.BusinessEntity.Business" }] } };

                if (serviceTypeEntity != undefined && serviceTypeEntity.Settings != undefined) {
                    ruleDefinitionPayload.selectedIds = serviceTypeEntity.Settings.IdentificationRuleDefinitionId;
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
                    selectedIds: serviceTypeEntity != undefined && serviceTypeEntity.Settings != undefined ? serviceTypeEntity.Settings.InitialStatusId : undefined
                };
                VRUIUtilsService.callDirectiveLoad(statusDefinitionSelectorAPI, statusDefinitionSelectorPayload, statusDefinitionSelectorLoadDeferred);
            });
            return statusDefinitionSelectorLoadDeferred.promise;
        }
        function loadExtendedSettingsDirectiveWrapper() {
            if (!isEditMode || serviceTypeEntity == undefined || serviceTypeEntity.Settings == undefined ||
                serviceTypeEntity.Settings.ExtendedSettings == undefined || serviceTypeEntity.Settings.ExtendedSettings.ConfigId == undefined)
                return;

            extendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            var extendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            extendedSettingsDirectiveReadyDeferred.promise.then(function () {
                extendedSettingsDirectiveReadyDeferred = undefined;

                var extendedSettingsDirectivePayload
                if (serviceTypeEntity != undefined && serviceTypeEntity.Settings != undefined && serviceTypeEntity.Settings.ExtendedSettings) {

                    extendedSettingsDirectivePayload = {
                        extendedSettings: serviceTypeEntity.Settings.ExtendedSettings,
                    };
                }
                VRUIUtilsService.callDirectiveLoad(extendedSettingsDirectiveAPI, extendedSettingsDirectivePayload, extendedSettingsDirectiveLoadDeferred);
            });

            return extendedSettingsDirectiveLoadDeferred.promise;
        }

        function updateServiceType() {
            $scope.scopeModel.isLoading = true;

            var serviceTypeObj = buildUpdateServiceTypeObjFromScope();
            return Retail_BE_ServiceTypeAPIService.UpdateServiceType(serviceTypeObj).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('ServiceType', response, 'Name')) {
                    if ($scope.onServiceTypeUpdated != undefined) {
                        $scope.onServiceTypeUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildUpdateServiceTypeObjFromScope() {

            return {
                ServiceTypeId: serviceTypeId,
                Title: $scope.scopeModel.title,
                Description: $scope.scopeModel.description,
                IdentificationRuleDefinitionId: ruleDefinitionSelectorAPI.getSelectedIds(),
                ChargingPolicyDefinitionSettings: chargingPolicyAPI.getData(),
                InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                ExtendedSettings: extendedSettingsDirectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_ServiceTypeEditorController', ServiceTypeEditorController);

})(appControllers);