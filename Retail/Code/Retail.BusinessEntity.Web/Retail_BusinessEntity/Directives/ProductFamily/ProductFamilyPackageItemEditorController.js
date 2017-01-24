(function (appControllers) {

    'use strict';

    ProductFamilyPackageItemController.$inject = ['$scope', 'UtilsService', 'VRUIUtilsService', 'VRNavigationService', 'VRNotificationService'];

    function ProductFamilyPackageItemController($scope, UtilsService, VRUIUtilsService, VRNavigationService, VRNotificationService) {

        var isEditMode;

        var productFamilyPackageItemEntity;
        var productDefinitionId;
        var excludedPackageIds;

        var packageSelectorAPI;
        var packageSelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var accountChargeEvaluatorSelectiveAPI;
        var accountChargeEvaluatorSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                productFamilyPackageItemEntity = parameters.productFamilyPackageItem;
                productDefinitionId = parameters.productDefinitionId;
                excludedPackageIds = parameters.excludedPackageIds;
            }

            isEditMode = (productFamilyPackageItemEntity != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isPackageSelectorDisabled = isEditMode ? true : false;

            $scope.scopeModel.onPackageSelectorReady = function (api) {
                packageSelectorAPI = api;
                packageSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onAccountChargeEvaluatorSelectiveReady = function (api) {
                accountChargeEvaluatorSelectiveAPI = api;
                accountChargeEvaluatorSelectiveReadyDeferred.resolve();
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
            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadPackageSelector, loadAccountChargeEvaluatorSelective]).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }
        function setTitle() {
            $scope.title = (isEditMode) ?
                UtilsService.buildTitleForUpdateEditor((productFamilyPackageItemEntity != undefined) ? productFamilyPackageItemEntity.PackageName : null, 'Package') :
                UtilsService.buildTitleForAddEditor('Package');
        }
        function loadStaticData() {
            if (productFamilyPackageItemEntity == undefined)
                return;
        }
        function loadPackageSelector() {
            var packageSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            packageSelectorReadyDeferred.promise.then(function () {

                var packageSelectorPayload = {
                    filter: {
                        ExcludedPackageIds: !isEditMode ? excludedPackageIds : undefined,
                        Filters: [{
                            $type: "Retail.BusinessEntity.Business.ProductDefinitionPackageFilter, Retail.BusinessEntity.Business",
                            ProductDefinitionId: productDefinitionId
                        }]
                    }
                };
                if (productFamilyPackageItemEntity != undefined) {
                    packageSelectorPayload.selectedIds = productFamilyPackageItemEntity.PackageId;
                }
                VRUIUtilsService.callDirectiveLoad(packageSelectorAPI, packageSelectorPayload, packageSelectorLoadDeferred);
            });

            return packageSelectorLoadDeferred.promise;
        }
        function loadAccountChargeEvaluatorSelective() {
            var accountChargeEvaluatorSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountChargeEvaluatorSelectiveReadyDeferred.promise.then(function () {

                var accountChargeEvaluatorSelectivePayload;
                if (productFamilyPackageItemEntity != undefined) {
                    accountChargeEvaluatorSelectivePayload = {
                        chargeEvaluator: productFamilyPackageItemEntity.ChargeEvaluator
                    };
                }
                VRUIUtilsService.callDirectiveLoad(accountChargeEvaluatorSelectiveAPI, accountChargeEvaluatorSelectivePayload, accountChargeEvaluatorSelectiveLoadDeferred);
            });

            return accountChargeEvaluatorSelectiveLoadDeferred.promise;
        }

        function insert() {
            var productFamilyPackageItemObject = buildProductFamilyPackageItemObjectFromScope();

            if ($scope.onProductFamilyPackageItemAdded != undefined && typeof ($scope.onProductFamilyPackageItemAdded) == 'function') {
                $scope.onProductFamilyPackageItemAdded(productFamilyPackageItemObject);
            }
            $scope.modalContext.closeModal();
        }
        function update() {
            var productFamilyPackageItemObject = buildProductFamilyPackageItemObjectFromScope();

            if ($scope.onProductFamilyPackageItemUpdated != undefined && typeof ($scope.onProductFamilyPackageItemUpdated) == 'function') {
                $scope.onProductFamilyPackageItemUpdated(productFamilyPackageItemObject);
            }
            $scope.modalContext.closeModal();
        }

        function buildProductFamilyPackageItemObjectFromScope() {

            var selectedPackage = $scope.scopeModel.selectedPackage;

            var obj = {
                PackageId: selectedPackage != undefined ? selectedPackage.PackageId : undefined,
                PackageName: selectedPackage != undefined ? selectedPackage.Name : undefined,
                ChargeEvaluator: accountChargeEvaluatorSelectiveAPI.getData()
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ProductFamilyPackageItemEditorController', ProductFamilyPackageItemController);

})(appControllers);