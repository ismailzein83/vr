"use strict";

app.directive("vrQmClitesterHistestcallGrid", ["UtilsService", "VRNotificationService", "Qm_CliTester_TestCallAPIService", 'Qm_CliTester_TestCallService', 'VRUIUtilsService', 'Qm_CliTester_CallTestResultEnum', 'Qm_CliTester_CallTestStatusEnum',
function (UtilsService, VRNotificationService, Qm_CliTester_TestCallAPIService, Qm_CliTester_TestCallService, VRUIUtilsService, Qm_CliTester_CallTestResultEnum, Qm_CliTester_CallTestStatusEnum) {

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
        templateUrl: "/Client/Modules/QM_CLITester/Directives/TestCall/Templates/HisTestCallGridTemplate.html"

    };

    function TestCallGrid($scope, ctrl) {

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;

        $scope.arrayCallTestStatus = UtilsService.getArrayEnum(Qm_CliTester_CallTestStatusEnum);
        $scope.arrayCallTestResult = UtilsService.getArrayEnum(Qm_CliTester_CallTestResultEnum);


        function initializeController() {

            var drillDownDefinitions = Qm_CliTester_TestCallService.getDrillDownDefinition();
            gridDrillDownTabsObj = VRUIUtilsService.defineGridDrillDownTabs(drillDownDefinitions, gridAPI, $scope.gridMenuActions);


            $scope.testcalls = [];

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
            defineMenuActions();
        }

        function defineMenuActions() {
            $scope.gridMenuActions = [{
                name: "Send mail",
                clicked: sendTestCall
            }];
        }

        function sendTestCall(testCallObj) {
            var onSendTestCall = function (testCallObj) {
                gridDrillDownTabsObj.setDrillDownExtensionObject(testCallObj);
                gridAPI.itemUpdated(testCallObj);
            }

            var source = "";
            var destination = "";
            var receivedCli = "";
            var releaseCode = "";
            if (testCallObj.Entity.TestProgress != null && testCallObj.Entity.TestProgress.CallResults != null)
                for (var i = 0; i < testCallObj.Entity.TestProgress.CallResults.length; i++) {
                    source = source + ";" + testCallObj.Entity.TestProgress.CallResults[i].Source;
                    destination = destination + ";" + testCallObj.Entity.TestProgress.CallResults[i].Destination;
                    receivedCli = receivedCli + ";" + testCallObj.Entity.TestProgress.CallResults[i].ReceivedCli;
                    releaseCode = releaseCode + ";" + testCallObj.Entity.TestProgress.CallResults[i].ReleaseCode;
                }

            Qm_CliTester_TestCallService.sendTestCall(testCallObj.SupplierName, testCallObj.UserName, testCallObj.CountryName, testCallObj.ZoneName,
                testCallObj.CallTestStatusDescription, testCallObj.CallTestResultDescription, testCallObj.ScheduleName, testCallObj.Entity.Measure.Pdd,
                testCallObj.Entity.Measure.Mos, testCallObj.Entity.CreationDate, source, destination, receivedCli, releaseCode,
                testCallObj.Entity.Measure.RingDuration, testCallObj.Entity.Measure.Duration, onSendTestCall);
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return Qm_CliTester_TestCallAPIService.GetFilteredTestCalls(dataRetrievalInput)
                .then(function (response) {
                    if (response.Data != undefined) {
                        for (var i = 0; i < response.Data.length; i++) {
                            if (response.Data[i].Entity.BatchNumber === 0)
                                response.Data[i].Entity.BatchNumber = "";
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                        }
                    }
                    onResponseReady(response);

                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        };

        $scope.getColorStatus = function (dataItem) {
            return Qm_CliTester_TestCallService.getCallTestStatusColor(dataItem.Entity.CallTestStatus);
        };

        $scope.getColorResult = function (dataItem) {
            return Qm_CliTester_TestCallService.getCallTestResultColor(dataItem.Entity.CallTestResult);
        };
    }

    return directiveDefinitionObject;

}]);