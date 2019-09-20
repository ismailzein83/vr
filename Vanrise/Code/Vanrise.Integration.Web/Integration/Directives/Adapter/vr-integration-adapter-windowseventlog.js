"use strict";

app.directive("vrIntegrationAdapterWindowseventlog", ['UtilsService',
function (UtilsService) {

    var directiveDefinitionObject = {
        restrict: "E",
        scope: {
            onReady: "="
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;

            var directiveConstructor = new DirectiveConstructor($scope, ctrl);
            directiveConstructor.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        compile: function (element, attrs) {
            return {
                pre: function ($scope, iElem, iAttrs, ctrl) {

                }
            };
        },
        templateUrl: "/Client/Modules/Integration/Directives/Adapter/Templates/AdapterWindowsEventLogTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "Vanrise.Integration.Adapters.WindowsEventLogReceiveAdapter.Arguments.WindowsEventLogAdapterArgument,Vanrise.Integration.Adapters.WindowsEventLogReceiveAdapter.Arguments",
                    Domain: $scope.domain,
                    HostName: $scope.hostName,
                    UserName: $scope.userName,
                    Password: $scope.password,
                    Sources: $scope.sources,
                    BatchSize: $scope.batchSize,
                    InitialStartDate: $scope.initialStartDate
                };
                return obj;
            };
            api.getStateData = function () {
                return {
                    $type: "Vanrise.Integration.Adapters.WindowsEventLogReceiveAdapter.Arguments.WindowsEventLogAdapterState, Vanrise.Integration.Adapters.WindowsEventLogReceiveAdapter.Arguments",
                };
            };


            api.load = function (payload) {

                if (payload != undefined) {
                    var argumentData = payload.adapterArgument;
                    if (argumentData != undefined) {
                        $scope.domain = argumentData.Domain;
                        $scope.hostName = argumentData.HostName;
                        $scope.userName = argumentData.UserName;
                        $scope.password = argumentData.Password;
                        $scope.sources = argumentData.Sources;
                        $scope.batchSize = argumentData.BatchSize;
                        $scope.initialStartDate = argumentData.InitialStartDate;
                    }
                    var adapterState = payload.adapterState;
                }

            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
