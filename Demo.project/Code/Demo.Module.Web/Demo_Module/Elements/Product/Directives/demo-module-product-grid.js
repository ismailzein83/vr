(function (app) {

    'use strict';

    productGrid.$inject = ['Demo_Module_ProductService', 'Demo_Module_ProductAPIService', 'VRNotificationService'];

    function productGrid(Demo_Module_ProductService, Demo_Module_ProductAPIService, VRNotificationService) {

        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var ctor = new productGridCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Product/Directives/Templates/ProductGridTemplate.html'
        };

        function productGridCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var manufactoryId;
            var gridAPI;

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.products = [];

                $scope.scopeModel.onProductGridReady = function (api) {
                    gridAPI = api;
                    defineAPI();
                };

                $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { //see retrieveData(query) in api.load
                    return Demo_Module_ProductAPIService.GetFilteredProducts(dataRetrievalInput).then(function (response) {
                        onResponseReady(response);
                    }).catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
                };

                defineMenuActions();
            }

            function defineAPI() {
                var api = {};
                var query;

                api.load = function (payload) {
                    
                    if (payload != undefined) {
                        query = payload.query;
                        manufactoryId = payload.manufactoryId;
                        $scope.scopeModel.hideManufactoryColumn = payload.hideManufactoryColumn;
                    }

                    return gridAPI.retrieveData(query);
                };

                api.onProductAdded = function (product) {
                    gridAPI.itemAdded(product);
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function defineMenuActions() {
                $scope.scopeModel.gridMenuActions = [{
                    name: 'Edit',
                    clicked: editProduct
                }];

                function editProduct(product) {
                    var onProductUpdated = function (product) {
                        gridAPI.itemUpdated(product);
                    };

                    var manufactoryIdItem;
                    if (manufactoryId != undefined) {
                        manufactoryIdItem = { manufactoryId: manufactoryId }; //sent from product search (caught by product service then by product editor)
                    }

                    Demo_Module_ProductService.editProduct(onProductUpdated, product.Id, manufactoryIdItem);
                }
            }
        }
    }

    app.directive('demoModuleProductGrid', productGrid);

})(app);