(function (appControllers) {

    'use stict';

    ProductFamilyService.$inject = ['VRModalService'];

    function ProductFamilyService(VRModalService) {

        function addProductFamily(onProductFamilyAdded) {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductFamilyAdded = onProductFamilyAdded
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ProductFamily/ProductFamilyEditor.html', null, settings);
        };

        function editProductFamily(productFamilyId, onProductFamilyUpdated) {

            var parameters = {
                productFamilyId: productFamilyId
            };

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onProductFamilyUpdated = onProductFamilyUpdated;
            };

            VRModalService.showModal('/Client/Modules/Retail_BusinessEntity/Views/ProductFamily/ProductFamilyEditor.html', parameters, settings);
        }


        return {
            addProductFamily: addProductFamily,
            editProductFamily: editProductFamily
        };
    }

    appControllers.service('Retail_BE_ProductFamilyService', ProductFamilyService);

})(appControllers);