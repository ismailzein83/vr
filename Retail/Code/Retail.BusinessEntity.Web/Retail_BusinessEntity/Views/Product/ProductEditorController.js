(function (appControllers) {

    "use strict";

    productEditorController.$inject = ['$scope', 'Retail_BE_ProductAPIService', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService'];

    function productEditorController($scope, Retail_BE_ProductAPIService, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService) {

        var isEditMode;

        var productId;
        var productEntity;
        var productEditorRuntime
        var productDefinitionId;

        var productDefinitionSelectorAPI;
        var productDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var productDefinitionSelectionChangedDeferred;

        var recurringChargeRuleSetsDirectiveAPI;
        var recurringChargeRuleSetsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var packageItemsDirectiveAPI;
        var packageItemsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var productExtendedSettingsDirectiveAPI;
        var productExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                productId = parameters.productId;
            }

            isEditMode = (productId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.productExtendedSettingsTemplateConfigs = [];
            $scope.scopeModel.isProductDefinitionSelectorDisabled = false;

            $scope.scopeModel.onProductDefinitionsSelectorReady = function (api) {
                productDefinitionSelectorAPI = api;
                productDefinitionSelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onRecurringChargeRuleSetsDirectiveReady = function (api) {
                recurringChargeRuleSetsDirectiveAPI = api;
                recurringChargeRuleSetsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onPackageItemsDirectiveReady = function (api) {
                packageItemsDirectiveAPI = api;
                packageItemsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onProductExtendedSettingsDirectiveReady = function (api) {
                productExtendedSettingsDirectiveAPI = api;
                productExtendedSettingsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onProductDefinitionsSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    $scope.scopeModel.isProductDefinitionSelectorDisabled = true;
                    productDefinitionId = selectedItem.ProductDefinitionId;

                    if (productDefinitionSelectionChangedDeferred != undefined) {
                        productDefinitionSelectionChangedDeferred.resolve();
                    }
                    else {
                        loadPackageItemsDirective();
                        loadProductExtendedSettingsDirectiveWrapper();
                    }

                    function loadPackageItemsDirective() {

                        $scope.scopeModel.isPackagesTabLoading = true;

                        var packageItemsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        packageItemsDirectiveReadyDeferred.promise.then(function () {
                            var packageItemsPayload = { productDefinitionId: productDefinitionId };
                            VRUIUtilsService.callDirectiveLoad(packageItemsDirectiveAPI, packageItemsPayload, packageItemsDirectiveLoadDeferred);
                        });

                        return packageItemsDirectiveLoadDeferred.promise.then(function () {
                            $scope.scopeModel.isPackagesTabLoading = false;
                        });
                    }
                    function loadProductExtendedSettingsDirectiveWrapper() {

                        $scope.scopeModel.isProductExtendedSettingsDirectiveLoading = true;

                        var productExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        productExtendedSettingsDirectiveReadyDeferred.promise.then(function () {
                            var productExtendedSettingsDirectivePayload;
                            VRUIUtilsService.callDirectiveLoad(productExtendedSettingsDirectiveAPI, productExtendedSettingsDirectivePayload, productExtendedSettingsDirectiveLoadDeferred);
                        });

                        return productExtendedSettingsDirectiveLoadDeferred.promise.then(function () {
                            $scope.scopeModel.isProductExtendedSettingsDirectiveLoading = false;
                        });
                    }
                }
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return updateProduct();
                }
                else {
                    return insertProduct();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.hasSaveProductPermission = function () {
                if ($scope.scopeModel.isEditMode)
                    return Retail_BE_ProductAPIService.HasUpdateProductPermission();
                else
                    return Retail_BE_ProductAPIService.HasAddProductPermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getProduct().then(function () {
                    loadAllControls()
                        .finally(function () {
                            productEntity = undefined;
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

        function getProduct() {
            return Retail_BE_ProductAPIService.GetProductEditorRuntime(productId).then(function (response) {
                productEditorRuntime = response;
                productEntity = productEditorRuntime.Entity;
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadProductDefinitionsSelector, loadRecurringChargeRuleSetsDirective,
                        loadPackageItemsDirective, loadProductExtendedSettingsDirectiveWrapper])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }
        function setTitle() {
            $scope.title =
                isEditMode ? UtilsService.buildTitleForUpdateEditor(productEntity ? productEntity.Name : undefined, 'Product') : UtilsService.buildTitleForAddEditor('Product');
        }
        function loadStaticData() {
            if (productEntity == undefined)
                return;

            $scope.scopeModel.name = productEntity.Name;
        }
        function loadProductDefinitionsSelector() {
            if (productEntity != undefined)
                productDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

            var productDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            productDefinitionSelectorReadyDeferred.promise.then(function () {

                var productDefinitionPayload = {};
                if (productEntity != undefined && productEntity.Settings != undefined) {
                    productDefinitionPayload.selectedIds = productEntity.Settings.ProductDefinitionId;
                }
                VRUIUtilsService.callDirectiveLoad(productDefinitionSelectorAPI, productDefinitionPayload, productDefinitionSelectorLoadDeferred);
            });

            return productDefinitionSelectorLoadDeferred.promise;
        }
        function loadRecurringChargeRuleSetsDirective() {
            var recurringChargeRuleSetsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            recurringChargeRuleSetsDirectiveReadyDeferred.promise.then(function () {

                var recurringChargeRuleSetsPayload;
                if (productEntity != undefined && productEntity.Settings != undefined) {
                    var recurringChargeRuleSetsPayload = {
                        recurringChargeRuleSets: productEntity.Settings.RecurringChargeRuleSets
                    }
                }
                VRUIUtilsService.callDirectiveLoad(recurringChargeRuleSetsDirectiveAPI, recurringChargeRuleSetsPayload, recurringChargeRuleSetsDirectiveLoadDeferred);
            });

            return recurringChargeRuleSetsDirectiveLoadDeferred.promise;
        }
        function loadPackageItemsDirective() {
            if (!isEditMode)
                return;

            $scope.scopeModel.isPackagesTabLoading = true;

            var packageItemsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([packageItemsDirectiveReadyDeferred.promise, productDefinitionSelectionChangedDeferred.promise]).then(function () {

                var packageItemsPayload = { productDefinitionId: productDefinitionId };
                if (productEntity != undefined && productEntity.Settings != undefined) {
                    packageItemsPayload.packageNameByIds = productEditorRuntime.PackageNameByIds;
                    packageItemsPayload.packages = productEntity.Settings.Packages;
                }
                VRUIUtilsService.callDirectiveLoad(packageItemsDirectiveAPI, packageItemsPayload, packageItemsDirectiveLoadDeferred);
            });

            return packageItemsDirectiveLoadDeferred.promise.then(function () {
                $scope.scopeModel.isPackagesTabLoading = false;
            });
        }
        function loadProductExtendedSettingsDirectiveWrapper() {
            if (!isEditMode)
                return;

            $scope.scopeModel.isProductExtendedSettingsDirectiveLoading = true;

            var productExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([productExtendedSettingsDirectiveReadyDeferred.promise, productDefinitionSelectionChangedDeferred.promise]).then(function () {

                var productExtendedSettingsDirectivePayload = {};
                if (productEntity != undefined && productEntity.Settings != undefined && productEntity.Settings.ExtendedSettings) {
                    productExtendedSettingsDirectivePayload.extendedSettings = productEntity.Settings.ExtendedSettings;
                }
                VRUIUtilsService.callDirectiveLoad(productExtendedSettingsDirectiveAPI, productExtendedSettingsDirectivePayload, productExtendedSettingsDirectiveLoadDeferred);
            });

            return productExtendedSettingsDirectiveLoadDeferred.promise.then(function () {
                $scope.scopeModel.isProductExtendedSettingsDirectiveLoading = false;
            });
        }

        function insertProduct() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_ProductAPIService.AddProduct(buildProductObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("Product", response, "Name")) {
                        if ($scope.onProductAdded != undefined)
                            $scope.onProductAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function updateProduct() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_ProductAPIService.UpdateProduct(buildProductObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("Product", response, "Name")) {
                        if ($scope.onProductUpdated != undefined)
                            $scope.onProductUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildProductObjFromScope() {
            var obj = {
                ProductId: productId,
                Name: $scope.scopeModel.name,
                Settings: {
                    ProductDefinitionId: productDefinitionSelectorAPI.getSelectedIds(),
                    RecurringChargeRuleSets: recurringChargeRuleSetsDirectiveAPI.getData(),
                    Packages: packageItemsDirectiveAPI.getData(),
                    ExtendedSettings: productExtendedSettingsDirectiveAPI.getData()
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ProductEditorController', productEditorController);

})(appControllers);
