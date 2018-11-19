﻿(function (appControllers) {

    "use strict";
    vrDynamicAPIEditorController.$inject = ['$scope', 'VRCommon_VRDynamicAPIAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function vrDynamicAPIEditorController($scope, VRCommon_VRDynamicAPIAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var vrDynamicAPIId;
        var vrDynamicAPIModuleId;
        var vrDynamicAPIEntity;
        var vrDynamicAPIMethodsDirectiveAPI;
        var vrDynamicAPIMethodsReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrDynamicAPIId = parameters.vrDynamicAPIId;
                vrDynamicAPIModuleId = parameters.vrDynamicAPIModuleId;
            }
            isEditMode = (vrDynamicAPIId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.onVRDynamicAPIMethodsDirectiveReady = function (api) {

                vrDynamicAPIMethodsDirectiveAPI = api;
                vrDynamicAPIMethodsReadyPromiseDeferred.resolve();
            };

            $scope.scopeModel.saveVRDynamicAPI = function () {

                if (isEditMode)
                    return updateVRDynamicAPI();
                else
                    return insertVRDynamicAPI();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        }

        function load() {

            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getVRDynamicAPI().then(function () {
                    loadAllControls().finally(function () {
                        vrDynamicAPIEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }

            else
                loadAllControls();
        }

        function getVRDynamicAPI() {

            return VRCommon_VRDynamicAPIAPIService.GetVRDynamicAPIById(vrDynamicAPIId).then(function (response) {
                vrDynamicAPIEntity = response;
            });
        }

        function loadAllControls() {

            function setTitle() {

                if (isEditMode && vrDynamicAPIEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrDynamicAPIEntity.Name, "Dynamic API");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Dynamic API");
            }

            function loadStaticData() {

                if (vrDynamicAPIEntity != undefined) {
                    $scope.scopeModel.name = vrDynamicAPIEntity.Name;
                }

            }

            function loadVRDynamicAPIMethodsDirective() {

                var vrDynamicAPIMethodsLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                vrDynamicAPIMethodsReadyPromiseDeferred.promise.then(function (response) {
                    var methodsPayload = {};
                    if (vrDynamicAPIEntity != undefined && vrDynamicAPIEntity.Settings != undefined && vrDynamicAPIEntity.Settings.Methods != undefined)
                        methodsPayload = { Methods: vrDynamicAPIEntity.Settings.Methods };
                    VRUIUtilsService.callDirectiveLoad(vrDynamicAPIMethodsDirectiveAPI, methodsPayload, vrDynamicAPIMethodsLoadPromiseDeferred);
                });

                return vrDynamicAPIMethodsLoadPromiseDeferred.promise;

            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadVRDynamicAPIMethodsDirective])
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildVRDynamicAPIObjectFromScope() {

            var object = {
                VRDynamicAPIId: (vrDynamicAPIId != undefined) ? vrDynamicAPIId : undefined,
                Name: $scope.scopeModel.name,
                ModuleId: vrDynamicAPIModuleId,
                Settings: { Methods: vrDynamicAPIMethodsDirectiveAPI.getData() }
            };

            return object;
        }

        function insertVRDynamicAPI() {

            $scope.scopeModel.isLoading = true;
            var vrDynamicAPIObject = buildVRDynamicAPIObjectFromScope();

            return VRCommon_VRDynamicAPIAPIService.AddVRDynamicAPI(vrDynamicAPIObject)
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Dynamic API", response, "Name")) {
                        if ($scope.onVRDynamicAPIAdded != undefined) {
                            $scope.onVRDynamicAPIAdded(response.InsertedObject);
                        }
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });

        }

        function updateVRDynamicAPI() {

            $scope.scopeModel.isLoading = true;
            var vrDynamicAPIObject = buildVRDynamicAPIObjectFromScope();
            //VRCommon_VRDynamicAPIAPIService.BuildAPIClassController(vrDynamicAPIObject);
            VRCommon_VRDynamicAPIAPIService.UpdateVRDynamicAPI(vrDynamicAPIObject).then(function (response) {

                if (VRNotificationService.notifyOnItemUpdated("Dynamic API", response, "Name")) {
                    if ($scope.onVRDynamicAPIUpdated != undefined) {
                        $scope.onVRDynamicAPIUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            })
                .catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyException(error, $scope);
                })
                .finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

    }

    appControllers.controller('VR_Dynamic_APIEditorController', vrDynamicAPIEditorController);

})(appControllers);