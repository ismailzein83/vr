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

    return ({
        addProduct: addProduct,
        editProduct: editProduct
    });

}]);