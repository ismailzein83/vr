"use strict";

app.directive("vrQmClitesterTestcalldetails", ["UtilsService", "VRNotificationService", "Qm_CliTester_TestCallAPIService", 
function (UtilsService, VRNotificationService, Qm_CliTester_TestCallAPIService) {

    var directiveDefinitionObject = {

        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var testCallDetailsGrid = new TestCallDetailsGrid($scope, ctrl, $attrs);
            testCallDetailsGrid.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/QM_CLITester/Directives/TestCall/Templates/TestCallDetailsTemplate.html"

    };

    function TestCallDetailsGrid($scope, ctrl, $attrs) {
        this.initializeController = initializeController;
        function initializeController() {
            var directiveAPI = {};
            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());
            function getDirectiveAPI() {
               
                directiveAPI.load = function (item) {
                    if (item.InitiateTestInformation != null && item.InitiateTestInformation != undefined) {
                        $scope.testId = item.InitiateTestInformation.Test_ID;
                    }
                    else {
                        $scope.testId = "";
                    }

                    if (item.TestProgress != null && item.TestProgress != undefined) {
                        $scope.name = item.TestProgress.Name;
                        $scope.callTotal = item.TestProgress.Calls_Total;
                        $scope.callComplete = item.TestProgress.Calls_Complete;
                        $scope.cliSuccess = item.TestProgress.CLI_Success;
                        $scope.cliNoResult = item.TestProgress.CLI_No_Result;
                        $scope.fail = item.TestProgress.CLI_Fail;
                        $scope.pdd = item.TestProgress.PDD;
                    }
                    else {
                        $scope.name = "";
                        $scope.callTotal = "";
                        $scope.callComplete = "";
                        $scope.cliSuccess = "";
                        $scope.cliNoResult = "";
                        $scope.fail = "";
                        $scope.pdd = "";
                    }
                }

                return directiveAPI;
            }
        }
    }

    return directiveDefinitionObject;

}]);
