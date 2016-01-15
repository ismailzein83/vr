"use strict";

app.directive("vrCpSupplierpricelistGrid", ["UtilsService", "CP_SupplierPricelist_SupplierPriceListAPIService", "CP_SupplierPricelist_SupplierPriceListService",
function (UtilsService, SupplierPriceListAPIService, SupplierPriceListService) {

    function SupplierPriceListGrid($scope, ctrl) {

        var lastUpdateHandle, lessThanID, nbOfRows;
        var input = {
            LastUpdateHandle: lastUpdateHandle,
            LessThanID: lessThanID,
            NbOfRows: nbOfRows
        };

        var gridAPI;
        var minId = undefined;

        function initializeController() {

            $scope.pricelist = [];
            var isGettingData = false;
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var timer = setInterval(function () {
                    if (!isGettingData) {
                        isGettingData = true;
                        var pageInfo = gridAPI.getPageInfo();
                        input.NbOfRows = pageInfo.toRow - pageInfo.fromRow;

                        SupplierPriceListAPIService.GetUpdated(input).then(function (response) {
                         
                            if (response != undefined) {
                                for (var i = 0; i < response.PriceLists.length; i++) {
                                    var pricelist = response.PriceLists[i];
                                    var findpricelist = false;
                                    for (var j = 0; j < $scope.pricelist.length; j++) {

                                        //Get the minimun ID Test Call to send as parameter to getData();
                                        if (i == 0) {//just in the first check all test calls list
                                            if (j == 0)
                                                minId = $scope.pricelist[j].PriceListId;
                                            else {
                                                if ($scope.pricelist[j].PriceListId < minId) {
                                                    minId = $scope.pricelist[j].PriceListId;
                                                }
                                            }
                                        }
                                        ///////////////////////////////////////////////////////////////////

                                        //Check if this test call exist in test call Details, if a new call
                                        // then unshift in the list(put the item in the top of the list)
                                        if ($scope.pricelist[j].PriceListId == pricelist.PriceListId) {
                                            $scope.pricelist[j] = pricelist;
                                            findpricelist = true;
                                        }
                                        //////////////////////////////////////////////////////////////
                                    }
                                    if (input.LastUpdateHandle == undefined) {
                                        $scope.pricelist.push(pricelist);
                                    }
                                    else
                                        if (!findpricelist)
                                            $scope.pricelist.unshift(pricelist);
                                }
                            }
                            input.LastUpdateHandle = response.MaxTimeStamp;
                        })
                            .finally(function () {
                                isGettingData = false;
                            });
                    }
                }, 2000);

                $scope.$on("$destroy", function () {
                    clearTimeout(timer);
                });
            };
        }

        this.initializeController = initializeController;

     
        function getData() {

            //var pageInfo = gridAPI.getPageInfo();
            //input.LessThanID = minId;
            //input.NbOfRows = pageInfo.toRow - pageInfo.fromRow;
            //return SupplierPriceListAPIService.GetBeforeId(input).then(function (response) {
            //    if (response != undefined) {
            //        for (var i = 0; i < response.PriceLists.length; i++) {
            //            var testCall = response.PriceLists[i];
            //            $scope.testcalls.push(testCall);
            //        }
            //    }
            //});
        }

        $scope.loadMoreData = function () {
            return getData();
        }
        $scope.getColorStatus = function (dataItem) {
            return SupplierPriceListService.getSupplierPriceListStatusColor(dataItem.Status);
        };

        $scope.getColorResult = function (dataItem) {
            return SupplierPriceListService.getSupplierPriceListResultColor(dataItem.Result);
        };
    }

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
        templateUrl: "/Client/Modules/CP_SupplierPriceList/Directives/SupplierPriceList/Templates/SupplierPriceListGridTemplate.html"

    };
    return directiveDefinitionObject;

}]);