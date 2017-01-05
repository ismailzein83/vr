(function (appControllers) {

    'use stict';

    ProductService.$inject = ['VRModalService'];

    function ProductService(VRModalService) {

        function addProduct(onProductAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductAdded = onProductAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Product/ProductEditor.html', null, settings);
        };

        function editProduct(productId, onProductUpdated) {
            var settings = {};

            var parameters = {
                productId: productId,
            };

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductUpdated = onProductUpdated;
            };
            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/Product/ProductEditor.html', parameters, settings);
        }


        return {
            addProduct: addProduct,
            editProduct: editProduct
        };
    }

    appControllers.service('Retail_BE_ProductService', ProductService);

})(appControllers);