(function (appControllers) {

    'use strict';

    ServiceTypeEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService', 'Retail_BE_ServiceTypeAPIService', 'Retail_BE_EntityTypeEnum'];

    function ServiceTypeEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_ServiceTypeAPIService, Retail_BE_EntityTypeEnum) {
        var isEditMode;

        var accountBEDefinitionId;
        var serviceTypeId;
        var serviceTypeEntity;

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
            $scope.scopeModel.showServiceTypeSettingsDirective = false;
            $scope.scopeModel.isServiceTypeSettingsLoading = false;
            $scope.scopeModel.onServiceTypeSettingsDirectiveReady = function (api) {
               
                serviceTypeSettingsAPI = api;
                serviceTypeSettingsReadyDeferred.resolve();
            };
            $scope.scopeModel.onBusinessEntityDefinitionSelectorReady = function (api) {
                businessEntityDefinitionSelectorAPI = api;
                businessEntityDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onBusinessEntityDefinitionSelectionChanged = function (selectedItem) {
                
                if (selectedItem != undefined) {
                    accountBEDefinitionId = selectedItem.BusinessEntityDefinitionId;
                    $scope.scopeModel.showServiceTypeSettingsDirective = true;

                    var servicetypeSettingsPayload = { accountBEDefinitionId: accountBEDefinitionId }
                    var setLoader = function (value) {
                        $scope.scopeModel.isServiceTypeSettingsLoading = value;
                    };
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, serviceTypeSettingsAPI, servicetypeSettingsPayload, setLoader, businessEntityDefinitionSelectionChangedDeferred);
                };
               
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

            }

            function loadServiceTypeSettings() {
                if (!isEditMode)
                    return;
               
                if (businessEntityDefinitionSelectionChangedDeferred == undefined)
                    businessEntityDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

                var serviceTypeSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                UtilsService.waitMultiplePromises([serviceTypeSettingsReadyDeferred.promise, businessEntityDefinitionSelectionChangedDeferred.promise]).then(function () {
                    businessEntityDefinitionSelectionChangedDeferred = undefined;
                    serviceTypeSettingsReadyDeferred = undefined;
                    var servicetypeSettingsPayload = {};
                    servicetypeSettingsPayload.accountBEDefinitionId=accountBEDefinitionId;
                    if (serviceTypeEntity != undefined && serviceTypeEntity.Settings != undefined) {
                        servicetypeSettingsPayload.serviceTypeSettings = serviceTypeEntity.Settings;
                    }
                    
                    VRUIUtilsService.callDirectiveLoad(serviceTypeSettingsAPI, servicetypeSettingsPayload, serviceTypeSettingsLoadDeferred);
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isServiceTypeSettingsLoading = false;
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

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServiceTypeSettings, loadBusinessEntityDefinitionSelector])
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
                Description: serviceTypeSettingsAPI.getData().Description,
                IdentificationRuleDefinitionId: serviceTypeSettingsAPI.getData().IdentificationRuleDefinitionId,
                ChargingPolicyDefinitionSettings: serviceTypeSettingsAPI.getData().ChargingPolicyDefinitionSettings,
                InitialStatusId: serviceTypeSettingsAPI.getData().InitialStatusId,
                ExtendedSettings: serviceTypeSettingsAPI.getData().ExtendedSettings,
                AccountBEDefinitionId: businessEntityDefinitionSelectorAPI.getSelectedIds()
            };
        }
    }

    appControllers.controller('Retail_BE_ServiceTypeEditorController', ServiceTypeEditorController);

})(appControllers);