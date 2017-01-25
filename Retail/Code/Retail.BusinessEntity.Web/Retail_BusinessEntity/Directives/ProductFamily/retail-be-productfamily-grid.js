'use strict';

app.directive('retailBeProductfamilyGrid', ['VRNotificationService', 'VRUIUtilsService', 'Retail_BE_ProductFamilyAPIService', 'Retail_BE_ProductFamilyService', 'Retail_BE_ProductService',
    function (VRNotificationService, VRUIUtilsService, Retail_BE_ProductFamilyAPIService, Retail_BE_ProductFamilyService, Retail_BE_ProductService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProductFamilyGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/ProductFamily/Templates/ProductFamilyGridTemplate.html'
        };

        function ProductFamilyGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var gridDrillDownTabsObj;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.productFamily = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(buildDrillDownTabs(), gridAPI, $scope.scopeModel.menuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_ProductFamilyAPIService.GetFilteredProductFamilies(dataRetrievalInput).then(function (response) {
                        if (response && response.Data) {
                            for (var i = 0; i < response.Data.length; i++) {
                                gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                            }
                        }
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
                };

                defineMenuActions();
            }
            function defineAPI() {
                var api = {};

                api.load = function (query) {
                    return gridAPI.retrieveData(query);
                };

                api.onProductFamilyAdded = function (addedProductFamily) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedProductFamily);
                    gridAPI.itemAdded(addedProductFamily);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function buildDrillDownTabs() {
                var drillDownTabs = [];

                drillDownTabs.push(buildProductTab());

                function buildProductTab() {
                    var productsTab = {};

                    productsTab.title = 'Products';
                    productsTab.directive = 'retail-be-product-grid';

                    productsTab.loadDirective = function (productGridAPI, productFamily) {
                        productFamily.productGridAPI = productGridAPI;

                        var productGridPayload = {
                            productFamilyId: productFamily.Entity != undefined ? productFamily.Entity.ProductFamilyId : undefined
                        };
                        return productFamily.productGridAPI.load(productGridPayload);
                    };

                    productsTab.parentMenuActions = [{
                        name: 'Add Product',
                        clicked: function (productFamily) {
                            var onProductAdded = function (addedProduct) {
                                productFamily.productGridAPI.onProductAdded(addedProduct);
                            };

                            Retail_BE_ProductService.addProduct(onProductAdded, productFamily.Entity.ProductFamilyId);
                        }
                    }];

                    return productsTab;
                }

                return drillDownTabs;
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editProductFamily
                    //haspermission: hasEditProductFamilyPermission
                });
            }
            function editProductFamily(productFamilyItem) {
                var onProductFamilyUpdated = function (updatedProductFamily) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedProductFamily);
                    gridAPI.itemUpdated(updatedProductFamily);
                };

                Retail_BE_ProductFamilyService.editProductFamily(productFamilyItem.Entity.ProductFamilyId, onProductFamilyUpdated);
            }
            //function hasEditProductFamilyPermission() {
            //    return Retail_BE_ProductFamilyAPIService.HasUpdateProductFamilyPermission()
            //}
        }
    }]);
