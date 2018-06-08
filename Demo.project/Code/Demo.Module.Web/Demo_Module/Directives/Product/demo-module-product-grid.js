"use strict"
app.directive("demoModuleProductGrid", ["UtilsService", "VRNotificationService", "Demo_Module_ProductAPIService", "Demo_Module_ProductService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_ProductAPIService, Demo_Module_ProductService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var productGrid = new ProductGrid($scope, ctrl, $attrs);
            productGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Product/Templates/ProductGridTemplate.html"
    };

    function ProductGrid($scope, ctrl) {

        var gridApi;
        var gridDrillDownTabsObj;

        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.products = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                var drillDownDefinitions = [];
                AddItemDrillDown();
                function AddItemDrillDown() {
                    var drillDownDefinition = {};

                    drillDownDefinition.title = "Item";
                    drillDownDefinition.directive = "demo-module-item-search";

                    drillDownDefinition.loadDirective = function (directiveAPI, productItem) {
                        console.log(productItem);
                        productItem.itemGridAPI = directiveAPI;
                        var payload = {
                            productId: productItem.ProductId
                        };
                        return productItem.itemGridAPI.load(payload);
                    };
                    drillDownDefinitions.push(drillDownDefinition);
                }

                gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridApi, $scope.gridMenuActions);
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (query) {
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onProductAdded = function (product) {
                        gridDrillDownTabsObj.setDrillDownExtensionObject(product);
                        gridApi.itemAdded(product);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                console.log("aaaaa");
                return Demo_Module_ProductAPIService.GetFilteredProducts(dataRetrievalInput)
                .then(function (response) {
                    if (response && response.Data) {
                        console.log(response);
                        for (var i = 0; i < response.Data.length; i++) {
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                        }
                    }
                    onResponseReady(response);//load products data
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
            };

            defineMenuActions();
        };

        function defineMenuActions() {
            $scope.scopeModel.gridMenuActions = [{
                name: "Edit",
                clicked: editProduct,

            }];
        };
        function editProduct(product) {
            var onProductUpdated = function (product) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(product);
                gridApi.itemUpdated(product);
            };
            Demo_Module_ProductService.editProduct(product.ProductId, onProductUpdated);
        };


    };
    return directiveDefinitionObject;
}]);
