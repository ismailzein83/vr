"use strict";

app.directive("vrCpPricelistsGrid", ["UtilsService", "CP_SupplierPricelist_SupplierPriceListAPIService", "CP_SupplierPricelist_SupplierPriceListService", "VRUIUtilsService", "VRNotificationService",
function (utilsService, CP_SupplierPricelist_SupplierPriceListAPIService, CP_SupplierPricelist_SupplierPriceListService, vRuiUtilsService, vRNotificationService) {

    var gridAPI;

    function PricelistGrid($scope, ctrl) {
        function initializeController() {

            $scope.pricelist = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;


                function getDirectiveApi() {
                    var directiveApi = {};
                    directiveApi.loadGrid = function (query) {
                        return gridAPI.retrieveData(query);
                    }

                    return directiveApi;
                }

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveApi());
            }
        }

        this.initializeController = initializeController;;

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return CP_SupplierPricelist_SupplierPriceListAPIService.GetFilteredPriceLists(dataRetrievalInput)
               .then(function (response) {

                   onResponseReady(response);
               })
               .catch(function (error) {
                   vRNotificationService.notifyExceptionWithClose(error, $scope);
               });
        };

        $scope.getColorStatus = function (dataItem) {
            return CP_SupplierPricelist_SupplierPriceListService.getSupplierPriceListStatusColor(dataItem.Entity.Status);
        };

        $scope.getColorResult = function (dataItem) {
            return CP_SupplierPricelist_SupplierPriceListService.getSupplierPriceListResultColor(dataItem.Entity.Result);
        };
    }
    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var pricelistGrid = new PricelistGrid($scope, ctrl, $attrs);
            pricelistGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/CP_SupplierPriceList/Directives/SupplierPriceList/Templates/PriceListsGridTemplate.html"

    };
    return directiveDefinitionObject;

}]);