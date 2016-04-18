"use strict";

app.directive("vrCpPricelistsGrid", ["UtilsService", "CP_SupplierPricelist_SupplierPriceListAPIService", "CP_SupplierPricelist_SupplierPriceListService", "VRUIUtilsService", "VRNotificationService",
function (utilsService, supplierApiService, supplierService, vRuiUtilsService, vRNotificationService) {

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
            return supplierApiService.GetFilteredPriceLists(dataRetrievalInput)
               .then(function (response) {

                   onResponseReady(response);
               })
               .catch(function (error) {
                   vRNotificationService.notifyExceptionWithClose(error, $scope);
               });
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