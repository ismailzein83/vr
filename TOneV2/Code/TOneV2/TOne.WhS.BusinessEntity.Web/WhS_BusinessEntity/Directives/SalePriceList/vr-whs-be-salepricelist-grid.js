"use strict";

app.directive("vrWhsBeSalepricelistGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePricelistAPIService", "FileAPIService",
function (UtilsService, VRNotificationService, WhS_BE_SalePricelistAPIService, FileAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SalePriceListGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceList/Templates/SalePriceListGridTemplate.html"

    };

    function SalePriceListGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.salepricelist = [];
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
                return WhS_BE_SalePricelistAPIService.GetFilteredSalePriceLists(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
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
