"use strict";

app.directive("vrWhsBeSellingproductGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SellingProductAPIService", "WhS_BE_SellingProductService", "WhS_BE_CustomerSellingProductService", "VRUIUtilsService", "WhS_BE_CustomerSellingProductAPIService",
function (UtilsService, VRNotificationService, WhS_BE_SellingProductAPIService, WhS_BE_SellingProductService, WhS_BE_CustomerSellingProductService, VRUIUtilsService, WhS_BE_CustomerSellingProductAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var sellingProductGrid = new SellingProductGrid($scope, ctrl);
            sellingProductGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SellingProduct/Templates/SellingProductGridTemplate.html"

    };

    function SellingProductGrid($scope, ctrl) {

        var gridAPI;
        this.initializeController = initializeController;
        var gridDrillDownTabsObj;
        function initializeController() {
            $scope.sellingProducts = [];

            defineMenuActions();

            $scope.gridReady = function (api) {
                gridAPI = api;

                var drillDownDefinitions = [];
                var drillDownDefinition = {};

                drillDownDefinition.title = "Customer Selling Product";
                drillDownDefinition.directive = "vr-whs-be-customersellingproduct-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, dataItem) {
                    dataItem.custormerSellingProductGridAPI = directiveAPI;
                    var payload = {
                        query: {
                            SellingProductsIds: [dataItem.Entity.SellingProductId]
                        },
                        hideSellingProductColumn: true
                    };
                    return dataItem.custormerSellingProductGridAPI.loadGrid(payload);
                };
                drillDownDefinitions.push(drillDownDefinition);
                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    };
                    directiveAPI.onSellingProductAdded = function (sellingProductObject) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(sellingProductObject);
                        gridAPI.itemAdded(sellingProductObject);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SellingProductAPIService.GetFilteredSellingProducts(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };

        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Edit",
                clicked: editSellingProduct,
                haspermission: hasUpdateSellingProductPermission
            },
           {
               name: "Assign Customer",
               clicked: assignCustomer,
               haspermission: hasAddCustomerSellingProductPermission
           }
            ];
        }

        function hasAddCustomerSellingProductPermission() {
            return WhS_BE_CustomerSellingProductAPIService.HasAddCustomerSellingProductPermission();
        }

        function hasUpdateSellingProductPermission() {
            return WhS_BE_SellingProductAPIService.HasUpdateSellingProductPermission();
        }

        function editSellingProduct(sellingProductObj) {
            var onSellingProductUpdated = function (sellingProduct) {
                gridAPI.itemUpdated(sellingProduct);
            };

            WhS_BE_SellingProductService.editSellingProduct(sellingProductObj.Entity, onSellingProductUpdated);
        }
        function assignCustomer(dataItem) {
            gridAPI.expandRow(dataItem);
            var query = {
                SellingProductsIds: [dataItem.Entity.SellingProductId]
            };

            var onCustomerSellingProductAdded = function (customerSellingProductObj) {
                if (dataItem.custormerSellingProductGridAPI != undefined) {
                    for (var i = 0; i < customerSellingProductObj.length; i++) {
                        dataItem.custormerSellingProductGridAPI.onCustomerSellingProductAdded(customerSellingProductObj[i]);

                    }
                }
            };
            WhS_BE_CustomerSellingProductService.addCustomerSellingProduct(onCustomerSellingProductAdded, dataItem.Entity);
        }
    }

    return directiveDefinitionObject;

}]);
