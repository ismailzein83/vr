'use strict';

app.directive('retailBeProductGrid', ['VRNotificationService', 'Retail_BE_ProductAPIService', 'Retail_BE_ProductService', 'VRUIUtilsService', 'VRCommon_ObjectTrackingService',
    function (VRNotificationService, Retail_BE_ProductAPIService, Retail_BE_ProductService, VRUIUtilsService, VRCommon_ObjectTrackingService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ProductGridCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Retail_BusinessEntity/Directives/Product/ProductRuntime/Templates/ProductGridTemplate.html'
        };

        function ProductGridCtor($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var productFamilyId;
            var productDefinitionId;
            var gridDrillDownTabsObj;

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.product = [];
                //$scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    var drillDownDefinitions = Retail_BE_ProductService.getDrillDownDefinition();
                    gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_ProductAPIService.GetFilteredProducts(dataRetrievalInput).then(function (response) {
                        if (response.Data != undefined) {
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

                api.load = function (payload) {

                    if (payload != undefined) {
                        productFamilyId = payload.productFamilyId;
                        productDefinitionId = payload.productDefinitionId;
                    }

                    function buildGridQuery(payload) {
                        var query = {
                            ProductFamilyId: productFamilyId,                        
                            productDefinitionId: payload.productDefinitionId
                        };

                        return query;
                    }

                    return gridAPI.retrieveData(buildGridQuery(payload));
                };

                api.onProductAdded = function (addedProduct) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(addedProduct);
                    gridAPI.itemAdded(addedProduct);
                };

                api.onProductUpdated = function (updatedProduct) {
                    gridAPI.itemUpdated(updatedProduct);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions = function (dataItem) {
                    var menuActions = [];
                    if (dataItem.AllowEdit == true)
                        menuActions.push({
                            name: 'Edit',
                            clicked: editProduct
                        });
                    return menuActions;
                }
            }
            function editProduct(productItem) {
                var onProductUpdated = function (updatedProduct) {
                    gridDrillDownTabsObj.setDrillDownExtensionObject(updatedProduct);
                    gridAPI.itemUpdated(updatedProduct);
                };

                Retail_BE_ProductService.editProduct(onProductUpdated, productItem.Entity.ProductId, productFamilyId, productDefinitionId);
            }
        }
    }]);
