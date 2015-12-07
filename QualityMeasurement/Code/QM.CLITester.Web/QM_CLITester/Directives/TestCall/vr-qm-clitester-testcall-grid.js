"use strict";

app.directive("vrQmClitesterTestcallGrid", ["UtilsService", "VRNotificationService", "Qm_CliTester_TestCallAPIService",
function (UtilsService, VRNotificationService, Qm_CliTester_TestCallAPIService) {

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
        this.initializeController = initializeController;

        function initializeController() {
            $scope.testcalls = [];

            $scope.onGridReady = function (api) {
                gridAPI = api;

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == "function")
                    ctrl.onReady(getDirectiveAPI());
                function getDirectiveAPI() {

                    var directiveAPI = {};
                    directiveAPI.loadGrid = function (query) {
                        //setInterval(function () { return gridAPI.retrieveData(query); }, 2000);
                    }
                    return directiveAPI;
                }
            };
            //defineMenuActions();
        }

        $scope.dataRetrievalFunction = function (dataRetrievalInput, onResponseReady) {
            return Qm_CliTester_TestCallAPIService.GetFilteredTestCalls(dataRetrievalInput)
                .then(function (response) {

                    onResponseReady(response);
                })
                .catch(function (error) {
                    VRNotificationService.notifyException(error, $scope);
                });
        };
    }



    return directiveDefinitionObject;

}]);