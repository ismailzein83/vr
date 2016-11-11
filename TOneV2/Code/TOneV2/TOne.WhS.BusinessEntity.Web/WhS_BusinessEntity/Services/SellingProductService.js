(function (appControllers) {

    'use strict';

    SellingProductService.$inject = ['WhS_BE_SellingProductAPIService', 'VRModalService', 'VRNotificationService'];

    function SellingProductService(WhS_BE_SellingProductAPIService, VRModalService, VRNotificationService) {
        return ({
            addSellingProduct: addSellingProduct,
            editSellingProduct: editSellingProduct
        });

        function addSellingProduct(onSellingProductAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onSellingProductAdded = onSellingProductAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', null, settings);
        }

        function editSellingProduct(sellingProductObj, onSellingProductUpdated) {
            var modalSettings = {
            };
            var parameters = {
                SellingProductId: sellingProductObj.SellingProductId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onSellingProductUpdated = onSellingProductUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/SellingProduct/SellingProductEditor.html', parameters, modalSettings);
        }

    }

    appControllers.service('WhS_BE_SellingProductService', SellingProductService);

})(appControllers);
