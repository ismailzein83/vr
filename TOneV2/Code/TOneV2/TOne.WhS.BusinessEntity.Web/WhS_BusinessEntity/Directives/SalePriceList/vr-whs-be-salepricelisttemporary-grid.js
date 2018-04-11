"use strict";

app.directive("vrWhsBeSalepricelisttemporaryGrid", ["UtilsService", "VRNotificationService", "FileAPIService"
                                           , "WhS_BE_SalePriceListChangeService", "WhS_BE_SalePriceListChangeAPIService",
function (utilsService, vrNotificationService, fileApiService, WhS_BE_SalePriceListChangeService, WhS_BE_SalePriceListChangeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            defaultSortDirection: "@",
            defaultSortByFieldName: "@"

        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SalePriceListTemporaryGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceList/Templates/SalePriceListGridTemporaryTemplate.html"

    };

    function SalePriceListTemporaryGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;

        function initializeController() {
            $scope.salePriceLists = [];
            defineMenuActions();

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.load = function (payload) {
                        if(payload != undefined)
                        return gridAPI.retrieveData(payload.query);
                    };
                    directiveAPI.getData = function () {
                       
                    };
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SalePriceListChangeAPIService.GetFilteredTemporarySalePriceLists(dataRetrievalInput)
                    .then(function (response) {
                        console.log(response);
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        vrNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };
        }
        function defineMenuActions() {
            $scope.gridMenuActions = function (dataItem) {
                var menuActions = [];
                var sourceId = dataItem.Entity.SourceId;
                if (dataItem.Entity.FileId !== 0) {
                    var downloadPricelist = {
                        name: "Download",
                        clicked: downloadPriceList
                    };
                    menuActions.push(downloadPricelist);
                }
                return menuActions;
            };
        }

        function downloadPriceList(priceListObj) {
            fileApiService.DownloadFile(priceListObj.Entity.FileId)
                    .then(function (response) {
                        utilsService.downloadFile(response.data, response.headers);
                    });
        }
    }

    return directiveDefinitionObject;

}]);
