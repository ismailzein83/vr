"use strict";

app.directive("qmClitesterSourceprofilereaderItest", [function () {
   
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
        return "/Client/Modules/QM_CLITester/Directives/MainExtensions/SourceProfileReader/Templates/SouceProfileReaderiTest.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        console.log('qmClitesterSourceprofilereaderITest.DirectiveConstructor')
        this.initializeController = initializeController;

        $scope.dummy = undefined;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                
                return {
                    $type: "QM.CLITester.iTestIntegration.SourceProfilesReaders.ProfileiTestReader, QM.CLITester.iTestIntegration",
                    Dummy: $scope.dummy
                };
            };


            api.load = function (payload) {
                if (payload != undefined) {
                    $scope.dummy = payload.Dummy;
                }
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
