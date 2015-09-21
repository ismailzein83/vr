RoutingProductManagementController.$inject = ['$scope', 'RoutingProductAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

function RoutingProductManagementController($scope, RoutingProductAPIService, UtilsService, VRModalService, VRNotificationService) {

    var gridApi;

    defineScope();
    load();

    function defineScope() {

        $scope.routingProducts = [];
        $scope.gridMenuActions = [];

        defineMenuActions();

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return RoutingProductAPIService.GetFilteredRoutingProducts(dataRetrievalInput)
                .then(function (response) {
                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyExceptionWithClose(error, $scope);
                });
        };

        $scope.searchClicked = function () {
            return retrieveData();
        };

        $scope.AddNewRoutingProduct = AddNewRoutingProduct;
    }

    function load() {

    }

    function retrieveData() {
        var query = {
            Name: $scope.name,
            SaleZonePackageIds: UtilsService.getPropValuesFromArray($scope.selectedSaleZonePackages, "SaleZonePackageId")
        };

        return gridApi.retrieveData(query);
    }

    function defineMenuActions() {
        $scope.gridMenuActions = [{
            name: "Edit",
            clicked: editRoutingProduct,
        },
        {
            name: "Delete",
            clicked: deleteRoutingProduct,
        }
        ];
    }

    function AddNewRoutingProduct() {

        var settings = {};

        settings.onScopeReady = function (modalScope) {
            modalScope.title = "New Routing Product";
            modalScope.onRoutingProductAdded = function (routingProduct) {
                gridApi.itemAdded(routingProduct);
            };
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProductEditor.html', null, settings);

    }

    function editRoutingProduct(routingProduct) {
        var modalSettings = {
        };
        var parameters = {
            routingProductId: routingProduct.RoutingProductId
        };

        modalSettings.onScopeReady = function (modalScope) {
            modalScope.title = "Edit Routing Product: " + routingProduct.Name;
            modalScope.onRoutingProductUpdated = function (routingProduct) {
                gridApi.itemUpdated(routingProduct);
            };
        };
        VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProductEditor.html', parameters, modalSettings);
    }

    function deleteRoutingProduct(routingProduct) {

    }
}

appControllers.controller('WhS_BusinessEntity_RoutingProductManagementController', RoutingProductManagementController);