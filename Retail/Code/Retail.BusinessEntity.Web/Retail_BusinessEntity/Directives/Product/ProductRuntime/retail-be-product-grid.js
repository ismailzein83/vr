'use strict';

app.directive('retailBeProductGrid', ['VRNotificationService', 'Retail_BE_ProductAPIService', 'Retail_BE_ProductService',
    function (VRNotificationService, Retail_BE_ProductAPIService, Retail_BE_ProductService) {
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

            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.product = [];
                $scope.scopeModel.menuActions = [];

                $scope.scopeModel.onGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                    return Retail_BE_ProductAPIService.GetFilteredProducts(dataRetrievalInput).then(function (response) {
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

                api.onProductAdded = function (addedProduct) {
                    gridAPI.itemAdded(addedProduct);
                };

                api.onProductUpdated = function (updatedProduct) {
                    gridAPI.itemUpdated(updatedProduct);
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function defineMenuActions() {
                $scope.scopeModel.menuActions.push({
                    name: 'Edit',
                    clicked: editProduct
                    //haspermission: hasEditProductPermission
                });
            }
            function editProduct(productItem) {
                var onProductUpdated = function (updatedProduct) {
                    gridAPI.itemUpdated(updatedProduct);
                };

                Retail_BE_ProductService.editProduct(productItem.Entity.ProductId, onProductUpdated);
            }
            //function hasEditProductPermission() {
            //    return Retail_BE_ProductAPIService.HasUpdateProductPermission()
            //}
        }
    }]);
