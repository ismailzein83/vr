"use strict";

app.directive("qmClitesterTestprogress", ['UtilsService', 'VRUIUtilsService', 'Qm_CliTester_TestCallAPIService',function (UtilsService, VRUIUtilsService, Qm_CliTester_TestCallAPIService) {

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
            }
        },
        templateUrl: function (element, attrs) {
            return getDirectiveTemplateUrl();
        }
    };

    function getDirectiveTemplateUrl() {
        return "/Client/Modules/QM_CliTester/Directives/MainExtensions/ITest/Templates/TestProgressTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;
        var sourceTypeDirectiveAPI;
        var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {

            $scope.sourceTypeTemplates = [];
            $scope.onSourceTypeDirectiveReady = function (api) {
                sourceTypeDirectiveAPI = api;
                sourceDirectiveReadyPromiseDeferred.resolve();
            }

            var api = {};
            api.getData = function () {
                var CLITestConnectorObj = sourceTypeDirectiveAPI.getData();
                CLITestConnectorObj.ConfigId = $scope.selectedSourceTypeTemplate.TemplateConfigID;
                return {
                    $type: "QM.CLITester.Business.TestProgressTaskActionArgument, QM.CLITester.Business",
                    CLITestConnector: CLITestConnectorObj,
                    MaximumRetryCount: $scope.maximumRetryCount
                };
            };


            api.load = function (payload) {

                var promises = [];
                if (payload != undefined && payload.data != undefined)
                    $scope.maximumRetryCount = payload.data.MaximumRetryCount;

                var testProgressTemplatesLoad = Qm_CliTester_TestCallAPIService.GetTestProgressTemplates().then(function (response) {
                    if (payload != undefined && payload.data != undefined && payload.data.CLITestConnector != undefined)
                        var sourceConfigId = payload.data.CLITestConnector.ConfigId;
                    angular.forEach(response, function (item) {
                        $scope.sourceTypeTemplates.push(item);
                    });

                    if (sourceConfigId != undefined)
                        $scope.selectedSourceTypeTemplate = UtilsService.getItemByVal($scope.sourceTypeTemplates, sourceConfigId, "TemplateConfigID");
                });
                promises.push(testProgressTemplatesLoad);

                return UtilsService.waitMultiplePromises(promises);
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
