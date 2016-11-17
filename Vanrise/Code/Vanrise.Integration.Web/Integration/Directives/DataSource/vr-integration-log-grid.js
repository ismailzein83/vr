"use strict";

app.directive("vrIntegrationLogGrid", ["UtilsService", "VRNotificationService", "VR_Integration_DataSourceLogAPIService",
function (UtilsService, VRNotificationService, VR_Integration_DataSourceLogAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var logGrid = new LogGrid($scope, ctrl, $attrs);
            logGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/Integration/Directives/DataSource/Templates/LogGridTemplate.html"

    };

    function LogGrid($scope, ctrl, $attrs) {

        var gridAPI;
        this.initializeController = initializeController;
        function initializeController() {

            $scope.getSeverityColor = function (dataItem, colDef) {
                return UtilsService.getLogEntryTypeColor(dataItem.Severity);
            };
            $scope.logs = [];
            $scope.gridReady = function (api) {

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

                return VR_Integration_DataSourceLogAPIService.GetFilteredDataSourceLogs(dataRetrievalInput)
                    .then(function (response) {

                        angular.forEach(response.Data, function (item) {
                            item.SeverityDescription = UtilsService.getLogEntryTypeDescription(item.Severity);
                        });
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyExceptionWithClose(error, $scope);
                    });
            };

        }

    }

    return directiveDefinitionObject;

}]);
