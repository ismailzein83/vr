"use strict"
app.directive("demoModuleProductGrid", ["VRNotificationService", "Demo_Module_ProductAPIService", "Demo_Module_ProductService", "Demo_Module_ItemService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
    function (VRNotificationService, Demo_Module_ProductAPIService, Demo_Module_ProductService, Demo_Module_ItemService, VRUIUtilsService, VRCommon_ObjectTrackingService) {
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
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Demo_Module/Directives/Product/templates/ProductGridTemplate.html"
        };

        function ProductGrid($scope, ctrl, $attrs) {
            var gridAPI;
            var gridDrillDownTabs;
            this.initializeController = initializeController;

            function initializeController() {

                $scope.scopeModel = {};
                $scope.scopeModel.products = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;

                    gridDrillDownTabs = VRUIUtilsService.defineGridDrillDownTabs(getGridDrillDownDefinitions(), gridAPI, $scope.scopeModel.gridMenuActions);

                    if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                        ctrl.onReady(getDirectiveAPI());
                    }

                    function getDirectiveAPI() {
                        var directiveAPI = {};
                        directiveAPI.loadGrid = function (query) {
                            return gridAPI.retrieveData(query);
                        };

                        directiveAPI.onProductAdded = function (product) {
                            gridDrillDownTabs.setDrillDownExtensionObject(product);
                            gridAPI.itemAdded(product);
                        };
                        return directiveAPI;
                    }
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Demo_Module_ProductAPIService.GetFilteredProducts(dataRetrievalInput)
                    .then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                var tableItem = response.Data[i];
                                gridDrillDownTabs.setDrillDownExtensionObject(tableItem);
                            }
                        }
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };
                defineMenuActions();
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: "Edit",
                    clicked: editProduct,

                }, {
                    name: "Delete",
                    clicked: deleteProduct,
                }];
            }

            function editProduct(product) {
                var onProductUpdated = function (product) {
                    gridAPI.itemUpdated(product);
                }
                Demo_Module_ProductService.editProduct(product.ProductId, onProductUpdated);
            }

            function deleteProduct(product) {
                var onProductDeleted = function (product) {
                    gridAPI.itemDeleted(product);
                };
                Demo_Module_ProductService.deleteProduct($scope, product, onProductDeleted)
            }

            function getGridDrillDownDefinitions() {
                var drillDownDefinitions = [];
                drillDownDefinitions.push(getItemDrillDownDefinition());
                return drillDownDefinitions;
            }

            function getItemDrillDownDefinition() {
                var drillDownDefinition = {};
                drillDownDefinition.title = "Items";
                drillDownDefinition.directive = "demo-module-item-grid";
                drillDownDefinition.loadDirective = function (itemGridAPI, tableItem) {
                    tableItem.itemGridAPI = itemGridAPI;
                    var query = {
                        Name: null,
                        ProductIds: [tableItem.Entity.ProductId]
                    };
                    return itemGridAPI.loadGrid(query);
                };

                drillDownDefinition.parentMenuActions = [{
                    name: "Add Item",
                    clicked: function (tableItem) {
                        if (drillDownDefinition.setTabSelected != undefined)
                            drillDownDefinition.setTabSelected(tableItem);

                        var onItemAdded = function (itemObj) {
                            if (tableItem.itemGridAPI != undefined) {
                                tableItem.itemGridAPI.onItemAdded(itemObj);
                            }
                        };
                        Demo_Module_ItemService.addItem(onItemAdded);
                    },
                }];
                return drillDownDefinition;
            }

            function getDrillDownToAnalyticTable() {
                var drillDownDefinition = {};

                drillDownDefinition.title = VRCommon_ObjectTrackingService.getObjectTrackingGridTitle();
                drillDownDefinition.directive = "vr-common-objecttracking-grid";

                drillDownDefinition.loadDirective = function (directiveAPI, analyticTableItem) {

                    analyticTableItem.objectTrackingGridAPI = directiveAPI;
                    var query = {
                        ObjectId: analyticTableItem.Entity.ProductId,
                        EntityUniqueName: ItemAPIService.GetItemsInfo(ObjectId),

                    };

                    return analyticTableItem.objectTrackingGridAPI.load(query);
                };

                return drillDownDefinition;
            }
        }
        return directiveDefinitionObject;
    }]
    );