"use strict";

app.directive("businessprocessBpInstanceTrackingMonitor", ["BusinessProcess_BPInstanceTrackingAPIService", "UtilsService","BusinessProcess_GridMaxSize",
function (BusinessProcess_BPInstanceTrackingAPIService, UtilsService,BusinessProcess_GridMaxSize) {

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
        }

        function loadFilters() {
            $scope.trackingSeverity = UtilsService.getLogEntryType();
        }

        function initializeController() {
            loadFilters();
            $scope.selectedTrackingSeverity = [];
            $scope.bpInstanceTracking = [];
            var timer;
            var isGettingData = false;
            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        input.BPInstanceID = query.BPInstanceID;
                        onInit();
                    }

                    directiveAPI.clearTimer = function () {
                        if (timer != undefined) {
                            clearTimeout(timer);
                        }
                    }
                    return directiveAPI;
                }

                $scope.searchClicked = function () {
                    input.Severities = UtilsService.getPropValuesFromArray($scope.selectedTrackingSeverity, "value");
                    onInit();
                };

                function onInit() {
                    $scope.isLoading = true;
                    input.LessThanID = undefined;
                    input.GreaterThanID = undefined;
                    input.NbOfRows = undefined;
                    $scope.bpInstanceTracking.length = 0;
                    createTimer();
                }

                function createTimer() {
                    if (timer != undefined) {
                        clearTimeout(timer);
                    }
                    timer = setInterval(function () {
                        if (!isGettingData) {
                            isGettingData = true;
                            var pageInfo = gridAPI.getPageInfo();
                            input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
                            var itemAddedInThisCall = false;
                            BusinessProcess_BPInstanceTrackingAPIService.GetUpdated(input).then(function (response) {
                                if (response != undefined) {
                                    for (var i = 0; i < response.ListBPInstanceTrackingDetails.length; i++) {
                                        var bpInstanceTracking = response.ListBPInstanceTrackingDetails[i];

                                        itemAddedInThisCall = true;
                                        $scope.bpInstanceTracking.push(bpInstanceTracking);

                                    }

                                    if (itemAddedInThisCall) {
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
                            })
                            .finally(function () {
                                isGettingData = false;
                                $scope.isLoading = false;
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