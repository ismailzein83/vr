"use strict";

app.directive("vrWhsAnalyticsReleasecodeGrid", ["UtilsService", "VRNotificationService", "WhS_Analytics_ReleaseCodeAPIService",
function (UtilsService, VRNotificationService, WhS_Analytics_ReleaseCodeAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "=",
            dimenssion: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new Grid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Analytics/Directives/ReleaseCodeStatistic/Templates/ReleaseCodeGridTemplate.html"

    };

    function Grid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;
        ctrl.showDimessionCol = function (d) {

            return ctrl.dimenssion != undefined && ctrl.dimenssion.indexOf(d) > -1;
        };

        function initializeController() {

            $scope.blockedAttempts = [];
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
                 ctrl.showGrid = true;            
                return WhS_Analytics_ReleaseCodeAPIService.GetAllFilteredReleaseCodes(dataRetrievalInput)
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