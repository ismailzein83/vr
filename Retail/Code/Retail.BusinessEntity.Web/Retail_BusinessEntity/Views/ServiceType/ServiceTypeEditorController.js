(function (appControllers) {

    'use strict';

    ServiceTypeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_ServiceTypeAPIService', 'Retail_BE_EntityTypeEnum'];

    function ServiceTypeEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_ServiceTypeAPIService, Retail_BE_EntityTypeEnum) {
        var isEditMode;

        var serviceTypeId;
        var serviceTypeEntity;

        var chargingPolicyAPI;
        var chargingPolicyReadyDeferred = UtilsService.createPromiseDeferred();

        var ruleDefinitionSelectorAPI;
        var ruleDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var statusDefinitionSelectorAPI;
        var statusDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

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

            $scope.scopeModel.onChargingPolicyReady = function(api)
            {
                chargingPolicyAPI = api;
                chargingPolicyReadyDeferred.resolve();
            }
            $scope.scopeModel.onStatusDefinitionSelectorReady = function (api) {
                statusDefinitionSelectorAPI = api;
                statusDefinitionSelectorReadyDeferred.resolve();
            }
            $scope.scopeModel.onRuleDefinitionSelectorReady = function(api)
            {
                ruleDefinitionSelectorAPI = api;
                ruleDefinitionSelectorReadyDeferred.resolve();
            }

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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadChargingPolicy, loadRuleDefinitionSelector, loadStatusDefinitionSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
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
        function loadChargingPolicy() {
            var chargingPolicyLoadDeferred = UtilsService.createPromiseDeferred();

            chargingPolicyReadyDeferred.promise.then(function () {
                var chargingPolicyPayload;

                if (serviceTypeEntity != undefined && serviceTypeEntity.Settings !=undefined) {
                    chargingPolicyPayload = { chargingPolicy: serviceTypeEntity.Settings.ChargingPolicyDefinitionSettings }
                }

                VRUIUtilsService.callDirectiveLoad(chargingPolicyAPI, chargingPolicyPayload, chargingPolicyLoadDeferred);
            });

            return chargingPolicyLoadDeferred.promise;
        }
        function loadRuleDefinitionSelector()
        {
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
                IdentificationRuleDefinitionId:ruleDefinitionSelectorAPI.getSelectedIds(),
                ChargingPolicyDefinitionSettings: chargingPolicyAPI.getData(),
                InitialStatusId: statusDefinitionSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_ServiceTypeEditorController', ServiceTypeEditorController);

})(appControllers);