app.service('Demo_Module_ProductService', ['VRModalService', 'Demo_Module_ProductAPIService','VRNotificationService',

function (VRModalService, Demo_Module_ProductAPIService,VRNotificationService) {


    function addProduct(onProductAdded) {

        var settings = {
        };

        settings.onScopeReady = function (modalScope) {
            modalScope.onProductAdded = onProductAdded;
        };
        var parameters = {};

        VRModalService.showModal("/Client/Modules/Demo_Module/Views/ProductsEditor.html",parameters,settings);
    }


    function editProduct(productId, onProductUpdated) {
        var settings = {
        };
        var parameters = {
            productId: productId
        };
        settings.onScopeReady = function (modalScope) {
            modalScope.onProductUpdated = onProductUpdated;
        };


        VRModalService.showModal('/Client/Modules/Demo_Module/Views/ProductsEditor.html', parameters, settings);
    }

    function deleteProduct(scope, dataItem, onProductDeleted) {
        VRNotificationService.showConfirmation().then(function (confirmed) {
            if (confirmed) {
                return Demo_Module_ProductAPIService.DeleteProduct(dataItem.Entity.ProductId).then(function (responseObject) {
                    var deleted = VRNotificationService.notifyOnItemDeleted('Product', responseObject);

                    if (deleted && onProductDeleted && typeof onProductDeleted == 'function') {
                        onProductDeleted(dataItem);
                    }
                }).catch(function (error) {
                    VRNotificationService.notifyException(error, scope);
                })
            }
        });
    }

    return ({
        addProduct: addProduct,
        editProduct: editProduct,
        deleteProduct: deleteProduct
    });

}]);