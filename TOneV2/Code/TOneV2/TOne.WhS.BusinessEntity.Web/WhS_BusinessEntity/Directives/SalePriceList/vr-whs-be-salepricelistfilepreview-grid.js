"use strict";

app.directive("vrWhsBeSalepricelistfilepreviewGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePricelistAPIService", "FileAPIService"
    ,  "WhS_BE_SalePriceListChangeService",
function (utilsService, vrNotificationService, whSBeSalePricelistApiService, fileApiService, whSBeSalePriceListPreviewService) {

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
        var context;

        function initializeController() {
            $scope.salepricelistFilePreview = [];
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
        }
    }
    return directiveDefinitionObject;
}]);
