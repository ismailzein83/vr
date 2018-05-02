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
        var testingChanges = 0;

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
                    //directiveAPI.getNewData = function (query) {


                    //    var dataRetrievalInput = {
                    //        Query: {
                    //            RuntimeNodeInstanceId: query.RuntimeNodeInstanceId
                    //        },
                    //        SortByColumnName: "RuntimeNodeId",
                    //        IsSortDescending: false,
                    //        ResultKey: null,
                    //        DataRetrievalResultType: 0,
                    //        FromRow: 1,
                    //        ToRow: 40
                    //    };

                    //    //testingChanges++;
                    //    VRRuntime_RunningProcessAPIService.GetFilteredRunningProcesses(dataRetrievalInput)
                    //            .then(function (response) {
                    //                for (var i = 0; i < $scope.runningProcesses.length; i++) {
                    //                    for (var key in $scope.runningProcesses[i]) {
                    //                       //  $scope.runningProcesses[i][key] = testingChanges+i;
                    //                       $scope.runningProcesses[i][key] = response.Data[i][key];
                    //                    }
                    //                }
                    //            });
                    //};

                  return directiveAPI;
                }
            };
            $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
                return VRRuntime_RunningProcessAPIService.GetFilteredRunningProcesses(dataRetrievalInput)
                    .then(function (response) {
                        if ($scope.runningProcesses.length > 0)
                        {
                            for (var i = 0; i < $scope.runningProcesses.length; i++) {
                                for (var key in $scope.runningProcesses[i]) {
                                    //  $scope.runningProcesses[i][key] = testingChanges+i;
                                    $scope.runningProcesses[i][key] = response.Data[i][key];
                                }
                            }
                        } else
                        {
                            onResponseReady(response);
                        }
                    })
                    .catch(function (error) {
                        VRNotificationService.notifyException(error, $scope);
                    });
            };
        }
    }
    return directiveDefinitionObject;

}]);