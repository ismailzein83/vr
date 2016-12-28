(function (appControllers) {

    "use strict";

    packageEditorController.$inject = ['$scope', 'Retail_BE_PackageAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function packageEditorController($scope, Retail_BE_PackageAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var packageId;
        var packageEntity;
        var extendedSettingsEditorRuntime;

        var packageDefinitionSelectorAPI;
        var packageDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var packageExtendedSettingsDirectiveAPI;
        var packageExtendedSettingsDirectiveReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                packageId = parameters.PackageId;
            }
            isEditMode = (packageId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.packageExtendedSettingsTemplateConfigs = [];
            $scope.scopeModel.onPackageDefinitionsSelectorReady = function (api) {
                packageDefinitionSelectorAPI = api;
                packageDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onPackageExtendedSettingsDirectiveReady = function (api) {
                packageExtendedSettingsDirectiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isPackageExtendedSettingsDirectiveLoading = value;
                };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, packageExtendedSettingsDirectiveAPI, undefined, setLoader, packageExtendedSettingsDirectiveReadyDeferred);
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return updatePackage();
                }
                else {
                    return insertPackage();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.hasSavePackagePermission = function () {
                if ($scope.scopeModel.isEditMode)
                    return Retail_BE_PackageAPIService.HasUpdatePackagePermission();
                else
                    return Retail_BE_PackageAPIService.HasAddPackagePermission();
            };
        }
        function load() {
            $scope.isLoading = true;

            if (isEditMode) {
                getPackage().then(function () {
                    loadAllControls()
                        .finally(function () {
                            packageEntity = undefined;
                        });
                }).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                    $scope.isLoading = false;
                });
            }
            else {
                loadAllControls();
            }
        }

        function getPackage() {
            return Retail_BE_PackageAPIService.GetPackageEditorRuntime(packageId).then(function (packageEditorRuntimeObj) {
                packageEntity = packageEditorRuntimeObj.Entity;
                extendedSettingsEditorRuntime = packageEditorRuntimeObj.ExtendedSettingsEditorRuntime;
            });
        }

        function loadAllControls() {
            function setTitle() {
                $scope.title =
                    isEditMode ? UtilsService.buildTitleForUpdateEditor(packageEntity ? packageEntity.Name : undefined, 'Package') : UtilsService.buildTitleForAddEditor('Package');
            }
            function loadStaticSection() {
                if (packageEntity != undefined) {
                    $scope.scopeModel.name = packageEntity.Name;
                    $scope.scopeModel.description = packageEntity.Description;
                }
            }
            function loadPackageDefinitionsSelector() {
                var packageDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();
                packageDefinitionSelectorReadyDeferred.promise.then(function () {
                    var packageDefinitionPayload;
                    if (packageEntity != undefined && packageEntity.Settings != undefined) {
                        packageDefinitionPayload = {
                            selectedIds: packageEntity.Settings.PackageDefinitionId,
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(packageDefinitionSelectorAPI, packageDefinitionPayload, packageDefinitionSelectorLoadDeferred);
                });
                return packageDefinitionSelectorLoadDeferred.promise;
            }
            function loadPackageExtendedSettingsDirectiveWrapper() {
                if (!isEditMode)
                    return;

                packageExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

                var packageExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                packageExtendedSettingsDirectiveReadyDeferred.promise.then(function () {
                    packageExtendedSettingsDirectiveReadyDeferred = undefined;

                    var packageExtendedSettingsDirectivePayload;
                    if (packageEntity != undefined && packageEntity.Settings != undefined && packageEntity.Settings.ExtendedSettings) {

                        packageExtendedSettingsDirectivePayload = {
                            extendedSettings: packageEntity.Settings.ExtendedSettings,
                            extendedSettingsEditorRuntime: extendedSettingsEditorRuntime
                        };
                    }
                    VRUIUtilsService.callDirectiveLoad(packageExtendedSettingsDirectiveAPI, packageExtendedSettingsDirectivePayload, packageExtendedSettingsDirectiveLoadDeferred);
                });

                return packageExtendedSettingsDirectiveLoadDeferred.promise;
            }

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadPackageDefinitionsSelector, loadPackageExtendedSettingsDirectiveWrapper])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
     

      


        function insertPackage() {
            $scope.isLoading = true;
            return Retail_BE_PackageAPIService.AddPackage(buildPackageObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("Package", response, "Name")) {
                    if ($scope.onPackageAdded != undefined)
                        $scope.onPackageAdded(response.InsertedObject);
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }
        function updatePackage() {
            $scope.isLoading = true;
            return Retail_BE_PackageAPIService.UpdatePackage(buildPackageObjFromScope())
            .then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("Package", response, "Name")) {
                    if ($scope.onPackageUpdated != undefined)
                        $scope.onPackageUpdated(response.UpdatedObject);

                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function buildPackageObjFromScope() {
            var obj = {
                PackageId: packageId,
                Name: $scope.scopeModel.name,
                Description: $scope.scopeModel.description,
                Settings: {
                    PackageDefinitionId:packageDefinitionSelectorAPI.getSelectedIds(),
                    ExtendedSettings: packageExtendedSettingsDirectiveAPI.getData()
                }
            };

            return obj;
        }


        function getContext()
        {
            var context;
            return context;
        }
    }

    appControllers.controller('Retail_BE_PackageEditorController', packageEditorController);
})(appControllers);
