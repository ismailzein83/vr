(function (appControllers) {

    "use strict";

    ProductEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_ProductAPIService'];

    function ProductEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_ProductAPIService) {

        var isEditMode;

        var productId;
        var productEntity;
        var productEditorRuntime
        var productFamilyInfo;

        var currencySelectorAPI;
        var currencySelectorReadyDeferred = UtilsService.createPromiseDeferred();

        var productFamilySelectorAPI;
        var productFamilySelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var productFamilySelectionChangedDeferred;

        var productExtendedSettingsDirectiveAPI;
        var productExtendedSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        var recurringChargeRuleSetsDirectiveAPI;
        var recurringChargeRuleSetsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        //var packageItemsDirectiveAPI;
        //var packageItemsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();



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
            $scope.scopeModel.isProductFamilySelectorDisabled = false;
            $scope.scopeModel.showRecurringChargeRulesTab = false;
            //$scope.scopeModel.showPackagesTab = false;


            $scope.scopeModel.onCurrencySelectorReady = function (api) {
                currencySelectorAPI = api;
                currencySelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onProductFamilySelectorReady = function (api) {
                productFamilySelectorAPI = api;
                productFamilySelectorReadyDeferred.resolve();
            };
            $scope.scopeModel.onProductExtendedSettingsDirectiveReady = function (api) {
                productExtendedSettingsDirectiveAPI = api;
                productExtendedSettingsDirectiveReadyDeferred.resolve();
            };
            $scope.scopeModel.onRecurringChargeRuleSetsDirectiveReady = function (api) {
                recurringChargeRuleSetsDirectiveAPI = api;
                recurringChargeRuleSetsDirectiveReadyDeferred.resolve();
            };
            //$scope.scopeModel.onPackageItemsDirectiveReady = function (api) {
            //    packageItemsDirectiveAPI = api;
            //    packageItemsDirectiveReadyDeferred.resolve();
            //};

            $scope.scopeModel.onProductFamilySelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    $scope.scopeModel.isProductFamilySelectorDisabled = isEditMode ? true : false;
                    $scope.scopeModel.showRecurringChargeRulesTab = true;
                    $scope.scopeModel.showPackagesTab = true;

                    productFamilyInfo = selectedItem;

                    if (productFamilySelectionChangedDeferred != undefined) {
                        productFamilySelectionChangedDeferred.resolve();
                    }
                    else {
                        loadProductExtendedSettingsDirectiveWrapper();
                        loadRecurringChargeRuleSetsDirective();
                        //loadPackageItemsDirective();
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
                    function loadRecurringChargeRuleSetsDirective() {
                        $scope.scopeModel.isRecurringChargeRulesTabLoading = true;

                        var recurringChargeRuleSetsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        recurringChargeRuleSetsDirectiveReadyDeferred.promise.then(function () {

                            var recurringChargeRuleSetsPayload = {
                                accountBEDefinitionId: productFamilyInfo.AccountBEDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(recurringChargeRuleSetsDirectiveAPI, recurringChargeRuleSetsPayload, recurringChargeRuleSetsDirectiveLoadDeferred);
                        });

                        return recurringChargeRuleSetsDirectiveLoadDeferred.promise.then(function () {
                            setTimeout(function () {
                                $scope.scopeModel.isRecurringChargeRulesTabLoading = false;
                            });
                        });
                    }
                    //function loadPackageItemsDirective() {
                    //    $scope.scopeModel.isPackagesTabLoading = true;

                    //    var packageItemsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                    //    packageItemsDirectiveReadyDeferred.promise.then(function () {
                    //        var packageItemsPayload = {
                    //            productFamilyId: productFamilyInfo.ProductFamilyId
                    //        };
                    //        VRUIUtilsService.callDirectiveLoad(packageItemsDirectiveAPI, packageItemsPayload, packageItemsDirectiveLoadDeferred);
                    //    });

                    //    return packageItemsDirectiveLoadDeferred.promise.then(function () {
                    //        setTimeout(function () {
                    //            $scope.scopeModel.isPackagesTabLoading = false;
                    //        });
                    //    });
                    //}
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

            //$scope.scopeModel.hasSaveProductPermission = function () {
            //    if ($scope.scopeModel.isEditMode)
            //        return Retail_BE_ProductAPIService.HasUpdateProductPermission();
            //    else
            //        return Retail_BE_ProductAPIService.HasAddProductPermission();
            //};
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

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadCurrencySelector, loadProductFamilySelector,
                loadProductExtendedSettingsDirectiveWrapper, loadRecurringChargeRuleSetsDirective])
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
        function loadCurrencySelector() {
            var currencySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            currencySelectorReadyDeferred.promise.then(function () {

                var currencySelectorPayload = {};
                if (productEntity != undefined && productEntity.PricingCurrencyId != undefined) {
                    currencySelectorPayload.selectedIds = productEntity.PricingCurrencyId;
                }
                else {
                    currencySelectorPayload.selectSystemCurrency = true;
                }

                VRUIUtilsService.callDirectiveLoad(currencySelectorAPI, currencySelectorPayload, currencySelectorLoadDeferred);
            });

            return currencySelectorLoadDeferred.promise;
        }
        function loadProductFamilySelector() {
            if (productEntity != undefined)
                productFamilySelectionChangedDeferred = UtilsService.createPromiseDeferred();

            var productFamilySelectorLoadDeferred = UtilsService.createPromiseDeferred();

            productFamilySelectorReadyDeferred.promise.then(function () {

                var productFamilyPayload = {};
                if (productEntity != undefined && productEntity.Settings != undefined) {
                    productFamilyPayload.selectedIds = productEntity.Settings.ProductFamilyId;
                }
                VRUIUtilsService.callDirectiveLoad(productFamilySelectorAPI, productFamilyPayload, productFamilySelectorLoadDeferred);
            });

            return productFamilySelectorLoadDeferred.promise;
        }
        function loadProductExtendedSettingsDirectiveWrapper() {
            if (!isEditMode)
                return;

            $scope.scopeModel.isProductExtendedSettingsDirectiveLoading = true;

            var productExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([productExtendedSettingsDirectiveReadyDeferred.promise, productFamilySelectionChangedDeferred.promise]).then(function () {

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
        function loadRecurringChargeRuleSetsDirective() {
            if (!isEditMode)
                return;

            var recurringChargeRuleSetsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

            UtilsService.waitMultiplePromises([recurringChargeRuleSetsDirectiveReadyDeferred.promise, productFamilySelectionChangedDeferred.promise]).then(function () {

                var recurringChargeRuleSetsPayload = {
                    accountBEDefinitionId: productFamilyInfo.AccountBEDefinitionId
                };
                if (productEntity != undefined && productEntity.Settings != undefined) {
                    recurringChargeRuleSetsPayload.recurringChargeRuleSets = productEntity.Settings.RecurringChargeRuleSets
                }
                VRUIUtilsService.callDirectiveLoad(recurringChargeRuleSetsDirectiveAPI, recurringChargeRuleSetsPayload, recurringChargeRuleSetsDirectiveLoadDeferred);
            });

            return recurringChargeRuleSetsDirectiveLoadDeferred.promise;
        }
        //function loadPackageItemsDirective() {
        //    if (!isEditMode)
        //        return;

        //    var packageItemsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

        //    UtilsService.waitMultiplePromises([packageItemsDirectiveReadyDeferred.promise, productFamilySelectionChangedDeferred.promise]).then(function () {

        //        var packageItemsPayload = {
        //            productFamilyId: productFamilyInfo.ProductFamilyId
        //        };
        //        if (productEntity != undefined && productEntity.Settings != undefined) {
        //            packageItemsPayload.packageNameByIds = productEditorRuntime.PackageNameByIds;
        //            packageItemsPayload.packages = productEntity.Settings.Packages;
        //        }
        //        VRUIUtilsService.callDirectiveLoad(packageItemsDirectiveAPI, packageItemsPayload, packageItemsDirectiveLoadDeferred);
        //    });

        //    return packageItemsDirectiveLoadDeferred.promise;
        //}


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
                    ProductFamilyId: productFamilySelectorAPI.getSelectedIds(),
                    ExtendedSettings: productExtendedSettingsDirectiveAPI.getData(),
                    PricingCurrencyId: currencySelectorAPI.getSelectedIds(),
                    RecurringChargeRuleSets: recurringChargeRuleSetsDirectiveAPI.getData()
                    //Packages: packageItemsDirectiveAPI.getData(),
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ProductEditorController', ProductEditorController);

})(appControllers);
