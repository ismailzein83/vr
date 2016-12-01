(function (appControllers) {

    'use strict';

    PackageItemEditorController.$inject = ['$scope','UtilsService','VRUIUtilsService','VRNavigationService','VRNotificationService','Retail_BE_PackageAPIService'];

    function PackageItemEditorController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService, Retail_BE_PackageAPIService) {
        var isEditMode;

        var packageItemEntity;
        var selectorAPI;
        var selectorReadyDeferred = UtilsService.createPromiseDeferred();

        var serviceTypeSelectorAPI;
        var serviceTypeSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var context;
        var directiveAPI;
        var directiveReadyDeferred;
        $scope.scopeModel = {};
        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined) {
                packageItemEntity = parameters.packageItem;
                    context =parameters.context;
            }
            if (packageItemEntity != undefined) {
                isEditMode = true;
                $scope.scopeModel.isServiceTypeSelectorDisabled = true;
            }
            else {
                isEditMode = false;
                $scope.scopeModel.isServiceTypeSelectorDisabled = false;
            }
        }
        function defineScope() {
         
            
            $scope.scopeModel.templateConfigs = [];
            
            $scope.scopeModel.onServiceTypeSelectorReady = function (api) {
                serviceTypeSelectorAPI = api;
                serviceTypeSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onServiceTypeSelectorChanged = function () {
                var selectedId = serviceTypeSelectorAPI.getSelectedIds();
                if (selectedId == undefined)
                    return;
                if (packageItemEntity != undefined)
                    return;
                loadPackageItemTemplateConfigs();
            };

            $scope.scopeModel.selectedTemplateConfig;

            $scope.scopeModel.onSelectorReady = function (api) {
                selectorAPI = api;
                selectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onDirectiveReady = function (api) {
                directiveAPI = api;
                var setLoader = function (value) {
                    $scope.scopeModel.isLoadingDirective = value;
                };
                var payload = { serviceTypeId: serviceTypeSelectorAPI.getSelectedIds() };
                VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, directiveAPI, payload, setLoader, directiveReadyDeferred);
            };

            $scope.scopeModel.savePackageItem = function () {
                return (isEditMode) ? updatePackageItem() : insertPackageItem();
            };

            $scope.scopeModel.validateServiceTypeSelection = function () {

                if (context != undefined && $scope.scopeModel.selectedServiceType != undefined && context.checkIfServiceTypeUsed($scope.scopeModel.selectedServiceType.ServiceTypeId))
                    return "Same service type already selected.";
                return null;
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadServiceTypeSelector, loadDirective, loadPackageItemDirective]).catch(function(error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function setTitle() {
            if (isEditMode) {
                var packageItemTitle = (packageItemEntity != undefined) ? packageItemEntity.serviceTypeTitle : undefined;
                $scope.title = UtilsService.buildTitleForUpdateEditor(packageItemTitle, 'Package Item');
            }
            else {
                $scope.title = UtilsService.buildTitleForAddEditor('Package Item');
            }
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

        function loadPackageItemDirective() {
            if (packageItemEntity == undefined)
                return;
           return  loadPackageItemTemplateConfigs();
        }
        function loadPackageItemTemplateConfigs() {
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
        function loadDirective() {
            if (packageItemEntity == undefined)
                return;
            directiveReadyDeferred = UtilsService.createPromiseDeferred();
            var loadDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
            directiveReadyDeferred.promise.then(function () {
                directiveReadyDeferred = undefined;
                var payloadDirective = { serviceTypeId: packageItemEntity.ServiceTypeId, settings: packageItemEntity.Settings};
                VRUIUtilsService.callDirectiveLoad(directiveAPI, payloadDirective, loadDirectivePromiseDeferred);
            });

            return loadDirectivePromiseDeferred.promise;
        }
        
        function insertPackageItem() {
            if ($scope.onPackageItemAdded != undefined)
                $scope.onPackageItemAdded(buildPackageItemObjFromScope());
            $scope.modalContext.closeModal();
        }
        function updatePackageItem() {
            if ($scope.onPackageItemUpdated != undefined)
                $scope.onPackageItemUpdated(buildPackageItemObjFromScope());
            $scope.modalContext.closeModal();
        }
        function buildPackageItemObjFromScope() {
            var settings = directiveAPI.getData();
            settings.ConfigId = $scope.scopeModel.selectedTemplateConfig.ExtensionConfigurationId;
            return {
                ServiceTypeId: serviceTypeSelectorAPI.getSelectedIds(),
                serviceTypeTitle: $scope.scopeModel.selectedServiceType.Title,
                Settings: settings
            };
        }
    }

    appControllers.controller('Retail_BE_PackageItemEditorController', PackageItemEditorController);

})(appControllers);