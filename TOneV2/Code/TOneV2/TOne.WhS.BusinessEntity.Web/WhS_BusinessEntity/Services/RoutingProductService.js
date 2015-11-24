

app.service('WhS_BE_RoutingProductService', ['WhS_BE_RoutingProductAPIService',
    'VRModalService', 'VRNotificationService', 'UtilsService',
    function (WhS_BE_RoutingProductAPIService, VRModalService, VRNotificationService, UtilsService) {


        function addRoutingProduct(onRoutingProductAdded) {
            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "New Routing Product";
                modalScope.onRoutingProductAdded = onRoutingProductAdded
            };

            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', null, settings);
        };

        function editRoutingProduct(routingProductObj, onRoutingProductUpdated) {
            var modalSettings = {
            };

            var parameters = {
                routingProductId: routingProductObj.RoutingProductId
            };

            modalSettings.onScopeReady = function (modalScope) {
                modalScope.title = "Edit Routing Product: " + routingProductObj.Name;
                modalScope.onRoutingProductUpdated = onRoutingProductUpdated;
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', parameters, modalSettings);
        };

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
        };


        return ({
            addRoutingProduct: addRoutingProduct,
            editRoutingProduct: editRoutingProduct,
            deleteRoutingProduct: deleteRoutingProduct
        });

    }]);