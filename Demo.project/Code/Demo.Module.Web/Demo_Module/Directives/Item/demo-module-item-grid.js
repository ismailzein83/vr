"use strict"
app.directive("demoModuleItemGrid", ["UtilsService", "VRNotificationService", "Demo_Module_ItemAPIService", "Demo_Module_ItemService", "VRUIUtilsService", "VRCommon_ObjectTrackingService",
function (UtilsService, VRNotificationService, Demo_Module_ItemAPIService, Demo_Module_ItemService, VRUIUtilsService, VRCommon_ObjectTrackingService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var itemGrid = new ItemGrid($scope, ctrl, $attrs);
            itemGrid.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        templateUrl: "/Client/Modules/Demo_Module/Directives/Item/templates/ItemGridTemplate.html"
    };

    function ItemGrid($scope, ctrl) {

        var gridApi;
        var productId;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.scopeModel = {};

            $scope.scopeModel.items = [];

            $scope.scopeModel.onGridReady = function (api) {
                gridApi = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function") {
                    ctrl.onReady(getDirectiveApi());
                }

                function getDirectiveApi() {
                    var directiveApi = {};

                    directiveApi.load = function (payload) {
                        var query = payload.query;
                        productId = payload.productId;
                        if (payload.hideproductColumn)
                            $scope.scopeModel.hideproductColumn = payload.hideproductColumn;
                        return gridApi.retrieveData(query);
                    };

                    directiveApi.onItemAdded = function (item) {
                        gridApi.itemAdded(item);
                    };
                    return directiveApi;
                };
            };
            $scope.scopeModel.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) { // takes retrieveData object

                return Demo_Module_ItemAPIService.GetFilteredItems(dataRetrievalInput)
                .then(function (response) {
                    console.log(response);
                    onResponseReady(response);
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
                clicked: editItem,

            }];
        };
        function editItem(item) {
            var onItemUpdated = function (item) {
                gridApi.itemUpdated(item);
            };
            var productIdItem = productId != undefined ? { productId: productId } : undefined;
            Demo_Module_ItemService.editItem(item.ItemId, onItemUpdated, productIdItem);
        };


    };
    return directiveDefinitionObject;
}]);
