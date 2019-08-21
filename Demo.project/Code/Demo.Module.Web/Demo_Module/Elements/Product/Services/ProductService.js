(function (appControllers) {

    'use strict';

    productService.$inject = ['VRModalService'];

    function productService(VRModalService) {

        function addProduct(onProductAdded, manufactoryIdItem) {

            var parameters = {
                manufactoryIdItem: manufactoryIdItem //send manufactory id in case of adding a new product from manufactory grid
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onProductAdded = onProductAdded;
            };

            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Product/Views/ProductEditor.html', parameters, settings);

        }

        function editProduct(onProductUpdated, productId, manufactoryIdItem) {

            var parameters = {
                productId: productId,
                manufactoryIdItem: manufactoryIdItem //disable selector for known manufactory
            };

            var settings = {};
            settings.onScopeReady = function (modalScope) {
                modalScope.onProductUpdated = onProductUpdated;
            };
            
            VRModalService.showModal('/Client/Modules/Demo_Module/Elements/Product/Views/ProductEditor.html', parameters, settings);

        }

        return {
            addProduct: addProduct,
            editProduct: editProduct
        };

    }

    appControllers.service('Demo_Module_ProductService', productService);

})(appControllers);