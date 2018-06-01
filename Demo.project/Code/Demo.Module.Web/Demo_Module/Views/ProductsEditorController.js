(function (appControllers) {


ProductsEditorController.$inject = ['$scope', 'Demo_Module_ProductAPIService', 'VRNotificationService', 'VRNavigationService', 'UtilsService', 'VRUIUtilsService'];

function ProductsEditorController($scope, Demo_Module_ProductAPIService, VRNotificationService, VRNavigationService, UtilsService, VRUIUtilsService) {


    var isEditMode;
    var productId;
    var productEntity;
    var context;

    loadParameters();
    defineScope();
    load();


    function loadParameters() {

        var parameters = VRNavigationService.getParameters($scope);

        if (parameters != undefined && parameters != null) {

            productId = parameters.productId;
            context = parameters.context;

        }
        
        isEditMode = (productId != undefined);
    }

    function defineScope() {

        $scope.saveProduct = function () {
            if (isEditMode)
                 updateProduct();
            else
                 insertProduct();
        }

        $scope.close = function () {
            $scope.modalContext.closeModal()
        };


    }

    function load() {

        $scope.isLoading = true;

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
    }

    function getProduct() {
        return Demo_Module_ProductAPIService.GetProductById(productId).then(function (productObject) {
            productEntity = productObject;
        });
    }


    function loadAllControls() {
        return UtilsService.waitMultipleAsyncOperations([setTitle, loadStaticData])
           .catch(function (error) {
               VRNotificationService.notifyExceptionWithClose(error, $scope);
           })
          .finally(function () {
              $scope.isLoading = false;
          });
    }

    function setTitle() {
        if (isEditMode && productEntity != undefined)
            $scope.title = UtilsService.buildTitleForUpdateEditor(productEntity.Name, "Product");
        else
            $scope.title = UtilsService.buildTitleForAddEditor("Product");
    }

    function loadStaticData() {
        if (productEntity != undefined)
            $scope.name = productEntity.Name;
    }


    function insertProduct() {

        $scope.isLoading = true;

        var productObj = {
            ProductId: productId,
            Name: $scope.name
        };

        return Demo_Module_ProductAPIService.AddProduct(productObj).then(function (response) {

            if (VRNotificationService.notifyOnItemAdded("Product", response, "Name")) {

                if ($scope.onProductAdded != undefined) {

                    $scope.onProductAdded(response.InsertedObject);
                }

                $scope.modalContext.closeModal();

            }

        }).catch(function (error) {

            VRNotificationService.notifyException(error, $scope);
            console.log(error);

        }).finally(function () {

            $scope.isLoading = false;
        });

    }

    function updateProduct() {
        $scope.isLoading = true;

        var productObj = {
            ProductId: productId,
            Name: $scope.name
        };

        Demo_Module_ProductAPIService.UpdateProduct(productObj)
        .then(function (response) {
            if (VRNotificationService.notifyOnItemUpdated("Product", response, "Name")) {
                if ($scope.ProductUpdated != undefined)
                    $scope.ProductUpdated(response.ProductObject);
                $scope.modalContext.closeModal();
            }
        }).catch(function (error) {
            VRNotificationService.notifyException(error, $scope);
        }).finally(function () {
            $scope.isLoading = false;
        });
    }
}


appControllers.controller('Demo_Module_ProductsEditorController', ProductsEditorController);
})(appControllers);