﻿(function (appControllers) {

    'use strict';

    ServiceTypeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_ServiceTypeAPIService', 'Retail_BE_EntityTypeEnum'];

    function ServiceTypeEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_ServiceTypeAPIService, Retail_BE_EntityTypeEnum) {
        var isEditMode;

        var accountBEDefinitionId;
        var serviceTypeId;
        var serviceTypeEntity;

        var extendedSettingsSelectorAPI;
        var extendedSettingsSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var ruleDefinitionSelectorAPI;
        var ruleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var statusDefinitionSelectorAPI;
        var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var serviceTypeSettingsAPI;
        var serviceTypeSettingsReadyDeferred = UtilsService.createPromiseDeferred();

        var businessEntityDefinitionSelectorAPI;
        var businessEntityDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var businessEntityDefinitionSelectionChangedDeferred;

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
            $scope.scopeModel.showGenericRuleDefinitionSelector = false;

            $scope.scopeModel.onExtendedSettingsSelectorReady = function (api) {
                extendedSettingsSelectorAPI = api;
                extendedSettingsSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRuleDefinitionSelectorReady = function (api) {
                ruleDefinitionSelectorAPI = api;
                ruleDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onServiceTypeSettingsDirectiveReady = function (api) {
                serviceTypeSettingsAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isServiceTypeSettingsLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, serviceTypeSettingsAPI, undefined, setLoader, serviceTypeSettingsReadyDeferred);
            };
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                businessEntityDefinitionSelectorAPI = api;
                businessEntityDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    accountBEDefinitionId = selectedItem.BusinessEntityDefinitionId;
                    $scope.scopeModel.showGenericRuleDefinitionSelector = true;

                    var ruleDefinitionPayload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountMappingRuleDefinitionFilter, Retail.BusinessEntity.Business",
                                AccountBEDefinitionId: accountBEDefinitionId
                            }]
                        }
                    };
                    var setLoader = function (value) {
                        $scope.scopeModel.isGenericRuleDefinitionSelectorLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, ruleDefinitionSelectorAPI, ruleDefinitionPayload, setLoader, businessEntityDefinitionSelectionChangedDeferred);
                }
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
           
            function loadRuleDefinitionSelector() {
                if (!isEditMode || accountBEDefinitionId == undefined)
                    return;

                if (businessEntityDefinitionSelectionChangedDeferred == undefined)
                    businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                var ruleDefinitionLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([ruleDefinitionSelectorReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {
                    businessEntityDefinitionSelectionChangedDeferred = undefined;

                    var ruleDefinitionPayload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountMappingRuleDefinitionFilter, Retail.BusinessEntity.Business",
                                AccountBEDefinitionId: accountBEDefinitionId //"9a427357-cf55-4f33-99f7-745206dee7cd"
                            }]
                        }
                    };
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
            function loadServiceTypeSettings() {
                var serviceTypeSettingsLoadDeferred = UtilsService.createPromiseDeferred();
                serviceTypeSettingsReadyDeferred.promise.then(function () {
                   
                    serviceTypeSettingsReadyDeferred = undefined;

                    var servicetypeSettingsPayload = {};
                    if (serviceTypeEntity != undefined && serviceTypeEntity.Settings != undefined) {
                        servicetypeSettingsPayload.serviceTypeSettings = serviceTypeEntity.Settings;
                    } 
                    VRUIUtilsService.callDirectiveLoad(serviceTypeSettingsAPI, servicetypeSettingsPayload, serviceTypeSettingsLoadDeferred);
                });

                return serviceTypeSettingsLoadDeferred.promise;
            }
            function loadBusinessEntityDefinitionSelector() {
                var businessEntityDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                businessEntityDefinitionSelectorReadyDeferred.promise.then(function () {
                    var payload = {
                        filter: {
                            Filters: [{
                                $type: "Retail.BusinessEntity.Business.AccountBEDefinitionFilter, Retail.BusinessEntity.Business",
                            }]
                        },
                        selectedIds: serviceTypeEntity != undefined ? serviceTypeEntity.AccountBEDefinitionId : undefined
                    };
                    VRUIUtilsService.callDirectiveLoad(businessEntityDefinitionSelectorAPI, payload, businessEntityDefinitionSelectorLoadDeferred);
                });

                return businessEntityDefinitionSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadExtendedSettingsSelector,
                                    loadRuleDefinitionSelector, loadStatusDefinitionSelector, loadServiceTypeSettings, loadBusinessEntityDefinitionSelector])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
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
                ChargingPolicyDefinitionSettings: serviceTypeSettingsAPI.getData().ChargingPolicyDefinitionSettings,
                InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds(),
                ExtendedSettings: serviceTypeSettingsAPI.getData().ExtendedSettings,
                AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_ServiceTypeEditorController', ServiceTypeEditorController);

})(appControllers);