
app.service('WhS_BE_MainService', ['WhS_BE_RouteRuleAPIService','WhS_BE_PricingProductAPIService', 'VRModalService', 'VRNotificationService', function (WhS_BE_RouteRuleAPIService,WhS_BE_PricingProductAPIService, VRModalService, VRNotificationService) {

    return ({
        addRouteRule: addRouteRule,
        editRouteRule: editRouteRule,
        deleteRouteRule: deleteRouteRule,
        addPricingProduct: addPricingProduct,
        editPricingProduct: editPricingProduct,
        deletePricingProduct: deletePricingProduct
    });

    function addRouteRule(onRouteRuleAdded)
    {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Route Rule";
            modalScope.onRouteRuleAdded = onRouteRuleAdded;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RouteRule/RouteRuleEditor.html', null, settings);
    }

    function editRouteRule(routeRuleObj, onRouteRuleUpdated) {
        var modalSettings = {
        };
        var parameters = {
            routeRuleId: routeRuleObj.RouteRuleId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Route Rule";
            modalScope.onRouteRuleUpdated = onRouteRuleUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RouteRule/RouteRuleEditor.html', parameters, modalSettings);
    }

    function deleteRouteRule(routeRuleObj, onRouteRuleDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_RouteRuleAPIService.DeleteRouteRule(routeRuleObj.RouteRuleId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Route Rule", deletionResponse);
                            onRouteRuleDeleted();
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

    function addPricingProduct(onPricingProductAdded) {
        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Pricing Product";
            modalScope.onPricingProductAdded = onPricingProductAdded;
        };

        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingProduct/PricingProductEditor.html', null, settings);
    }

    function editPricingProduct(pricingProductObj, onPricingProductUpdated) {
        var modalSettings = {
        };
        var parameters = {
            routeRuleId: pricingProductObj.PricingProductId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Pricing Product";
            modalScope.onPricingProductUpdated = onPricingProductUpdated;
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/PricingProduct/PricingProductEditor.html', parameters, modalSettings);
    }

    function deletePricingProduct(pricingProductObj, onPricingProductDeleted) {
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {
                    return WhS_BE_PricingProductAPIService.DeletePricingProduct(pricingProductObj.PricingProductId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Pricing Product", deletionResponse);
                            onPricingProductDeleted();
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }

}]);
