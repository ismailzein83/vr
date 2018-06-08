(function (appControllers) {

    "use strict";
    ProductEditorController.$inject = ['$scope', 'Demo_Module_ProductAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

    function ProductEditorController($scope, Demo_Module_ProductAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {

        var isEditMode;
        var productId;
        var ProductEntity;

        loadParameters();
        defineScope();
        load();

        function loadParameters() {
            var parameters = VRNavigationService.getParameters($scope);
            if (parameters != undefined && parameters != null) {
                productId = parameters.productId;
            }
            isEditMode = (productId != undefined);
        };

        function defineScope() {

            $scope.scopeModel = {};

            $scope.scopeModel.saveProduct = function () {
                if (isEditMode)
                    return updateProduct();
                else
                    return insertProduct();

            };

            $scope.scopeModel.close = function () {
                $scope.modalContext.closeModal();
            };

        };

        function load() {
            $scope.scopeModel.isLoading = true;
            if (isEditMode) {
                getProduct().then(function () {
                    loadAllControls().finally(function () {
                        ProductEntity = undefined;
                    });
                }).catch(function (error) {
                    $scope.scopeModel.isLoading = false;
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
            }
            else
                loadAllControls();
        };

        function getProduct() {
            return Demo_Module_ProductAPIService.GetProductById(productId).then(function (response) {
                ProductEntity = response;
            });
        };

        function loadAllControls() {

            function setTitle() {
                if (isEditMode && ProductEntity != undefined)
                    $scope.title = UtilsService.buildTitleForUpdateEditor(ProductEntity.Name, "product");
                else
                    $scope.title = UtilsService.buildTitleForAddEditor("product");
            };

            function loadStaticData() {
                if (ProductEntity != undefined)
                    $scope.scopeModel.name = ProductEntity.Name;
            };

            return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
             .catch(function (error) {
                 VRNotificationService.notifyExceptionWithClose(error, $scope);
             })
               .finally(function () {
                   $scope.scopeModel.isLoading = false;
               });
        };

        function buildProductObjectFromScope() {
            var object = {
                productId: (productId != undefined) ? productId : undefined,
                Name: $scope.scopeModel.name,
            };
            return object;
        };

        function insertProduct() {

            $scope.scopeModel.isLoading = true;
            var ProductObject = buildProductObjectFromScope();
            return Demo_Module_ProductAPIService.AddProduct(ProductObject)
            .then(function (response) {
                if (VRNotificationService.notifyOnItemAdded("product", response, "Name")) {
                    if ($scope.onProductAdded != undefined) {
                        $scope.onProductAdded(response.InsertedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;
            });

        };

        function updateProduct() {
            $scope.scopeModel.isLoading = true;
            var ProductObject = buildProductObjectFromScope();
            console.log(ProductObject);
            Demo_Module_ProductAPIService.UpdateProduct(ProductObject).then(function (response) {
                if (VRNotificationService.notifyOnItemUpdated("product", response, "Name")) {
                    if ($scope.onProductUpdated != undefined) {
                        $scope.onProductUpdated(response.UpdatedObject);
                    }
                    $scope.modalContext.closeModal();
                }
            }).catch(function (error) {
                $scope.scopeModel.isLoading = false;
                VRNotificationService.notifyException(error, $scope);
            }).finally(function () {
                $scope.scopeModel.isLoading = false;

            });
        };

    };
    appControllers.controller('Demo_Module_ProductEditorController', ProductEditorController);
})(appControllers);