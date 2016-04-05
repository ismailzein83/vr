"use strict";

app.directive("vrCommonLogEntryGrid", ["UtilsService", "VRNotificationService", "VRCommon_LogEntryAPIService",
function (UtilsService, VRNotificationService, VRCommon_LogEntryAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var loggerGrid = new LoggerGrid($scope, ctrl, $attrs);
            loggerGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Common/Directives/Logger/Templates/LoggerGridTemplate.html"

    };

    function LoggerGrid($scope, ctrl, $attrs) {

        var gridAPI;
        var disabCountry;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.loggers = [];
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {

                        return gridAPI.retrieveData(query);
                    }
                    return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRCommon_LogEntryAPIService.GetFilteredLoggers(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            //defineMenuActions();
        }

        //function defineMenuActions() {
        //    $scope.gridMenuActions = [{
        //        name: "Download",
        //        clicked: downloadPriceList
        //    }];
        //}

    }

    return directiveDefinitionObject;

}]);
