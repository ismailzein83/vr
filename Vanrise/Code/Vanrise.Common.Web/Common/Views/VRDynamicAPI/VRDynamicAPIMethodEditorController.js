﻿(function (appControllers) {

    "use strict";
    vrDynamicAPIMethodEditorController.$inject = ['$scope', 'VRCommon_VRDynamicAPIAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function vrDynamicAPIMethodEditorController($scope, VRCommon_VRDynamicAPIAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var vrDynamicAPIMethodEntity;
        var vrDynamicAPIMethodSettingsDirectiveAPI;
        var vrDynamicAPIMethodSettingsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrDynamicAPIMethodEntity = parameters.vrDynamicAPIMethodEntity;
            }
            isEditMode = (vrDynamicAPIMethodEntity != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveVRDynamicAPIMethod = function () {
                if (isEditMode)
                    return updateVRDynamicAPIMethod();
                else
                    return insertVRDynamicAPIMethod();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onVRDynamicAPIMethodSettingsDirectiveReady = function (api) {

                vrDynamicAPIMethodSettingsDirectiveAPI = api;
                vrDynamicAPIMethodSettingsReadyPromiseDeferred.resolve();
            };
        }

        function load() {

            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                loadAllControls().finally(function () {
                    vrDynamicAPIMethodEntity = undefined;
                });
            }
            else
                loadAllControls();
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && vrDynamicAPIMethodEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrDynamicAPIMethodEntity.Name, "Dynamic API Method");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Dynamic API Method");
            }

            function loadStaticData() {
                if (vrDynamicAPIMethodEntity != undefined) {
                    $scope.scopeModel.name = vrDynamicAPIMethodEntity.Name;
                }
            }

            function loadVRDynamicAPIMethodSettingsDirective() {

                var promises = [];
                var vrDynamicAPIMethodSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                promises.push(vrDynamicAPIMethodSettingsReadyPromiseDeferred.promise);
                UtilsService.waitMultiplePromises(promises).then(function (response) {
                    var settingsPayload = {};
                    if (isEditMode) 
                        settingsPayload.Settings = vrDynamicAPIMethodEntity.Settings;
                    VRUIUtilsService.callDirectiveLoad(vrDynamicAPIMethodSettingsDirectiveAPI, settingsPayload, vrDynamicAPIMethodSettingsDirectiveLoadDeferred);
                });
                return vrDynamicAPIMethodSettingsDirectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRDynamicAPIMethodSettingsDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildVRDynamicAPIMethodObjectFromScope() {

            var object = {
                VRDynamicAPIMethodId: (vrDynamicAPIMethodEntity != undefined) ? vrDynamicAPIMethodEntity.VRDynamicAPIMethodId : undefined,
                Name: $scope.scopeModel.name,
                Settings: vrDynamicAPIMethodSettingsDirectiveAPI.getData()
            };
            return object;
        }

        function insertVRDynamicAPIMethod() {

            $scope.scopeModel.isLoading = true;

            if ($scope.onVRDynamicAPIMethodAdded != undefined) 

                $scope.onVRDynamicAPIMethodAdded(buildVRDynamicAPIMethodObjectFromScope());

            $scope.modalContext.closeModal();

            $scope.scopeModel.isLoading = false;
        }

        function updateVRDynamicAPIMethod() {

            $scope.scopeModel.isLoading = true;

            if ($scope.onVRDynamicAPIMethodUpdated != undefined)

                $scope.onVRDynamicAPIMethodUpdated(buildVRDynamicAPIMethodObjectFromScope());

            $scope.modalContext.closeModal();
            
            $scope.scopeModel.isLoading = false;
        }
     }

    appControllers.controller('VR_Dynamic_API_MethodEditorController', vrDynamicAPIMethodEditorController);
})(appControllers);