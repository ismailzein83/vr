RoutingProductManagementController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SaleZonePackageAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

function RoutingProductManagementController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_SaleZonePackageAPIService, UtilsService, VRModalService, VRNotificationService) {

    var gridApi;

    defineScope();
    load();

    function defineScope() {

        $scope.routingProducts = [];
        $scope.gridMenuActions = [];

        $scope.saleZonePackages = [];
        $scope.selectedSaleZonePackages = [];

        defineMenuActions();

        $scope.gridReady = function (api) {
            gridApi = api;
            return retrieveData();
        };

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return WhS_BE_RoutingProductAPIService.GetFilteredRoutingProducts(dataRetrievalInput)
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
        $scope.isLoadingFilterData = true;

        WhS_BE_SaleZonePackageAPIService.GetSaleZonePackages().then(function (response) {
            angular.forEach(response, function (item) {
                $scope.saleZonePackages.push(item);
            });
        }).catch(function (error) {
            VRNotificationService.notifyExceptionWithClose(error, $scope);
        }).finally(function () {
            $scope.isLoadingFilterData = false;
        });
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
        VRNotificationService.showConfirmation()
            .then(function (response) {
                if (response) {

                    return WhS_BE_RoutingProductAPIService.DeleteRoutingProduct(routingProduct.RoutingProductId)
                        .then(function (deletionResponse) {
                            VRNotificationService.notifyOnItemDeleted("Routing Product", deletionResponse);
                            return retrieveData();
                        })
                        .catch(function (error) {
                            VRNotificationService.notifyException(error, $scope);
                        });
                }
            });
    }
}

appControllers.controller('WhS_BE_RoutingProductManagementController', RoutingProductManagementController);