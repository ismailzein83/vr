"use strict";

app.directive("vrQmClitesterTestcalldetails", [
function () {

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

        var gridAPI;

        this.initializeController = initializeController;

        function initializeController() {
            var directiveAPI = {};
            $scope.testcallsdetails = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;
            };

            if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                ctrl.onReady(getDirectiveAPI());

            function getDirectiveAPI() {

                directiveAPI.load = function (item) {

                    if (item.Entity.InitiateTestInformation != null && item.Entity.InitiateTestInformation != undefined) {
                        ctrl.testId = item.Entity.InitiateTestInformation.Test_ID;
                    }
                    
                    if (item.Entity.TestProgress != null && item.Entity.TestProgress != undefined) {
                        
                        ctrl.name = item.Entity.TestProgress.Name;
                        
                        $scope.testcallsdetails = (item.Entity.TestProgress.CallResults)?item.Entity.TestProgress.CallResults:[];
                        // $scope.testcallsdetails =  item.Entity.TestProgress.l                       
                        //ctrl.callTotal = item.Entity.TestProgress.TotalCalls;
                        //ctrl.callComplete = item.Entity.TestProgress.CompletedCalls;
                        //ctrl.cliSuccess = item.Entity.TestProgress.CliSuccess;
                        //ctrl.cliNoResult = item.Entity.TestProgress.CliNoResult;
                        //ctrl.fail = item.Entity.TestProgress.CliFail;
                        //ctrl.pdd = item.Entity.Measure.Pdd;

                        ctrl.ReceivedCli = item.Entity.TestProgress.ReceivedCli;
                        ctrl.ReleaseCode = item.Entity.TestProgress.ReleaseCode;
                        ctrl.Source = item.Entity.TestProgress.Source;
                        ctrl.Destination = item.Entity.TestProgress.Destination;
                    }
                    if (item.Entity.Measure != null && item.Entity.Measure != undefined) {

                        //ctrl.ReceivedCli = item.Entity.Measure.ReceivedCli;
                        //ctrl.ReleaseCode = item.Entity.Measure.ReleaseCode;
                        ctrl.Duration = item.Entity.Measure.Duration;
                    }
                }

                return directiveAPI;
            }
        }
    }

    return directiveDefinitionObject;

}]);
