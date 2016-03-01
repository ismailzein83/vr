"use strict";

app.directive("businessprocessBpInstanceMonitorGrid", ["BusinessProcess_BPInstanceAPIService", "BusinessProcess_BPInstanceService",
function (BusinessProcess_BPInstanceAPIService, BusinessProcess_BPInstanceService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var bpInstanceGrid = new BPInstanceGrid($scope, ctrl, $attrs);
            bpInstanceGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/BPInstanceMonitorGridTemplate.html"

    };

    function BPInstanceGrid($scope, ctrl) {

        var lastUpdateHandle, lessThanID, nbOfRows, definitionsId;
        var input = {
            LastUpdateHandle: lastUpdateHandle,
            LessThanID: lessThanID,
            NbOfRows: nbOfRows,
            DefinitionsId: definitionsId
        };

        var gridAPI;
        this.initializeController = initializeController;

        $scope.loadMoreData = function () {
            return getData();
        }

        var minId = undefined;

        function initializeController() {
            $scope.bpInstances = [];
            var timer;
            var isGettingData = false;
            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        input.LastUpdateHandle = undefined;
                        input.LessThanID = undefined;
                        input.NbOfRows = undefined;
                        input.DefinitionsId = query.DefinitionsId;
                        $scope.bpInstances.length = 0;
                        createTimer();
                    }
                    return directiveAPI;
                }

                function createTimer() {
                    if (timer != undefined) {
                        clearTimeout(timer);
                    }
                    timer = setInterval(function () {
                        if (!isGettingData) {
                            var pageInfo = gridAPI.getPageInfo();
                            input.NbOfRows = pageInfo.toRow - pageInfo.fromRow;
                            BusinessProcess_BPInstanceAPIService.GetUpdated(input).then(function (response) {
                                isGettingData = true;
                                if (response != undefined) {
                                    for (var i = 0; i < response.ListBPInstanceDetails.length; i++) {

                                        var bpInstance = response.ListBPInstanceDetails[i];

                                        var findBPInstance = false;

                                        for (var j = 0; j < $scope.bpInstances.length; j++) {
                                            //if (i === 1) {
                                            if (j === 0)
                                                minId = $scope.bpInstances[j].Entity.ProcessInstanceID;
                                            else {
                                                if ($scope.bpInstances[j].Entity.ProcessInstanceID < minId) {
                                                    minId = $scope.bpInstances[j].Entity.ProcessInstanceID;
                                                }
                                            }
                                            //}
                                            ///////////////////////////////////////////////////////////////////

                                            if ($scope.bpInstances[j].Entity.ProcessInstanceID == bpInstance.Entity.ProcessInstanceID) {
                                                $scope.bpInstances[j] = bpInstance;
                                                findBPInstance = true;
                                            }
                                            //////////////////////////////////////////////////////////////
                                        }
                                        if (input.LastUpdateHandle == undefined) {
                                            $scope.bpInstances.push(bpInstance);
                                        }
                                        else
                                            if (!findBPInstance)
                                                $scope.bpInstances.unshift(bpInstance);
                                    }
                                }
                                input.LastUpdateHandle = response.MaxTimeStamp;
                            })
                            .finally(function () {
                                isGettingData = false;
                            });
                        }
                    }, 2000);
                };


                $scope.$on("$destroy", function () {
                    if (timer != undefined) {
                        clearTimeout(timer);
                    }
                });
            };
        }

        $scope.getStatusColor = function (dataItem) {
            return BusinessProcess_BPInstanceService.getStatusColor(dataItem.Entity.Status);
        };

        $scope.processInstanceClicked = function (dataItem) {
            BusinessProcess_BPInstanceService.openProcessTracking(dataItem.Entity.ProcessInstanceID);
        }

        function getData() {
            
            var pageInfo = gridAPI.getPageInfo();
            input.LessThanID = minId;
            input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
            return BusinessProcess_BPInstanceAPIService.GetBeforeId(input).then(function (response) {
                if (response != undefined && response) {
                    for (var i = 0; i < response.length; i++) {
                        var bpInstance = response[i];
                        minId = response[i].Entity.ProcessInstanceID;
                        $scope.bpInstances.push(bpInstance);
                    }
                }
            });
        }
    }
    return directiveDefinitionObject;

}]);