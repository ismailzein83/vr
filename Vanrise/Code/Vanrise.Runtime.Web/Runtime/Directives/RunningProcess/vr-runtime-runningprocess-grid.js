"use strict";
app.directive("vrRuntimeRunningprocessGrid", ["VRNotificationService", "VRRuntime_RunningProcessAPIService",
function (VRNotificationService, VRRuntime_RunningProcessAPIService) {
    

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var runningProcessGrid = new RunningProcessGrid($scope, ctrl, $attrs);
            runningProcessGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Runtime/Directives/RunningProcess/Templates/RunningProcessTemplate.html"
    };

    function RunningProcessGrid($scope, ctrl, $attrs) {
        var gridAPI;
        this.initializeController = initializeController;

        function initializeController() {
            $scope.runningProcesses = [];

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

                return VRRuntime_RunningProcessAPIService.GetFilteredRunningProcesses(dataRetrievalInput)
                    .then(function (response) {
                        onResponseReady(response);
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
            defineAPI();
        }
        function defineAPI() {
            var api = {};
            api.getNewGridData = function () {
                // console.log("api.getNewGridData");
            };
        }
    }
    return directiveDefinitionObject;

}]);