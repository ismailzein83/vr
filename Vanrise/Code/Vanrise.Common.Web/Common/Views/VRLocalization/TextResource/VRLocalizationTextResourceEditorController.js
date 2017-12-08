(function (appControllers) {

    "use strict";

    VRLocalizationTextResourceController.$inject = ['$scope', 'VRCommon_VRLocalizationTextResourceAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function VRLocalizationTextResourceController($scope, VRCommon_VRLocalizationTextResourceAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;

        var vrLocalizationTextResourceId;
        var vrLocalizationTextResourceEntity;

        var moduleId;

        var localizationModuleSelectorAPI;
        var localizationModuleSelectorReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();

        defineScope();

        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                vrLocalizationTextResourceId = parameters.vrLocalizationTextResourceId;
                moduleId = parameters.moduleId;

            }
            isEditMode = (vrLocalizationTextResourceId != undefined);
        }

        function defineScope() {

            $scope.scopeModel = {};

            $scope.saveVRLocalizationTextResource = function () {
                if (isEditMode)
                    return updateVRLocalizationTextResource();
                else
                    return insertVRLocalizationTextResource();
            };


            $scope.scopeModel.onLocalizationModuleSelectorReady = function (api) {
                localizationModuleSelectorAPI = api;
                localizationModuleSelectorReadyDeferred.resolve();
            };

            $scope.close = function () {
                $scope.modalContext.closeModal();
            };
        }

        function load() {

            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getVRLocalizationTextResource().then(function () {
                    loadAllControls()
                        .finally(function () {
                            vrLocalizationTextResourceEntity = undefined;
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

        function getVRLocalizationTextResource() {
            return VRCommon_VRLocalizationTextResourceAPIService.GetVRLocalizationTextResource(vrLocalizationTextResourceId).then(function (entity) {
                vrLocalizationTextResourceEntity = entity;
            });
        }

        function loadAllControls() {

            function setTitle() {
                if (isEditMode)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(vrLocalizationTextResourceEntity.ResourceKey, "Localization TextResource");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("Localization TextResource");
            }

            function loadStaticData() {
                if (vrLocalizationTextResourceEntity != undefined) {
                    $scope.scopeModel.resourceKey = vrLocalizationTextResourceEntity.ResourceKey;
                    if (vrLocalizationTextResourceEntity.Settings != null)
                    $scope.scopeModel.defaultValue = vrLocalizationTextResourceEntity.Settings.DefaultValue;
                }
            }

            function loadLocalizationModuleSelector() {
                var localizationModuleSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                localizationModuleSelectorReadyDeferred.promise.then(function () {
                    var localizationModuleSelectorPayload;

                    if (moduleId != undefined) 
                    {
                        $scope.scopeModel.isSelectorDisabled = true;

                        localizationModuleSelectorPayload = {
                            selectedIds: moduleId
                        };
                    }
                    if (vrLocalizationTextResourceEntity != undefined) {


                        localizationModuleSelectorPayload = {
                            selectedIds: vrLocalizationTextResourceEntity.ModuleId
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(localizationModuleSelectorAPI, localizationModuleSelectorPayload, localizationModuleSelectorLoadDeferred);
                });
                return localizationModuleSelectorLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadLocalizationModuleSelector])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.scopeModel.isLoading = false;
              });

        }

        function buildVRLocalizationTextResourceObjFromScope() {
            return {
                VRLocalizationTextResourceId: vrLocalizationTextResourceId,
                ResourceKey: $scope.scopeModel.resourceKey,
                ModuleId: localizationModuleSelectorAPI.getSelectedIds(),
                Settings: {
                    DefaultValue: $scope.scopeModel.defaultValue
                }
            };
        }

        function insertVRLocalizationTextResource() {

            $scope.scopeModel.isLoading = true;

            var VRLocalizationTextResourceObject = buildVRLocalizationTextResourceObjFromScope();

            return VRCommon_VRLocalizationTextResourceAPIService.AddVRLocalizationTextResource(VRLocalizationTextResourceObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Localization TextResource", response, "ResourceKey")) {
                    if ($scope.onVRLocalizationTextResourceAdded != undefined) {
                        $scope.onVRLocalizationTextResourceAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        }

        function updateVRLocalizationTextResource() {

            $scope.scopeModel.isLoading = true;

            var VRLocalizationTextResourceObject = buildVRLocalizationTextResourceObjFromScope();

            VRCommon_VRLocalizationTextResourceAPIService.UpdateVRLocalizationTextResource(VRLocalizationTextResourceObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Localization TextResource", response, "ResourceKey")) {
                    if ($scope.onVRLocalizationTextResourceUpdated != undefined)
                        $scope.onVRLocalizationTextResourceUpdated(response.UpdatedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
    }
    appControllers.controller('VRCommon_VRLocalizationTextResourceController', VRLocalizationTextResourceController);
})(appControllers);
