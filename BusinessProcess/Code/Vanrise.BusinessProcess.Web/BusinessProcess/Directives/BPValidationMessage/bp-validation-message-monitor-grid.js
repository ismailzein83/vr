"use strict";

app.directive("businessprocessBpValidationMessageMonitorGrid", ["BusinessProcess_BPValidationMessageAPIService", "UtilsService", "BusinessProcess_GridMaxSize", "VRTimerService","BusinessProcess_BPValidationMessageService",
function (BusinessProcess_BPValidationMessageAPIService, UtilsService, BusinessProcess_GridMaxSize, VRTimerService, BusinessProcess_BPValidationMessageService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var bpValidationMessageGrid = new BPValidationMessageGrid($scope, ctrl, $attrs);
            bpValidationMessageGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/BusinessProcess/Directives/BPValidationMessage/Templates/BPValidationMessageMonitorGridTemplate.html"

    };

    function BPValidationMessageGrid($scope, ctrl) {

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

        function initializeController() {
            $scope.bpValidationMessages = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());

                function getDirectiveAPI() {
                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        input.BPInstanceID = query.BPInstanceID;
                        onInit();
                    };

                    directiveAPI.clearTimer = function () {
                        if ($scope.job) {
                            VRTimerService.unregisterJob($scope.job);
                        }
                    };
                    return directiveAPI;
                }

                function onInit() {
                    $scope.isLoading = true;
                    input.LessThanID = undefined;
                    input.GreaterThanID = undefined;
                    input.NbOfRows = undefined;
                    $scope.bpValidationMessages.length = 0;
                    createTimer();
                }

                function manipulateDataUpdated(response) {
                    var itemAddedOrUpdatedInThisCall = false;
                    if (response != undefined) {
                        for (var i = 0; i < response.ListValidationMessageDetails.length; i++) {
                            var bpValidationMessage = response.ListValidationMessageDetails[i];

                            itemAddedOrUpdatedInThisCall = true;
                            $scope.bpValidationMessages.push(bpValidationMessage);

                        }

                        if (itemAddedOrUpdatedInThisCall) {
                            if ($scope.bpValidationMessages.length > 0) {
                                $scope.bpValidationMessages.sort(function (a, b) {
                                    return b.Entity.ValidationMessageId - a.Entity.ValidationMessageId;
                                });

                                if ($scope.bpValidationMessages.length > BusinessProcess_GridMaxSize.maximumCount) {
                                    $scope.bpValidationMessages.length = BusinessProcess_GridMaxSize.maximumCount;
                                }
                                input.LessThanID = $scope.bpValidationMessages[$scope.bpValidationMessages.length - 1].Entity.ValidationMessageId;
                                input.GreaterThanID = $scope.bpValidationMessages[0].Entity.ValidationMessageId;
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
                    return BusinessProcess_BPValidationMessageAPIService.GetUpdated(input).then(function (response) {
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
            return BusinessProcess_BPValidationMessageService.getSeverityColor(dataItem.Entity.Severity);
        };

        function getData() {

            var pageInfo = gridAPI.getPageInfo();
            input.NbOfRows = pageInfo.toRow - pageInfo.fromRow + 1;
            return BusinessProcess_BPValidationMessageAPIService.GetBeforeId(input).then(function (response) {
                if (response != undefined && response) {
                    for (var i = 0; i < response.length; i++) {
                        var bpValidationMessage = response[i];
                        $scope.bpValidationMessages.push(bpValidationMessage);

                    }
                    $scope.bpValidationMessages.sort(function (a, b) {
                        return b.Entity.ValidationMessageId - a.Entity.ValidationMessageId;
                    });
                    input.LessThanID = $scope.bpValidationMessages[$scope.bpValidationMessages.length - 1].Entity.ValidationMessageId;
                    input.GreaterThanID = $scope.bpValidationMessages[0].Entity.ValidationMessageId;
                }
            });
        }
    }
    return directiveDefinitionObject;

}]);