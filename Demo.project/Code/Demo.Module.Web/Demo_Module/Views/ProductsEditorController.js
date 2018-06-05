(function (appControllers) {


ProductsEditorController.$inject = ['$scope', 'Demo_Module_ProductAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

function ProductsEditorController($scope, Demo_Module_ProductAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {


    var isEditMode;
    var productId;
    var productEntity;

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
                updateProduct();
            else
                insertProduct();
        };
        $scope.scopeModel.close = function () {
            $scope.modalContext.closeModal()
        };

    };

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
                $scope.isLoading = false;
            });
        } else {
            loadAllControls();
        }
    };

    function getProduct() {
        return Demo_Module_ProductAPIService.GetProductById(productId).then(function (response) {
            productEntity = response;
        });
    };


    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.scopeModel.isLoading = false;
          });
    }

    function setTitle() {
        if (isEditMode && productEntity != undefined)
            $scope.title = UtilsService.buildTitleForUpdateEditor(productEntity.Name, "Product");
        else
            $scope.title = UtilsService.buildTitleForAddEditor("Product");
    };

    function loadStaticData() {
        if (productEntity != undefined)
            $scope.scopeModel.name = productEntity.Name;
    };


    function buildProductObjectFromScope() {
        var object = {
            ProductId: (productId != undefined) ? productId : undefined,
            Name: $scope.scopeModel.name,
        };
        return object;
    };

    function insertProduct() {

        $scope.scopeModel.isLoading = true;
        var productObject = buildProductObjectFromScope();
        return Demo_Module_ProductAPIService.AddProduct(productObject).then(function (response) {

            if (VRNotificationService.notifyOnItemAdded("Product", response, "Name")) {

                if ($scope.onProductAdded != undefined) {
                    $scope.onProductAdded(response.InsertedObject);
                }
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

        var productObject = buildProductObjectFromScope();
        Demo_Module_ProductAPIService.UpdateProduct(productObject)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Product", response, "Name")) {
                if ($scope.ProductUpdated != undefined)
                    $scope.ProductUpdated(response.ProductObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.scopeModel.isLoading = false;
        });
    }
}


appControllers.controller('Demo_Module_ProductsEditorController', ProductsEditorController);
})(appControllers);