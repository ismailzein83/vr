"use strict";

app.directive("demoModuleItemSearch", ['VRNotificationService', 'UtilsService', 'VRUIUtilsService', 'VRValidationService', 'Demo_Module_ItemService',
function (VRNotificationService, UtilsService, VRUIUtilsService, VRValidationService, Demo_Module_ItemService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new ItemSearch($scope, ctrl, $attrs);
            ctor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Demo_Module/Directives/Item/Templates/ItemSearchTemplate.html"

    };

    function ItemSearch($scope, ctrl, $attrs) {

        var gridAPI;
        var productId;

        this.initializeController = initializeController;
        var context;
        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.addItem = function () {
                var productIdItem = { ProductId: productId };
                var onItemAdded = function (obj) {
                    gridAPI.onItemAdded(obj);
                };
                Demo_Module_ItemService.addItem(onItemAdded, productIdItem);
            };

            $scope.scopeModel.onGridReady = function (api) {
                gridAPI = api;
                defineAPI();
            };
        }

        function defineAPI() {
            var api = {};

            api.load = function (payload) {
                if (payload != undefined) {
                    productId = payload.productId;
                }
                return gridAPI.load(getGridQuery());
            };
            api.onItemAdded = function (itemObject) {
                gridAPI.onItemAdded(itemObject);
            };


            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(api);
        }

        function getGridQuery() {
            var payload = {
                query: { ProductIds: [productId] },
                productId: productId,
                hideProductColumn: true
            };
            return payload;
        }
    }

    return directiveDefinitionObject;

}]);
