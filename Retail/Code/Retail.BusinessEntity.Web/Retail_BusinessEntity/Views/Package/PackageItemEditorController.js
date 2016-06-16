(function (appControllers) {

    'use strict';

    PackageItemEditorController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService','Retail_BE_PackageAPIService'];

    function PackageItemEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_PackageAPIService) {
        var isEditMode;

        var packageItemEntity;
        var selectorAPI;

        var serviceTypeSelectorAPI;
        var serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var directiveAPI;
        var directiveReadyDeferred;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                packageItemEntity = parameters.packageItem;
            }

            isEditMode = (packageItemEntity != undefined);
        }

        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.templateConfigs = [];
            
            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeSelectorAPI = api;
                serviceTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onServiceTypeSelectorChanged = function () {
                var selectedId = serviceTypeSelectorAPI.getSelectedIds();

                if (selectedId != undefined)
                    return;
                loadPackageItemsTemplateConfigs();
            };


            $scope.scopeModel.selectedTemplateConfig;

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var payload = { serviceTypeId: serviceTypeSelectorAPI.getSelectedIds() }
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.savePackageItem = function () {
                return (isEditMode) ? updatePackageItem() : insertPackageItem();
            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadDirective, loadServiceTypeSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function loadDirective() {
            if (packageItemEntity == undefined)
                return;
            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            directiveReadyDeferred.promise.then(function () {
                var payloadDirective = { packageItem: packageItemEntity.Settings };
                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
            });
            return loadDirectivePromiseDeferred.promise;
        }

        function loadServiceTypeSelector() {
            var serviceSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            serviceTypeSelectorReadyDeferred.promise.then(function () {
                var serviceTypeSelectorPayload;
                if (packageItemEntity != undefined) {
                    serviceTypeSelectorPayload = {
                        selectedIds: packageItemEntity.ServiceTypeId
                    };
                }
                VRUIUtilsService.callDirectiveLoad(serviceTypeSelectorAPI, serviceTypeSelectorPayload, serviceSelectorLoadDeferred);
            });

            return serviceSelectorLoadDeferred.promise;
        }

        function loadPackageItemsTemplateConfigs() {
            return Retail_BE_PackageAPIService.GetServicePackageItemConfigs().then(function (response) {
                if (selectorAPI != undefined)
                    selectorAPI.clearDataSource();
                if (response != null) {
                    for (var i = 0; i < response.length; i++) {
                        $scope.scopeModel.templateConfigs.push(response[i]);
                    }
                    if (packageItemEntity != undefined && packageItemEntity.Settings !=undefined)
                        $scope.scopeModel.selectedTemplateConfig = UtilsService.getItemByVal($scope.scopeModel.templateConfigs, packageItemEntity.Settings.ConfigId, 'ExtensionConfigurationId');
                }
            });
        }

        function setTitle() {
            if (isEditMode) {
                var packageItemTitle = (packageItemEntity != undefined) ? packageItemEntity.PackageItemTitle : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(packageItemTitle, 'Package Item');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Package Item');
            }
        }

        function updatePackageItem() {
            var packageItemObj = buildPackageItemObjFromScope();

            if ($scope.onPackageItemUpdated != undefined) {
                $scope.onPackageItemUpdated(packageItemObj);
            }
            $scope.modalContext.closeModal();
        }

        function insertPackageItem() {
            var packageItemObj = buildPackageItemObjFromScope();
            if ($scope.onPackageItemAdded != undefined) {
                $scope.onPackageItemAdded(packageItemObj);
            }
            $scope.modalContext.closeModal();
        }

        function buildPackageItemObjFromScope() {
            var packageItem = directiveAPI.getData();
            if (packageItem != undefined)
                packageItem.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;

            var packageItemObj = {
                PackageItem: {
                    Settings :packageItem,
                    ServiceTypeId: serviceTypeSelectorAPI.getSelectedIds()
                    }
            };
            return packageItemObj;
        }
    }

    appControllers.controller('Retail_BE_PackageItemEditorController', PackageItemEditorController);

})(appControllers);