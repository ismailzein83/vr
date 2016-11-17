(function (appControllers) {

    'use strict';

    RoutingProductService.$inject = ['VRModalService', 'VRNotificationService','UtilsService'];

    function RoutingProductService(VRModalService, VRNotificationService, UtilsService) {
        return ({
            addRoutingProduct: addRoutingProduct,
            editRoutingProduct: editRoutingProduct,
            deleteRoutingProduct: deleteRoutingProduct,
            viewRoutingProduct: viewRoutingProduct
        });

        function addRoutingProduct(onRoutingProductAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.onRoutingProductAdded = onRoutingProductAdded;
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', null, settings);
        }

        function editRoutingProduct(routingProductId, onRoutingProductUpdated) {
            var modalSettings = {
            };

            var parameters = {
                routingProductId: routingProductId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.onRoutingProductUpdated = onRoutingProductUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', parameters, modalSettings);
        }


        function viewRoutingProduct(routingProductId) {
            var modalSettings = {
            };

            var parameters = {
                routingProductId: routingProductId
            };

            modalSettings.onScopeReady = function (modalScope) {
                UtilsService.setContextReadOnly(modalScope);

            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', parameters, modalSettings);
        }

        function deleteRoutingProduct(scope, routingProductObj, onRoutingProductDeleted) {
            VRNotificationService.showConfirmation()
                    .then(function (response) {
                        if (response) {

                            return WhS_BE_RoutingProductAPIService.DeleteRoutingProduct(routingProductObj.RoutingProductId)
                                .then(function (deletionResponse) {
                                    VRNotificationService.notifyOnItemDeleted("Routing Product", deletionResponse);
                                    onRoutingProductDeleted(routingProductObj);
                                })
                                .catch(function (error) {
                                    VRNotificationService.notifyException(error, scope);
                                });
                        }
                    });
        }
    }

    appControllers.service('WhS_BE_RoutingProductService', RoutingProductService);

})(appControllers);
