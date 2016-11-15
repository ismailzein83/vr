"use strict";

app.directive("vrWhsAnalyticsRawcdrlogGrid", ["UtilsService", "VRNotificationService", "WhS_Analytics_RawCDRLogAPIService",
function (UtilsService, VRNotificationService, WhS_Analytics_RawCDRLogAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new RawCDRLogGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Analytics/Directives/RawCDR/Templates/RawCDRLogGridTemplate.html"

    };

    function RawCDRLogGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.rawCDRLog = [];
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
                ctrl.isLoadingGrid = true;
                ctrl.showGrid = true;
                return WhS_Analytics_RawCDRLogAPIService.GetRawCDRData(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .finally(function () {
                        ctrl.isLoadingGrid = false;
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }

    }

    return directiveDefinitionObject;

}]);