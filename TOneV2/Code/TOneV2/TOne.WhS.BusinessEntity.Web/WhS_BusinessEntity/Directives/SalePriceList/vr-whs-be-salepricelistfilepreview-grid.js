"use strict";

app.directive("vrWhsBeSalepricelistfilepreviewGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePriceListChangeAPIService",
function (utilsService, VRNotificationService, WhS_BE_SalePriceListChangeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            defaultSortDirection: "@",
            defaultSortByFieldName: "@"

        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SalePriceListFilePreviewGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceList/Templates/SalePricelistFilePreviewTemplate.html"

    };

    function SalePriceListFilePreviewGrid($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        var gridAPI;
        $scope.downloadPricelist = function (dataItem) {
            WhS_BE_SalePriceListChangeAPIService.DownloadSalePriceList(dataItem.FileId).then(function (bufferArrayRespone) {
                utilsService.downloadFile(bufferArrayRespone.data, bufferArrayRespone.headers);
            });
        };
        function initializeController() {
            ctrl.datasource = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (data) {
                        if (data != undefined) {
                            for (var i = 0; i < data.length; i++) {
                                ctrl.datasource.push(data[i]);
                            }
                        }
                    };
                    return directiveAPI;
                }
            };
        }

    }
    return directiveDefinitionObject;
}]);
