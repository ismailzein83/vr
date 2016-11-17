"use strict";

app.directive("vrCommonLogEntryGrid", ["UtilsService", "VRNotificationService", "VRCommon_LogEntryAPIService", "VRCommon_LogEntryService",
function (UtilsService, VRNotificationService, VRCommon_LogEntryAPIService, VRCommon_LogEntryService) {

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
        templateUrl: "/Client/Modules/Common/Directives/Log/Templates/LogGridTemplate.html"

    };

    function LoggerGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {

            $scope.logs = [];
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
                return VRCommon_LogEntryAPIService.GetFilteredLogs(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            
        }

        $scope.getTypeColor = function (dataItem) {
            return VRCommon_LogEntryService.getTypeColor(dataItem.Entity.EntryType);
        };

    }

    return directiveDefinitionObject;

}]);
