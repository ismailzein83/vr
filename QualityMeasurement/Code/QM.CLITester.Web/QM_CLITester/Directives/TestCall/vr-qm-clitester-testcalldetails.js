"use strict";

app.directive("vrQmClitesterTestcalldetails", [ "Qm_CliTester_TestCallService",
function (Qm_CliTester_TestCallService) {

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
                        
                        $scope.testcallsdetails = (item.Entity.TestProgress.CallResults) ? item.Entity.TestProgress.CallResults : [];
                    }
                    if (item.Entity.Measure != null && item.Entity.Measure != undefined) {
                        ctrl.Duration = item.Entity.Measure.Duration;
                    }

                    var arrayLength = $scope.testcallsdetails.length;
                    for (var i = 0; i < arrayLength; i++) {
                        if ($scope.testcallsdetails[i].Duration == null)
                            $scope.testcallsdetails[i].Duration = "";
                    }
                }

                return directiveAPI;
            }
        }

        $scope.getColorResult = function (dataItem) {
            return Qm_CliTester_TestCallService.getCallTestResultColor(dataItem.CallTestResult);
        };
    }

    return directiveDefinitionObject;

}]);
