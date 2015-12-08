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

        var gridAPI;
        var gridDrillDownTabsObj;
        this.initializeController = initializeController;
        $scope.arrayCallTestStatus = [];
        $scope.arrayCallTestStatus.push(Qm_CliTester_CallTestStatusEnum.New);
        $scope.arrayCallTestStatus.push(Qm_CliTester_CallTestStatusEnum.Initiated);
        $scope.arrayCallTestStatus.push(Qm_CliTester_CallTestStatusEnum.InitiationFailedWithRetry);
        $scope.arrayCallTestStatus.push(Qm_CliTester_CallTestStatusEnum.PartiallyCompleted);
        $scope.arrayCallTestStatus.push(Qm_CliTester_CallTestStatusEnum.GetProgressFailedWithRetry);
        $scope.arrayCallTestStatus.push(Qm_CliTester_CallTestStatusEnum.Completed);
        $scope.arrayCallTestStatus.push(Qm_CliTester_CallTestStatusEnum.InitiationFailedWithNoRetry);
        $scope.arrayCallTestStatus.push(Qm_CliTester_CallTestStatusEnum.GetProgressFailedWithNoRetry);
        
        $scope.arrayCallTestResult = [];
        $scope.arrayCallTestResult.push(Qm_CliTester_CallTestResultEnum.NotCompleted);
        $scope.arrayCallTestResult.push(Qm_CliTester_CallTestResultEnum.Succeeded);
        $scope.arrayCallTestResult.push(Qm_CliTester_CallTestResultEnum.PartiallySucceeded);
        $scope.arrayCallTestResult.push(Qm_CliTester_CallTestResultEnum.Failed);
        $scope.arrayCallTestResult.push(Qm_CliTester_CallTestResultEnum.NotAnswered);

        console.log($scope.arrayCallTestResult);
        console.log($scope.arrayCallTestStatus);
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
                    directiveAPI.loadGrid = function(query) {
                        //setInterval(function () { return gridAPI.retrieveData(query); }, 2000);
                        return gridAPI.retrieveData(query);
                    };
                    return directiveAPI;
                }
                   
            };
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return Qm_CliTester_TestCallAPIService.GetFilteredTestCalls(dataRetrievalInput)
                .then(function (response) {

                    if (response.Data != undefined) {
                        for (var i = 0; i < response.Data.length; i++) {
                            response.Data[i].CallTestStatusDescription = setEnumDescription(response.Data[i].CallTestStatus, Qm_CliTester_CallTestStatusEnum);
                            response.Data[i].CallTestResultDescription = setEnumDescription(response.Data[i].CallTestResult, Qm_CliTester_CallTestResultEnum);
                            gridDrillDownTabsObj.setDrillDownExtensionObject(response.Data[i]);
                        }
                    }
                    onResponseReady(response);

                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        };

        $scope.getColor = function (dataItem, coldef) {
            return getMeasureColor(dataItem, coldef);
        };
    }
    function setEnumDescription(value, varEnum) {
        for (var p in varEnum)
            if (varEnum[p].value == value)
                return varEnum[p].description;
    }
    function getCallTestStatusColor(value) {
        if (value == Qm_CliTester_CallTestStatusEnum.New.value)
            return LabelColorsEnum.New.color;

        if (value == Qm_CliTester_CallTestStatusEnum.Initiated.value)
            return LabelColorsEnum.Primary.color;

        if (value == Qm_CliTester_CallTestStatusEnum.InitiationFailedWithRetry.value)
            return LabelColorsEnum.Warning.color;
        if (value == Qm_CliTester_CallTestStatusEnum.InitiationFailedWithNoRetry.value)
            return LabelColorsEnum.WarningLevel2.color;


        if (value == Qm_CliTester_CallTestStatusEnum.PartiallyCompleted.value)
            return LabelColorsEnum.Processing.color;
        if (value == Qm_CliTester_CallTestStatusEnum.Completed.value)
            return LabelColorsEnum.Success.color;


        if (value == Qm_CliTester_CallTestStatusEnum.GetProgressFailedWithRetry.value)
            return LabelColorsEnum.WarningLevel1.color;
        if (value == Qm_CliTester_CallTestStatusEnum.GetProgressFailedWithNoRetry.value)
            return LabelColorsEnum.Failed.color;
        return undefined;
    }

    function getCallTestResultColor(value) {
        if (value == Qm_CliTester_CallTestResultEnum.NotCompleted.value)
            return LabelColorsEnum.Processing.color;

        if (value == Qm_CliTester_CallTestResultEnum.Succeeded.value)
            return LabelColorsEnum.Success.color;

        if (value == Qm_CliTester_CallTestResultEnum.PartiallySucceeded.value)
            return LabelColorsEnum.WarningLevel1.color;

        if (value == Qm_CliTester_CallTestResultEnum.Failed.value)
            return LabelColorsEnum.Failed.color;

        if (value == Qm_CliTester_CallTestResultEnum.NotAnswered.value)
            return LabelColorsEnum.Warning.color;
        return undefined;
    }

    function getMeasureColor(dataItem, coldef) {
        if (coldef.tag === "CallTestStatus")
            return getCallTestStatusColor(dataItem.CallTestStatus);
        else if (coldef.tag === "CallTestResult")
            return getCallTestResultColor(dataItem.CallTestResult);
        return undefined;
    }

    return directiveDefinitionObject;

}]);