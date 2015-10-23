(function (appControllers) {

    "use strict";

    routingProductManagementController.$inject = ['$scope', 'WhS_BE_RoutingProductAPIService', 'WhS_BE_SellingNumberPlanAPIService', 'UtilsService', 'VRModalService', 'VRNotificationService'];

    function routingProductManagementController($scope, WhS_BE_RoutingProductAPIService, WhS_BE_SellingNumberPlanAPIService, UtilsService, VRModalService, VRNotificationService) {
        var gridApi;

        defineScope();
        load();

        function defineScope() {

            $scope.routingProducts = [];
            $scope.gridMenuActions = [];

            $scope.sellingNumberPlans = [];
            $scope.selectedSellingNumberPlans = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                api.loadGrid({});
            }

            //$scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            //    return WhS_BE_RoutingProductAPIService.GetFilteredRoutingProducts(dataRetrievalInput)
            //        .then(function (response) {
            //            onResponseReady(response);
            //        })
            //        .catch(function (error) {
            //            VRNotificationService.notifyExceptionWithClose(error, $scope);
            //        });
            //};

            $scope.searchClicked = function () {
                if (!$scope.isGettingData && gridAPI != undefined)
                    return gridAPI.loadGrid(getFilterObject());
            };

            $scope.AddNewRoutingProduct = AddNewRoutingProduct;

            defineMenuActions();

            function getFilterObject() {
                var data = {
                    Name: $scope.name,
                    SellingNumberPlanIds: UtilsService.getPropValuesFromArray($scope.selectedSellingNumberPlans, "SellingNumberPlanId")
                };
                return data;
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
        }

        function load() {
            $scope.isLoadingFilterData = true;

            WhS_BE_SellingNumberPlanAPIService.GetSellingNumberPlans().then(function (response) {
                angular.forEach(response, function (item) {
                    $scope.sellingNumberPlans.push(item);
                });
            }).catch(function (error) {
                VRNotificationService.notifyExceptionWithClose(error, $scope);
            }).finally(function () {
                $scope.isLoadingFilterData = false;
            });
        }

        function AddNewRoutingProduct() {

            var settings = {};

            settings.onScopeReady = function (modalScope) {
                modalScope.title = "New Routing Product";
                modalScope.onRoutingProductAdded = function (routingProduct) {
                    gridApi.itemAdded(routingProduct);
                };
            };
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', null, settings);

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
            VRModalService.showModal('/Client/Modules/WhS_BusinessEntity/Views/RoutingProduct/RoutingProductEditor.html', parameters, modalSettings);
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

    appControllers.controller('WhS_BE_RoutingProductManagementController', routingProductManagementController);
})(appControllers);