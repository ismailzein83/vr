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
                    if (item.Entity.InitiateTestInformation != null && item.Entity.InitiateTestInformation != undefined) {
                        ctrl.testId = item.Entity.InitiateTestInformation.Test_ID;
                    }

                    if (item.Entity.TestProgress != null && item.Entity.TestProgress != undefined) {
                        ctrl.name = item.Entity.TestProgress.Name;
                        ctrl.callTotal = item.Entity.TestProgress.Calls_Total;
                        ctrl.callComplete = item.Entity.TestProgress.Calls_Complete;
                        ctrl.cliSuccess = item.Entity.TestProgress.CLI_Success;
                        ctrl.cliNoResult = item.Entity.TestProgress.CLI_No_Result;
                        ctrl.fail = item.Entity.TestProgress.CLI_Fail;
                        ctrl.pdd = item.Entity.TestProgress.PDD;
                    }
                }

                return directiveAPI;
            }
        }
    }

    return directiveDefinitionObject;

}]);
