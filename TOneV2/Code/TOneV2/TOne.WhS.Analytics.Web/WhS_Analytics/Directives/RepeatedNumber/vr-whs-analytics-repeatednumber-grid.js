"use strict";

app.directive("vrWhsAnalyticsRepeatednumberGrid", ["UtilsService", "VRNotificationService", "WhS_Analytics_RepeatedNumberAPIService", "WhS_Analytics_GenericAnalyticService",
function (UtilsService, VRNotificationService, WhS_Analytics_RepeatedNumberAPIService, WhS_Analytics_GenericAnalyticService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var grid = new RepeatedNumberGrid($scope, ctrl, $attrs);
            grid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Analytics/Directives/RepeatedNumber/Templates/RepeatedNumberGridTemplate.html"

    };

    function RepeatedNumberGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.repeatedNumber = [];


            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        ctrl.parameters = query;
                        return gridAPI.retrieveData(query);
                    };

                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                ctrl.showGrid = true;
                return WhS_Analytics_RepeatedNumberAPIService.GetAllFilteredRepeatedNumbers(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .finally(function () {
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
           
            //$scope.gridMenuActions = [{
            //    name: "CDRs",
            //    clicked: function () {
            //        var parameters = {
            //            fromDate: UtilsService.cloneDateTime(ctrl.parameters.From),
            //            toDate: UtilsService.cloneDateTime(ctrl.parameters.To),
            //            customerIds: [],
            //            saleZoneIds: [],
            //            supplierIds: [],
            //            switchIds: ctrl.parameters.Filter.SwitchIds,
            //            supplierZoneIds: []
            //        };
            //        WhS_Analytics_GenericAnalyticService.showCdrLog(parameters);

            //    }
            //}];
        }
    }

    return directiveDefinitionObject;

}]);