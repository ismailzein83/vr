(function (appControllers) {

    "use strict";

    packageEditorController.$inject = ['$scope', 'Retail_BE_PackageAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService','Retail_BE_PackageDefinitionAPIService' ];

    function packageEditorController($scope, Retail_BE_PackageAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_PackageDefinitionAPIService) {

        var isEditMode;

        var packageId;
        var packageEntity;
        var extendedSettingsEditorRuntime;
        var packageDefinition;

        var packageDefinitionSelectorAPI;
        var packageDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var packageDefinitionSelectionChangedDeferred;

        var packageExtendedSettingsDirectiveAPI;
        var packageExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

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
                packageExtendedSettingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onPackageDefinitionsSelectionChanged = function (selectedItem) {
                if (selectedItem != undefined) {
                    if (packageDefinitionSelectionChangedDeferred != undefined)
                        packageDefinitionSelectionChangedDeferred.resolve();
                    else
                    {
                        getPackageDefinition(selectedItem.PackageDefinitionId).then(function (response) {
                            if (response != undefined && response.Settings != undefined) {
                                packageExtendedSettingsDirectiveReadyDeferred.promise.then(function () {
                                    var setLoader = function (value) {
                                        $scope.scopeModel.isPackageExtendedSettingsDirectiveLoading = value;
                                    };
                                    var payload = { context: getContext(), extendedSettingsDefinition: response.Settings.ExtendedSettings };
                                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, packageExtendedSettingsDirectiveAPI, payload, setLoader);
                                });
                            }
                        });
                    }
                }
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
                packageDefinition = packageEditorRuntimeObj.PackageDefinition;
            });
        }
        function getPackageDefinition(packageDefinitionId)
        {
           return Retail_BE_PackageDefinitionAPIService.GetPackageDefinition(packageDefinitionId);
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticSection, loadPackageDefinitionsSelector, loadPackageExtendedSettingsDirectiveWrapper])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
              .finally(function () {
                  $scope.isLoading = false;
              });
        }
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
            if (packageEntity != undefined)
                packageDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

            var packageDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            packageDefinitionSelectorReadyDeferred.promise.then(function () {
                var packageDefinitionPayload = {
                    filter: {
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.PackageDefinitionAddOrEditFilter, Retail.BusinessEntity.Business",
                            EditMode: isEditMode
                        }]
                    }
                };
                if (packageEntity != undefined && packageEntity.Settings != undefined) {
                    packageDefinitionPayload.selectedIds = packageEntity.Settings.PackageDefinitionId;
                }
                VRUIUtilsService.callDirectiveLoad(packageDefinitionSelectorAPI, packageDefinitionPayload, packageDefinitionSelectorLoadDeferred);
            });

            return packageDefinitionSelectorLoadDeferred.promise;
        }
        function loadPackageExtendedSettingsDirectiveWrapper() {
            if (!isEditMode)
                return;

            var packageExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([packageExtendedSettingsDirectiveReadyDeferred.promise, packageDefinitionSelectionChangedDeferred.promise]).then(function () {
                packageDefinitionSelectionChangedDeferred = undefined;
                var packageExtendedSettingsDirectivePayload = { context: getContext() };
                if (packageEntity != undefined && packageEntity.Settings != undefined && packageEntity.Settings.ExtendedSettings) {
                    packageExtendedSettingsDirectivePayload.extendedSettings = packageEntity.Settings.ExtendedSettings;
                    packageExtendedSettingsDirectivePayload.extendedSettingsEditorRuntime = extendedSettingsEditorRuntime;
                }

                if (packageDefinition != undefined && packageDefinition.Settings != undefined)
                {
                    packageExtendedSettingsDirectivePayload.extendedSettingsDefinition = packageDefinition.Settings.ExtendedSettings;
                }
                VRUIUtilsService.callDirectiveLoad(packageExtendedSettingsDirectiveAPI, packageExtendedSettingsDirectivePayload, packageExtendedSettingsDirectiveLoadDeferred);
            });

            return packageExtendedSettingsDirectiveLoadDeferred.promise;
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
            var context = {
                getServiceTypeFilter: function () {
                    return {
                        $type: "Retail.BusinessEntity.Business.PackageDefinitionServiceTypeFilter,Retail.BusinessEntity.Business",
                        PackageDefinitionId: $scope.scopeModel.selectedPackageDefinition.PackageDefinitionId
                    };
                },
            };
            return context;
        }
    }

    appControllers.controller('Retail_BE_PackageEditorController', packageEditorController);

})(appControllers);
