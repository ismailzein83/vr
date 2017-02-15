(function (appControllers) {

    "use strict";

    ProductFamilyEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_ProductFamilyAPIService'];

    function ProductFamilyEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_ProductFamilyAPIService) {

        var isEditMode;

        var productFamilyId;
        var productFamilyEntity;
        var productFamilyEditorRuntime;
        var productDefinitionInfo;

        var productDefinitionSelectorAPI;
        var productDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var productDefinitionSelectionChangedDeferred;

        var productFamilyPackageItemsDirectiveAPI;
        var productFamilyPackageItemsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);

            if (parameters != undefined) {
                productFamilyId = parameters.productFamilyId;
            }

            isEditMode = (productFamilyId != undefined);
        }
        function defineScope() {
            $scope.scopeModel = {};
            $scope.scopeModel.isProductDefinitionSelectorDisabled = false;
            $scope.scopeModel.showPackagesTab = false;

            $scope.scopeModel.onProductDefinitionsSelectorReady = function (api) {
                productDefinitionSelectorAPI = api;
                productDefinitionSelectorReadyDeferred.resolve();
            };

            $scope.scopeModel.onProductFamilyPackageItemsDirectiveReady = function (api) {
                productFamilyPackageItemsDirectiveAPI = api;
                productFamilyPackageItemsDirectiveReadyDeferred.resolve();
            };

            $scope.scopeModel.onProductDefinitionsSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    $scope.scopeModel.isProductDefinitionSelectorDisabled = isEditMode ? true : false;
                    $scope.scopeModel.showRecurringChargeRulesTab = true;
                    $scope.scopeModel.showPackagesTab = true;

                    productDefinitionInfo = selectedItem;

                    if (productDefinitionSelectionChangedDeferred != undefined) {
                        productDefinitionSelectionChangedDeferred.resolve();
                    }
                    else {
                        loadProductFamilyPackageItemsDirective();
                    }

                    function loadProductFamilyPackageItemsDirective() {
                        $scope.scopeModel.isPackagesTabLoading = true;

                        var productFamilyPackageItemsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        productFamilyPackageItemsDirectiveReadyDeferred.promise.then(function () {
                            var productFamilyPackageItemsPayload = {
                                productDefinitionId: productDefinitionInfo.ProductDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(productFamilyPackageItemsDirectiveAPI, productFamilyPackageItemsPayload, productFamilyPackageItemsDirectiveLoadDeferred);
                        });

                        return productFamilyPackageItemsDirectiveLoadDeferred.promise.then(function () {
                            setTimeout(function () {
                                $scope.scopeModel.isPackagesTabLoading = false;
                            });
                        });
                    }
                }
            };

            $scope.scopeModel.save = function () {
                if (isEditMode) {
                    return updateProductFamily();
                }
                else {
                    return insertProductFamily();
                }
            };
            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal()
            };

            $scope.scopeModel.hasSaveProductFamilyPermission = function () {
                if (isEditMode)
                    return Retail_BE_ProductFamilyAPIService.HasUpdateProductFamilyPermission();
                else
                    return Retail_BE_ProductFamilyAPIService.HasAddProductFamilyPermission();
            };
        }
        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getProductFamily().then(function () {
                    loadAllControls()
                        .finally(function () {
                            productFamilyEntity = undefined;
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

        function getProductFamily() {
            return Retail_BE_ProductFamilyAPIService.GetProductFamilyEditorRuntime(productFamilyId).then(function (response) {
                productFamilyEditorRuntime = response;
                productFamilyEntity = response.Entity;
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadProductDefinitionsSelector, loadProductFamilyPackageItemsDirective])
               .catch(function (error) {
                   VRNotificationService.notifyExceptionWithClose(error, $scope);
               })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        }
        function setTitle() {
            $scope.title =
                isEditMode ? UtilsService.buildTitleForUpdateEditor(productFamilyEntity ? productFamilyEntity.Name : undefined, 'ProductFamily') : UtilsService.buildTitleForAddEditor('ProductFamily');
        }
        function loadStaticData() {
            if (productFamilyEntity == undefined)
                return;

            $scope.scopeModel.name = productFamilyEntity.Name;
        }
        function loadProductDefinitionsSelector() {
            if (productFamilyEntity != undefined)
                productDefinitionSelectionChangedDeferred = UtilsService.createPromiseDeferred();

            var productDefinitionSelectorLoadDeferred = UtilsService.createPromiseDeferred();

            productDefinitionSelectorReadyDeferred.promise.then(function () {

                var productDefinitionPayload = {};
                if (productFamilyEntity != undefined && productFamilyEntity.Settings != undefined) {
                    productDefinitionPayload.selectedIds = productFamilyEntity.Settings.ProductDefinitionId;
                }
                VRUIUtilsService.callDirectiveLoad(productDefinitionSelectorAPI, productDefinitionPayload, productDefinitionSelectorLoadDeferred);
            });

            return productDefinitionSelectorLoadDeferred.promise;
        }
        function loadProductFamilyPackageItemsDirective() {
            if (!isEditMode)
                return;

            var productFamilyPackageItemsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([productFamilyPackageItemsDirectiveReadyDeferred.promise, productDefinitionSelectionChangedDeferred.promise]).then(function () {

                var productFamilyPackageItemsPayload = {
                    productDefinitionId: productDefinitionInfo.ProductDefinitionId
                };
                if (productFamilyEntity != undefined && productFamilyEntity.Settings != undefined) {
                    productFamilyPackageItemsPayload.packages = productFamilyEntity.Settings.Packages;
                    productFamilyPackageItemsPayload.packageNameByIds = productFamilyEditorRuntime.PackageNameByIds;
                }
                VRUIUtilsService.callDirectiveLoad(productFamilyPackageItemsDirectiveAPI, productFamilyPackageItemsPayload, productFamilyPackageItemsDirectiveLoadDeferred);
            });

            return productFamilyPackageItemsDirectiveLoadDeferred.promise;
        }


        function insertProductFamily() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_ProductFamilyAPIService.AddProductFamily(buildProductFamilyObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemAdded("ProductFamily", response, "Name")) {
                        if ($scope.onProductFamilyAdded != undefined)
                            $scope.onProductFamilyAdded(response.InsertedObject);
                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }
        function updateProductFamily() {
            $scope.scopeModel.isLoading = true;

            return Retail_BE_ProductFamilyAPIService.UpdateProductFamily(buildProductFamilyObjFromScope())
                .then(function (response) {
                    if (VRNotificationService.notifyOnItemUpdated("ProductFamily", response, "Name")) {
                        if ($scope.onProductFamilyUpdated != undefined)
                            $scope.onProductFamilyUpdated(response.UpdatedObject);

                        $scope.modalContext.closeModal();
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });
        }

        function buildProductFamilyObjFromScope() {
            var obj = {
                ProductFamilyId: productFamilyId,
                Name: $scope.scopeModel.name,
                Settings: {
                    ProductDefinitionId: productDefinitionSelectorAPI.getSelectedIds(),
                    Packages: productFamilyPackageItemsDirectiveAPI.getData()
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ProductFamilyEditorController', ProductFamilyEditorController);

})(appControllers);
