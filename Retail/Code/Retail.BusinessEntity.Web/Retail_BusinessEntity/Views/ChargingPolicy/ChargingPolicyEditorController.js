(function (appControllers) {

    'use strict';

    ChargingPolicyEditorController.$inject = ['$scope', 'Retail_BE_ChargingPolicyAPIService', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ChargingPolicyEditorController($scope, Retail_BE_ChargingPolicyAPIService, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {
        var isEditMode;

        var chargingPolicyId;
        var chargingPolicyEntity;

        var serviceTypeSelectorAPI;
        var serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var settingsDirectiveAPI;
        var settingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                chargingPolicyId = parameters.chargingPolicyId;
            }

            if (chargingPolicyId != undefined) {
                isEditMode = true;
                $scope.isServiceTypeSelectorDisabled = true;
            }
            else {
                isEditMode = false;
                $scope.isServiceTypeSelectorDisabled = false;
            }
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeSelectorAPI = api;
                serviceTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onServiceTypeSelectorChanged = function () {
                var selectedId = serviceTypeSelectorAPI.getSelectedIds();

                if (selectedId == undefined)
                    return;
                if (chargingPolicyEntity != undefined)
                    return;
                
                var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();
                var settingsDirectivePayload = {
                    serviceTypeId: selectedId
                };
                $scope.scopeModel.isLoading = true;
                VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
                settingsDirectiveLoadDeferred.promise.finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
            };

            $scope.scopeModel.onSettingsDirectiveReady = function (api) {
                settingsDirectiveAPI = api;
                settingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                return (isEditMode) ? updateChargingPolicy() : insertChargingPolicy();
            };

            $scope.scopeModel.hasSaveChargingPolicyPermission = function () {
                return (isEditMode) ? Retail_BE_ChargingPolicyAPIService.HasUpdateChargingPolicyPermission() : Retail_BE_ChargingPolicyAPIService.HasAddChargingPolicyPermission();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getChargingPolicy().then(function () {
                    loadAllControls().finally(function () {
                        chargingPolicyEntity = undefined;
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

        function getChargingPolicy() {
            return Retail_BE_ChargingPolicyAPIService.GetChargingPolicy(chargingPolicyId).then(function (response) {
                chargingPolicyEntity = response;
            });
        }
        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadServiceTypeSelector, loadSettingsDirective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            if (isEditMode) {
                var chargingPolicyName = (chargingPolicyEntity != undefined) ? chargingPolicyEntity.Name : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(chargingPolicyName, 'Charging Policy');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Charging Policy');
            }
        }
        function loadStaticData() {
            if (chargingPolicyEntity == undefined)
                return;
            $scope.scopeModel.name = chargingPolicyEntity.Name;
        }
        function loadServiceTypeSelector() {
            var serviceSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            serviceTypeSelectorReadyDeferred.promise.then(function () {
                var serviceTypeSelectorPayload;
                if (chargingPolicyEntity != undefined) {
                    serviceTypeSelectorPayload = {
                        selectedIds: chargingPolicyEntity.ServiceTypeId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(serviceTypeSelectorAPI, serviceTypeSelectorPayload, serviceSelectorLoadDeferred);
            });

            return serviceSelectorLoadDeferred.promise;
        }
        function loadSettingsDirective() {
            if (chargingPolicyEntity == undefined)
                return;

            var settingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            settingsDirectiveReadyDeferred.promise.then(function () {
                var settingsDirectivePayload = {
                    serviceTypeId: chargingPolicyEntity.ServiceTypeId,
                    chargingPolicy: chargingPolicyEntity
                };
                VRUIUtilsService.callDirectiveLoad(settingsDirectiveAPI, settingsDirectivePayload, settingsDirectiveLoadDeferred);
            });

            return settingsDirectiveLoadDeferred.promise;
        }

        function insertChargingPolicy() {
            $scope.scopeModel.isLoading = true;
            return Retail_BE_ChargingPolicyAPIService.AddChargingPolicy(buildChargingPolicyObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Charging Policy', response, 'Name')) {
                    if ($scope.onChargingPolicyAdded != undefined)
                        $scope.onChargingPolicyAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function updateChargingPolicy() {
            $scope.scopeModel.isLoading = true;
            return Retail_BE_ChargingPolicyAPIService.UpdateChargingPolicy(buildChargingPolicyObjFromScope()).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Charging Policy', response, 'Name')) {
                    if ($scope.onChargingPolicyUpdated != undefined) {
                        $scope.onChargingPolicyUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function buildChargingPolicyObjFromScope() {
            var obj = {
                ChargingPolicyId: chargingPolicyId,
                Name: $scope.scopeModel.name
            };

            if (!isEditMode) {
                obj.ServiceTypeId = serviceTypeSelectorAPI.getSelectedIds();
            }

            obj.Settings = settingsDirectiveAPI.getData();

            return obj;
        }
    }

    appControllers.controller('Retail_BE_ChargingPolicyEditorController', ChargingPolicyEditorController);

})(appControllers);