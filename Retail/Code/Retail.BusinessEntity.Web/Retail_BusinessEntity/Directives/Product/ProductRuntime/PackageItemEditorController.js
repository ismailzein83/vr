(function (appControllers) {

    'use strict';

    ProductPackageItemController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ProductPackageItemController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var packageItemEntity;
        var productDefinitionId;
        var excludedPackageIds;

        var packageSelectorAPI;
        var packageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountConditionSelectiveAPI;
        var accountConditionSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            console.log(parameters);

            if (parameters != undefined) {
                packageItemEntity = parameters.packageItem;
                productDefinitionId = parameters.productDefinitionId;
                excludedPackageIds = parameters.excludedPackageIds;
            }

            isEditMode = (packageItemEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};

            $scope.scopeModel.onPackageSelectorReady = function (api) {
                packageSelectorAPI = api;
                packageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountConditionSelectiveReady = function (api) {
                accountConditionSelectiveAPI = api;
                accountConditionSelectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.save = function () {
                if (isEditMode)
                    return update();
                else
                    return insert();
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;
            loadAllControls();
        }

        function loadAllControls() {
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPackageSelector]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((packageItemEntity != undefined) ? packageItemEntity.FieldName : null, 'Package') :
                UtilsService.buildTitleForAddEditor('Package');
        }
        function loadStaticData() {
            if (packageItemEntity == undefined)
                return;
        }
        function loadPackageSelector() {
            var packageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            packageSelectorReadyDeferred.promise.then(function () {

                var packageSelectorPayload = {
                    filter: {
                        //ExcludedPackageIds: excludedPackageIds,
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.ProductDefinitionPackageFilter, Retail.BusinessEntity.Business",
                            ProductDefinitionId: productDefinitionId
                        }]
                    }
                };
                if (packageItemEntity != undefined) {
                    packageSelectorPayload.selectedIds = packageItemEntity.PackageId;
                }
                VRUIUtilsService.callDirectiveLoad(packageSelectorAPI, packageSelectorPayload, packageSelectorLoadDeferred);
            });

            return packageSelectorLoadDeferred.promise;
        }
        function loadAccountConditionSelective() {
            var accountConditionSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountConditionSelectiveReadyDeferred.promise.then(function () {

                var accountConditionSelectivePayload;
                if (packageItemEntity != undefined) {
                    accountConditionSelectivePayload = {
                        beFilter: packageItemEntity.SubAccountsAvailabilityCondition
                    };
                }
                VRUIUtilsService.callDirectiveLoad(accountConditionSelectiveAPI, accountConditionSelectivePayload, accountConditionSelectiveLoadDeferred);
            });

            return accountConditionSelectiveLoadDeferred.promise;
        }

        function insert() {
            var packageItemObject = buildPackageItemObjectFromScope();

            if ($scope.onPackageItemAdded != undefined && typeof ($scope.onPackageItemAdded) == 'function') {
                $scope.onPackageItemAdded(packageItemObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var packageItemObject = buildPackageItemObjectFromScope();

            if ($scope.onPackageItemUpdated != undefined && typeof ($scope.onPackageItemUpdated) == 'function') {
                $scope.onPackageItemUpdated(packageItemObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildPackageItemObjectFromScope() {

            //var accountGenericFieldDefinitionSelectorObj = packageSelectorAPI.getData();

            var selectedPackage = $scope.scopeModel.selectedPackage;

            console.log(selectedPackage);

            return {
                PackageId: selectedPackage != undefined ? selectedPackage.PackageId : undefined,
                PackageName: selectedPackage != undefined ? selectedPackage.Name : undefined
            };
        }
    }

    appControllers.controller('Retail_BE_ProductPackageItemEditorController', ProductPackageItemController);

})(appControllers);