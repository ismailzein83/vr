(function (appControllers) {

    "use strict";

    ProductFamilyEditorController.$inject = ['$scope', 'UtilsService', 'VRNotificationService', 'VRNavigationService', 'VRUIUtilsService', 'Retail_BE_ProductFamilyAPIService'];

    function ProductFamilyEditorController($scope, UtilsService, VRNotificationService, VRNavigationService, VRUIUtilsService, Retail_BE_ProductFamilyAPIService) {

        var isEditMode;

        var productFamilyId;
        var productFamilyEntity;
        var productFamilyEditorRuntime;
        var productDefinitionInfoEntity;

        var productDefinitionSelectorAPI;
        var productDefinitionSelectorReadyDeferred = UtilsService.createPromiseDeferred();
        var productDefinitionSelectionChangedDeferred;

        //var packageItemsDirectiveAPI;
        //var packageItemsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();


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

            //$scope.scopeModel.onPackageItemsDirectiveReady = function (api) {
            //    packageItemsDirectiveAPI = api;
            //    packageItemsDirectiveReadyDeferred.resolve();
            //};

            $scope.scopeModel.onProductDefinitionsSelectionChanged = function (selectedItem) {

                if (selectedItem != undefined) {
                    $scope.scopeModel.isProductDefinitionSelectorDisabled = isEditMode ? true : false;
                    $scope.scopeModel.showRecurringChargeRulesTab = true;
                    $scope.scopeModel.showPackagesTab = true;

                    productDefinitionInfoEntity = selectedItem;

                    if (productDefinitionSelectionChangedDeferred != undefined) {
                        productDefinitionSelectionChangedDeferred.resolve();
                    }
                    else {
                        //loadProductFamilyExtendedSettingsDirectiveWrapper();
                        //loadRecurringChargeRuleSetsDirective();
                        //loadPackageItemsDirective();
                    }

                    function loadProductFamilyExtendedSettingsDirectiveWrapper() {

                        $scope.scopeModel.isProductFamilyExtendedSettingsDirectiveLoading = true;

                        var productFamilyExtendedSettingsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        productFamilyExtendedSettingsDirectiveReadyDeferred.promise.then(function () {
                            var productFamilyExtendedSettingsDirectivePayload;
                            VRUIUtilsService.callDirectiveLoad(productFamilyExtendedSettingsDirectiveAPI, productFamilyExtendedSettingsDirectivePayload, productFamilyExtendedSettingsDirectiveLoadDeferred);
                        });

                        return productFamilyExtendedSettingsDirectiveLoadDeferred.promise.then(function () {
                            $scope.scopeModel.isProductFamilyExtendedSettingsDirectiveLoading = false;
                        });
                    }
                    function loadRecurringChargeRuleSetsDirective() {
                        $scope.scopeModel.isRecurringChargeRulesTabLoading = true;

                        var recurringChargeRuleSetsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        recurringChargeRuleSetsDirectiveReadyDeferred.promise.then(function () {

                            var recurringChargeRuleSetsPayload = {
                                accountBEDefinitionId: productDefinitionInfoEntity.AccountBEDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(recurringChargeRuleSetsDirectiveAPI, recurringChargeRuleSetsPayload, recurringChargeRuleSetsDirectiveLoadDeferred);
                        });

                        return recurringChargeRuleSetsDirectiveLoadDeferred.promise.then(function () {
                            setTimeout(function () {
                                $scope.scopeModel.isRecurringChargeRulesTabLoading = false;
                            });
                        });
                    }
                    function loadPackageItemsDirective() {
                        $scope.scopeModel.isPackagesTabLoading = true;

                        var packageItemsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        packageItemsDirectiveReadyDeferred.promise.then(function () {
                            var packageItemsPayload = {
                                productDefinitionId: productDefinitionInfoEntity.ProductDefinitionId
                            };
                            VRUIUtilsService.callDirectiveLoad(packageItemsDirectiveAPI, packageItemsPayload, packageItemsDirectiveLoadDeferred);
                        });

                        return packageItemsDirectiveLoadDeferred.promise.then(function () {
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

            //$scope.scopeModel.hasSaveProductFamilyPermission = function () {
            //    if ($scope.scopeModel.isEditMode)
            //        return Retail_BE_ProductFamilyAPIService.HasUpdateProductFamilyPermission();
            //    else
            //        return Retail_BE_ProductFamilyAPIService.HasAddProductFamilyPermission();
            //};
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
            return Retail_BE_ProductFamilyAPIService.GetProductFamily(productFamilyId).then(function (response) {
                productFamilyEntity = response;
            });
        }

        function loadAllControls() {

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadProductDefinitionsSelector])
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
        //function loadPackageItemsDirective() {
        //    if (!isEditMode)
        //        return;

        //    var packageItemsDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

        //    UtilsService.waitMultiplePromises([packageItemsDirectiveReadyDeferred.promise, productDefinitionSelectionChangedDeferred.promise]).then(function () {

        //        var packageItemsPayload = {
        //            productDefinitionId: productDefinitionInfoEntity.ProductDefinitionId
        //        };
        //        if (productFamilyEntity != undefined && productFamilyEntity.Settings != undefined) {
        //            packageItemsPayload.packageNameByIds = productFamilyEditorRuntime.PackageNameByIds;
        //            packageItemsPayload.packages = productFamilyEntity.Settings.Packages;
        //        }
        //        VRUIUtilsService.callDirectiveLoad(packageItemsDirectiveAPI, packageItemsPayload, packageItemsDirectiveLoadDeferred);
        //    });

        //    return packageItemsDirectiveLoadDeferred.promise;
        //}


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
                    //Packages: packageItemsDirectiveAPI.getData()
                }
            };
            return obj;
        }
    }

    appControllers.controller('Retail_BE_ProductFamilyEditorController', ProductFamilyEditorController);

})(appControllers);
