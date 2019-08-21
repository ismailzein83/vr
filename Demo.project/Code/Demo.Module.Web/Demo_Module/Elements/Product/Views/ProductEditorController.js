(function (appControllers) {

    'use strict';

    productEditorController.$inject = ['$scope', 'UtilsService', 'VRNavigationService', 'VRNotificationService', 'Demo_Module_ProductAPIService', 'VRUIUtilsService'];

    function productEditorController($scope, UtilsService, VRNavigationService, VRNotificationService, Demo_Module_ProductAPIService, VRUIUtilsService) {

        var isEditMode;
        var productId;
        var product;
        var manufactoryIdItem;

        var manufactorySelectorAPI;
        var manufactorySelectoryReadyDeferred = UtilsService.createPromiseDeferred();

        var productSettingsAPI;
        var productSettingsDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

        loadParameters();
        defineAPI();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            
            if (parameters != undefined) {
                productId = parameters.productId;
                manufactoryIdItem = parameters.manufactoryIdItem;
            }

            isEditMode = (productId != undefined);
        }

        function defineAPI() {

            $scope.scopeModel = {};
            $scope.scopeModel.disableManufactory = (manufactoryIdItem != undefined);

            $scope.scopeModel.onSaveClicked = function () {
                if (isEditMode)
                    return updateProduct();
                else
                    return insertProduct();
            };

            $scope.scopeModel.onCloseClicked = function () {
                $scope.modalContext.closeModal();
            };

            $scope.scopeModel.onManufactorySelectorReady = function (api) {
                manufactorySelectorAPI = api;
                manufactorySelectoryReadyDeferred.resolve();
            };

            $scope.scopeModel.onProductSettingsDirectiveReady = function (api) {
                productSettingsAPI = api;
                productSettingsDirectiveReadyDeferred.resolve();
            };

        }

        function load() {
            $scope.scopeModel.isLoading = true;

            if (isEditMode) {
                getProduct().then(function () {
                    loadAllControls().finally(function () {
                        product = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else {
                loadAllControls();
            }

            function loadAllControls() {

                function setTitle() {
                    if (isEditMode && product != undefined) {
                        $scope.title = UtilsService.buildTitleForUpdateEditor(product.Name, 'Product');
                    }
                    else {
                        $scope.title = UtilsService.buildTitleForAddEditor('Produt');
                    }
                }

                function loadStaticData() {
                    if (product != undefined) {
                        $scope.scopeModel.name = product.Name;
                    }
                }

                function loadManufactorySelector() {
                    var manufactorySelectorLoadDeferred = UtilsService.createPromiseDeferred();

                    manufactorySelectoryReadyDeferred.promise.then(function () {
                        var payload = {};
                        if (manufactoryIdItem != undefined) { //in case of adding a new product from manufactory grid
                            payload.selectedIds = manufactoryIdItem.manufactoryId;
                        }
                        if (product != undefined) {
                            payload.selectedIds = product.ManufactoryId;
                        }
                        VRUIUtilsService.callDirectiveLoad(manufactorySelectorAPI, payload, manufactorySelectorLoadDeferred);
                    });

                    return manufactorySelectorLoadDeferred.promise;
                }

                function loadProductSettingsDirective() {
                    var productSettingsLoadDeferred = UtilsService.createPromiseDeferred();

                    productSettingsDirectiveReadyDeferred.promise.then(function () {
                        var payload = product;
                        VRUIUtilsService.callDirectiveLoad(productSettingsAPI, payload, productSettingsLoadDeferred);
                    });

                    return productSettingsLoadDeferred.promise;
                }

                return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData, loadManufactorySelector, loadProductSettingsDirective]).catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                }).finally(function () {
                    $scope.scopeModel.isLoading = false;
                });

            }
        }

        function getProduct() {
            return Demo_Module_ProductAPIService.GetProductById(productId).then(function (response) {
                product = response;
            });
        }

        function updateProduct() {
            $scope.scopeModel.isLoading = true;

            var updatedProduct = buildProduct();
            Demo_Module_ProductAPIService.UpdateProduct(updatedProduct).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated('Product', response, 'Name')) {
                    if ($scope.onProductUpdated != undefined) {
                        $scope.onProductUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoading = false;
            });
        }

        function insertProduct() {
            $scope.scopeModel.isLoading = true;

            var insertedProduct = buildProduct();
            return Demo_Module_ProductAPIService.AddProduct(insertedProduct).then(function (response) {
                if (VRNotificationService.notifyOnItemAdded('Product', response, 'Name')) {
                    if ($scope.onProductAdded != undefined) {
                        $scope.onProductAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });
        }

        function buildProduct() {
            return {
                Id: productId,
                Name: $scope.scopeModel.name,
                ManufactoryId: manufactorySelectorAPI.getSelectedIds(),
                Settings: productSettingsAPI.getData()
            };
        }

    }

    appControllers.controller('Demo_Module_ProductEditorController', productEditorController);

})(appControllers);