"use strict";
app.directive("vrCpCustomermanagmentGrid", ["UtilsService", "CP_SupplierPricelist_CustomerManagmentAPIService",
function (UtilsService, customerManagmentApiService) {

    function customerManagmentGrid($scope, ctrl) {

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

                        customerManagmentApiService.GetCustomers(input).then(function (response) {

                            if (response != undefined) {
                                for (var i = 0; i < response.Customers.length; i++) {
                                    var customer = response.Customers[i];
                                    var findpCustomer = false;
                                    for (var j = 0; j < $scope.Customers.length; j++) {

                                        //Get the minimun ID Test Call to send as parameter to getData();
                                        if (i == 0) {//just in the first check all test calls list
                                            if (j == 0)
                                                minId = $scope.Customers[j].CustomerId;
                                            else {
                                                if ($scope.Customers[j].CustomerId < minId) {
                                                    minId = $scope.Customers[j].CustomerId;
                                                }
                                            }
                                        }
                                        ///////////////////////////////////////////////////////////////////

                                        //Check if this test call exist in test call Details, if a new call
                                        // then unshift in the list(put the item in the top of the list)
                                        if ($scope.Customers[j].CustomerId == customer.CustomerId) {
                                            $scope.Customers[j] = customer;
                                            findpCustomer = true;
                                        }
                                        //////////////////////////////////////////////////////////////
                                    }
                                    if (input.LastUpdateHandle == undefined) {
                                        $scope.Customers.push(customer);
                                    }
                                    else
                                        if (!findpCustomer)
                                            $scope.Customers.unshift(customer);
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
    }

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var customerManagmentGrid = new customerManagmentGrid($scope, ctrl, $attrs);
            customerManagmentGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/CP_SupplierPriceList/Directives/Customer/Templates/CustomerManagmentGridTemplate.html"

    };
    return directiveDefinitionObject;

}]);