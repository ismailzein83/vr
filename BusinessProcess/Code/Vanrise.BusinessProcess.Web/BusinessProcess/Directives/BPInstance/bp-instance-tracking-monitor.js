"use strict";

app.directive("businessprocessBpInstanceTrackingMonitor", ["BusinessProcess_BPInstanceTrackingAPIService", "UtilsService", "BusinessProcess_GridMaxSize", "VRTimerService",
function (BusinessProcess_BPInstanceTrackingAPIService, UtilsService, BusinessProcess_GridMaxSize, VRTimerService) {

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
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPInstance/Templates/BPInstanceTrackingMonitorGridTemplate.html"

    };

    function BPInstanceGrid($scope, ctrl) {

        var lessThanID, greaterThanId, nbOfRows, bpInstanceId, severities;
        var input = {
            LessThanID: lessThanID,
            GreaterThanID: greaterThanId,
            NbOfRows: nbOfRows,
            BPInstanceID: bpInstanceId,
            Severities: severities
        };

        var gridAPI;
        this.initializeController = initializeController;

        $scope.loadMoreData = function () {
            return getData();
        };

        function loadFilters() {
            $scope.trackingSeverity = UtilsService.getLogEntryType();
        }

        function initializeController() {
            loadFilters();
            $scope.selectedTrackingSeverity = [];
            $scope.bpInstanceTracking = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        input.BPInstanceID = query.BPInstanceID;
                        $scope.selectedTrackingSeverity = query.Severities;
                        onInit();
                    };

                    directiveAPI.clearTimer = function () {
                        if ($scope.job) {
                            VRTimerService.unregisterJob($scope.job);
                        }
                    };
                    return directiveAPI;
                }

                $scope.searchClicked = function () {
                    onInit();
                };

                function onInit() {
                    $scope.isLoading = true;
                    input.LessThanID = undefined;
                    input.GreaterThanID = undefined;
                    input.NbOfRows = undefined;
                    $scope.bpInstanceTracking.length = 0;
                    input.Severities = UtilsService.getPropValuesFromArray($scope.selectedTrackingSeverity, "value");
                    createTimer();
                }

                function manipulateDataUpdated(response) {
                    var itemAddedOrUpdatedInThisCall = false;
                    if (response != undefined) {
                        for (var i = 0; i < response.ListBPInstanceTrackingDetails.length; i++) {
                            var bpInstanceTracking = response.ListBPInstanceTrackingDetails[i];

                            itemAddedOrUpdatedInThisCall = true;
                            $scope.bpInstanceTracking.push(bpInstanceTracking);

                        }

                        if (itemAddedOrUpdatedInThisCall) {
                            if ($scope.bpInstanceTracking.length > 0) {
                                $scope.bpInstanceTracking.sort(function (a, b) {
                                    return b.Entity.Id - a.Entity.Id;
                                });

                                if ($scope.bpInstanceTracking.length > BusinessProcess_GridMaxSize.maximumCount) {
                                    $scope.bpInstanceTracking.length = BusinessProcess_GridMaxSize.maximumCount;
                                }
                                input.LessThanID = $scope.bpInstanceTracking[$scope.bpInstanceTracking.length - 1].Entity.Id;
                                input.GreaterThanID = $scope.bpInstanceTracking[0].Entity.Id;
                            }
                        }
                    }
                }

                function createTimer() {
                    if ($scope.job) {
                        VRTimerService.unregisterJob($scope.job);
                    }
                    var pageInfo = gridAPI.getPageInfo();
                    input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                    VRTimerService.registerJob(onTimerElapsed, $scope);
                }

                function onTimerElapsed() {
                    return BusinessProcess_BPInstanceTrackingAPIService.GetUpdated(input).then(function (response) {
                        manipulateDataUpdated(response);
                        $scope.isLoading = false;
                    },
                     function (excpetion) {
                         console.log(excpetion);
                         $scope.isLoading = false;
                     });
                }
            };
        }

        $scope.getSeverityColor = function (dataItem, colDef) {
            return UtilsService.getLogEntryTypeColor(dataItem.Entity.Severity);
        };

        function getData() {

            var pageInfo = gridAPI.getPageInfo();
            input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
            return BusinessProcess_BPInstanceTrackingAPIService.GetBeforeId(input).then(function (response) {
                if (response != undefined && response) {
                    for (var i = 0; i < response.length; i++) {
                        var bpInstanceTracking = response[i];
                        $scope.bpInstanceTracking.push(bpInstanceTracking);

                    }
                    $scope.bpInstanceTracking.sort(function (a, b) {
                        return b.Entity.Id - a.Entity.Id;
                    });
                    input.LessThanID = $scope.bpInstanceTracking[$scope.bpInstanceTracking.length - 1].Entity.Id;
                    input.GreaterThanID = $scope.bpInstanceTracking[0].Entity.Id;
                }
            });
        }
    }
    return directiveDefinitionObject;

}]);