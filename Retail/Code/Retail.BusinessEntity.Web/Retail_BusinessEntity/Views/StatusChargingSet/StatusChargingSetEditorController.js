﻿(function (appControllers) {
    "use strict";

    StatusChargingSetEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_StatusChargingSetAPIService'];

    function StatusChargingSetEditorController($scope, utilsService, vrNotificationService, vrNavigationService, vruiUtilsService, retailBeStatusChargingSetApiService) {
        var isEditMode;
        var statusChargingSetId;
        var statusChargingEntity;
        var entityTypeAPI;
        var entityTypeSelectorReadyDeferred = utilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = vrNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                statusChargingSetId = parameters.statusChargingSetId;
            }
            isEditMode = (statusChargingSetId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return update();
                }
                else {
                    return insert();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onEntityTypeSelectorReady = function (api) {
                entityTypeAPI = api;
                entityTypeSelectorReadyDeferred.resolve();
            }
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                GetStatusChargingSet().then(function () {
                    loadAllControls();
                }).catch(function (error) {
                    vrNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.scopeModel.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function setTitle() {
            if (isEditMode) {
                var statusDefinitionName = (statusChargingEntity != undefined) ? statusChargingEntity.Name : null;
                $scope.title = utilsService.buildTitleForUpdateEditor(statusDefinitionName, 'StatusChargingSet');

            }
            else {
                $scope.title = utilsService.buildTitleForAddEditor('StatusChargingSet');
            }
        }
        function buildStatusChargingSetObjFromScope() {

            var settings = {
                EntityType: entityTypeAPI.getSelectedIds()
            }

            return {
                StatusChargingSetId: statusChargingEntity != undefined ? statusChargingEntity.StatusChargingSetId : undefined,
                Name: $scope.scopeModel.name,
                Settings: settings
            };
        }

        function update() {
            $scope.scopeModel.isLoading = true;
            return retailBeStatusChargingSetApiService.UpdateStatusChargingSet(buildStatusChargingSetObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemUpdated('StatusChargingSet', response, 'Name')) {
                    if ($scope.onStatusChargingSetUpdated != undefined) {
                        $scope.onStatusChargingSetUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function insert() {
            $scope.scopeModel.isLoading = true;
            return retailBeStatusChargingSetApiService.AddStatusChargingSet(buildStatusChargingSetObjFromScope()).then(function (response) {
                if (vrNotificationService.notifyOnItemAdded('StatusChargingSet', response, 'Name')) {
                    if ($scope.onStatusChargingSetAdded != undefined)
                        $scope.onStatusChargingSetAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                vrNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }


        function loadEntityTypeSelector() {

            var entityTypeSelectorLoadDeferred = utilsService.createPromiseDeferred();
            entityTypeSelectorReadyDeferred.promise.then(function () {
                var entityTypeSelectorPayload = null;
                if (isEditMode) {
                    entityTypeSelectorPayload = {
                        selectedIds: statusChargingEntity.Settings.EntityType
                    };
                }
                vruiUtilsService.callDirectiveLoad(entityTypeAPI, entityTypeSelectorPayload, entityTypeSelectorLoadDeferred);
            });
            return entityTypeSelectorLoadDeferred.promise;
        }
        function loadStaticData() {
            if (statusChargingEntity == undefined)
                return;
            $scope.scopeModel.name = statusChargingEntity.Name;
        }
        function loadAllControls() {
            return utilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadEntityTypeSelector]).catch(function (error) {
                vrNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function GetStatusChargingSet() {
            return retailBeStatusChargingSetApiService.GetStatusChargingSet(statusChargingSetId).then(function (response) {
                statusChargingEntity = response;
            });
        }
    }
    appControllers.controller('Retail_BE_StatusChargingSetEditorController', StatusChargingSetEditorController);

})(appControllers);