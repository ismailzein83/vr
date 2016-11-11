"use strict";

app.directive("vrWhsBeSupplierpricelistGrid", ["UtilsService", "VRNotificationService", "FileAPIService", "WhS_BE_SupplierPriceListAPIService",
function (UtilsService, VRNotificationService, FileAPIService, WhS_BE_SupplierPriceListAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var supplierPriceListGrid = new SupplierPriceListGrid($scope, ctrl, $attrs);
            supplierPriceListGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SupplierPricelist/Templates/SupplierPricelistGridTemplate.html"

    };

    function SupplierPriceListGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var disabCountry;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.supplierPriceLists = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SupplierPriceListAPIService.GetFilteredSupplierPricelist(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineMenuActions();
        }



        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Download",
                clicked: downloadPriceList
            }];
        }

        function downloadPriceList(priceListObj) {
            FileAPIService.DownloadFile(priceListObj.Entity.FileId)
                    .then(function (response) {
                        UtilsService.downloadFile(response.data, response.headers);
                    });
        }

    }

    return directiveDefinitionObject;

}]);
