"use strict";

app.directive("vrQmClitesterTestcallGrid", ["UtilsService", "VRNotificationService", "Qm_CliTester_TestCallAPIService", 'Qm_CliTester_TestCallService', 'VRUIUtilsService', 'LabelColorsEnum', 'Qm_CliTester_CallTestResultEnum', 'Qm_CliTester_CallTestStatusEnum',
function (UtilsService, VRNotificationService, Qm_CliTester_TestCallAPIService, Qm_CliTester_TestCallService, VRUIUtilsService, LabelColorsEnum, Qm_CliTester_CallTestResultEnum, Qm_CliTester_CallTestStatusEnum) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var testCallGrid = new TestCallGrid($scope, ctrl, $attrs);
            testCallGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/QM_CLITester/Directives/TestCall/Templates/TestCallGridTemplate.html"

    };

    function TestCallGrid($scope, ctrl) {

        var lastUpdateHandle, lessThanID, nbOfRows;
        var input = {
            LastUpdateHandle: lastUpdateHandle,
            LessThanID: lessThanID,
            NbOfRows: nbOfRows
        };

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        $scope.arrayCallTestStatus = UtilsService.getArrayEnum(Qm_CliTester_CallTestStatusEnum);
        $scope.arrayCallTestResult = UtilsService.getArrayEnum(Qm_CliTester_CallTestResultEnum);

        $scope.loadMoreData = function () {
            return getData();
        }

        var minId = undefined;
        function initializeController() {

            var drillDownDefinitions = Qm_CliTester_TestCallService.getDrillDownDefinition();
            gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);

            $scope.testcalls = [];
            var isGettingData = false;
            $scope.onGridReady = function (api) {
                gridAPI = api;

                var timer = setInterval(function () {
                    if (!isGettingData) {
                        var pageInfo = gridAPI.getPageInfo();
                        input.NbOfRows = pageInfo.toRow - pageInfo.fromRow;
                        
                        Qm_CliTester_TestCallAPIService.GetUpdated(input).then(function (response) {
                            isGettingData = true;
                            if (response != undefined) {
                                for (var i = 0; i < response.ListTestCallDetails.length; i++) {
                                    var testCall = response.ListTestCallDetails[i];

                                    gridDrillDownTabsObj.setDrillDownExtensionObject(testCall);
                                    var findTestCall = false;
                                    for (var j = 0; j < $scope.testcalls.length; j++) {

                                        //Get the minimun ID Test Call to send as parameter to getData();
                                        if (i == 0) {//just in the first check all test calls list
                                            if (j == 0)
                                                minId = $scope.testcalls[j].Entity.ID;
                                            else {
                                                if ($scope.testcalls[j].Entity.ID < minId) {
                                                    minId = $scope.testcalls[j].Entity.ID;
                                                }
                                            }
                                        }
                                        ///////////////////////////////////////////////////////////////////


                                        //Check if this test call exist in test call Details, if a new call
                                        // then unshift in the list(put the item in the top of the list)
                                        if ($scope.testcalls[j].Entity.ID == testCall.Entity.ID) {
                                            $scope.testcalls[j] = testCall;
                                            findTestCall = true;
                                        }
                                        //////////////////////////////////////////////////////////////
                                    }
                                    if (input.LastUpdateHandle == undefined) {
                                        $scope.testcalls.push(testCall);
                                    }
                                    else
                                        if (!findTestCall)
                                            $scope.testcalls.unshift(testCall);
                                }
                            }
                            input.LastUpdateHandle = response.MaxTimeStamp;
                            isGettingData = false;
                        });
                    }
                }, 2000);

                $scope.$on("$destroy", function () {
                    clearTimeout(timer);
                });
            };
        }

        function getData() {

            var pageInfo = gridAPI.getPageInfo();
            input.LessThanID = minId;
            input.NbOfRows = pageInfo.toRow - pageInfo.fromRow;
            return Qm_CliTester_TestCallAPIService.GetBeforeId(input).then(function (response) {
                if (response != undefined) {
                    for (var i = 0; i < response.ListTestCallDetails.length; i++) {
                        var testCall = response.ListTestCallDetails[i];
                        $scope.testcalls.push(testCall);
                    }
                }
            });
        }

        $scope.getColor = function (dataItem, coldef) {
            return getMeasureColor(dataItem, coldef);
        };
    }

    function getCallTestStatusColor(value) {
        switch(value) {
            case Qm_CliTester_CallTestStatusEnum.New.value:
                return LabelColorsEnum.New.color;
                break;
            case Qm_CliTester_CallTestStatusEnum.Initiated.value:
                return LabelColorsEnum.Primary.color;
                break;
            case Qm_CliTester_CallTestStatusEnum.InitiationFailedWithRetry.value:
                return LabelColorsEnum.Warning.color;
                break;
            case Qm_CliTester_CallTestStatusEnum.InitiationFailedWithNoRetry.value:
                return LabelColorsEnum.WarningLevel2.color;
                break;
            case Qm_CliTester_CallTestStatusEnum.PartiallyCompleted.value:
                return LabelColorsEnum.Processing.color;
                break;
            case Qm_CliTester_CallTestStatusEnum.Completed.value:
                return LabelColorsEnum.Success.color;
                break;
            case Qm_CliTester_CallTestStatusEnum.GetProgressFailedWithRetry.value:
                return LabelColorsEnum.WarningLevel1.color;
                break;
            case Qm_CliTester_CallTestStatusEnum.GetProgressFailedWithNoRetry.value:
                return LabelColorsEnum.Failed.color;
                break;
            default:
                return undefined;
        }
    }

    function getCallTestResultColor(value) {
        switch (value) {
            case Qm_CliTester_CallTestResultEnum.NotCompleted.value:
                return LabelColorsEnum.Error.color;
                break;
            case Qm_CliTester_CallTestResultEnum.Succeeded.value:
                return LabelColorsEnum.Success.color;
                break;
            case Qm_CliTester_CallTestResultEnum.PartiallySucceeded.value:
                return LabelColorsEnum.WarningLevel1.color;
                break;
            case Qm_CliTester_CallTestResultEnum.Failed.value:
                return LabelColorsEnum.Failed.color;
                break;
            case Qm_CliTester_CallTestResultEnum.NotAnswered.value:
                return LabelColorsEnum.Warning.color;
                break;
            default:
                return undefined;
        }
    }

    function getMeasureColor(dataItem, coldef) {
        if (coldef.tag === "CallTestStatus")
            return getCallTestStatusColor(dataItem.Entity.CallTestStatus);
        else if (coldef.tag === "CallTestResult")
            return getCallTestResultColor(dataItem.Entity.CallTestResult);
        return undefined;
    }

    return directiveDefinitionObject;

}]);