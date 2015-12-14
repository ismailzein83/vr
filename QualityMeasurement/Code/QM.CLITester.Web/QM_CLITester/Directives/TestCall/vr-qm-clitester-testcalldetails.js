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
                        ctrl.callTotal = item.Entity.TestProgress.TotalCalls;
                        ctrl.callComplete = item.Entity.TestProgress.CompletedCalls;
                        ctrl.cliSuccess = item.Entity.TestProgress.CliSuccess;
                        ctrl.cliNoResult = item.Entity.TestProgress.CliNoResult;
                        ctrl.fail = item.Entity.TestProgress.CliFail;
                        ctrl.pdd = item.Entity.TestProgress.Pdd;
                    }
                }

                return directiveAPI;
            }
        }
    }

    return directiveDefinitionObject;

}]);
