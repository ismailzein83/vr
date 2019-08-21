(function (app) {

    'use strict';

    productSearch.$inject = ['UtilsService', 'Demo_Module_ProductService'];

    function productSearch(UtilsService, Demo_Module_ProductService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope) {
                var ctrl = this;
                var ctor = new productSearchCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/Demo_Module/Elements/Product/Directives/Templates/ProductSearchTemplate.html'
        };

        function productSearchCtor($scope, ctrl) {
            this.initializeController = initializeController;

            var manufactoryId;
            var productGridAPI;

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.addProductClicked = function () {
                    var manufactoryItem = {
                        manufactoryId: manufactoryId
                    };
                    var onProductAdded = function (product) {
                        productGridAPI.onProductAdded(product);
                    };

                    Demo_Module_ProductService.addProduct(onProductAdded, manufactoryItem);
                };

                $scope.scopeModel.onGridReady = function (api) {
                    productGridAPI = api;
                    defineAPI();
                };
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    
                    if (payload != undefined) {
                        manufactoryId = payload.manufactoryId;
                    }
                    
                    return productGridAPI.load(getGridPayload());
                };

                if (ctrl.onReady != undefined && typeof(ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            function getGridPayload() {
                var payload = {
                    query: {
                        ManufactoryIds: [manufactoryId]
                    },
                    manufactoryId: manufactoryId, //send current manufactory id to disable selector in edit mode from manufactory grid (caught by product grid)
                    hideManufactoryColumn: true
                };

                return payload;
            }
        }

    }

    app.directive('demoModuleProductSearch', productSearch);

})(app);