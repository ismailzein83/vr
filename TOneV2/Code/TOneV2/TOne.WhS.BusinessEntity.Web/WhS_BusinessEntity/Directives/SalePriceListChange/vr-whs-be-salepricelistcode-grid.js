"use strict";

app.directive("vrWhsBeSalepricelistcodeGrid", ["UtilsService", "VRNotificationService", "WhS_BE_SalePriceListChangeAPIService",
function (UtilsService, VRNotificationService, WhS_BE_SalePriceListChangeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new SaleCodeGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/SalePriceListChange/Templates/SalePricelistCodeTemplate.html"

    };

    function SaleCodeGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var gridQuery;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.salecodes = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (payload) {
                        gridQuery = payload.query;
                        return gridAPI.retrieveData(gridQuery);
                    };

                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return WhS_BE_SalePriceListChangeAPIService.GetSalePricelistCodes(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

    }

    return directiveDefinitionObject;

}]);
