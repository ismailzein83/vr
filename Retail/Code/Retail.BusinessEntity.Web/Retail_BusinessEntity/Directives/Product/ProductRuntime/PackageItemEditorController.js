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

        var accountChargeEvaluatorSelectiveAPI;
        var accountChargeEvaluatorSelectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                packageItemEntity = parameters.packageItem;
                productDefinitionId = parameters.productDefinitionId;
                excludedPackageIds = parameters.excludedPackageIds;
            }

            isEditMode = (packageItemEntity != undefined);
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
                UtilsService.buildTitleForUpdateEditor((packageItemEntity != undefined) ? packageItemEntity.PackageName : null, 'Package') :
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
                        ExcludedPackageIds: !isEditMode ? excludedPackageIds: undefined,
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
        function loadAccountChargeEvaluatorSelective() {
            var accountChargeEvaluatorSelectiveLoadDeferred = UtilsService.createPromiseDeferred();

            accountChargeEvaluatorSelectiveReadyDeferred.promise.then(function () {

                var accountChargeEvaluatorSelectivePayload;
                if (packageItemEntity != undefined) {
                    accountChargeEvaluatorSelectivePayload = {
                        chargeEvaluator: packageItemEntity.ChargeEvaluator
                    };
                }
                VRUIUtilsService.callDirectiveLoad(accountChargeEvaluatorSelectiveAPI, accountChargeEvaluatorSelectivePayload, accountChargeEvaluatorSelectiveLoadDeferred);
            });

            return accountChargeEvaluatorSelectiveLoadDeferred.promise;
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

            var selectedPackage = $scope.scopeModel.selectedPackage;

            return {
                PackageId: selectedPackage != undefined ? selectedPackage.PackageId : undefined,
                PackageName: selectedPackage != undefined ? selectedPackage.Name : undefined,
                ChargeEvaluator: accountChargeEvaluatorSelectiveAPI.getData()
            };
        }
    }

    appControllers.controller('Retail_BE_ProductPackageItemEditorController', ProductPackageItemController);

})(appControllers);