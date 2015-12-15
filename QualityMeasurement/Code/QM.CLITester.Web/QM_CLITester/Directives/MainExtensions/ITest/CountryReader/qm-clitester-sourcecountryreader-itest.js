"use strict";

app.directive("qmClitesterSourcecountryreaderItest", [function () {
    
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
       
        return "/Client/Modules/QM_CLITester/Directives/MainExtensions/ITest/CountryReader/Templates/SourceCountryReaderItest.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;


        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                
                return {
                    $type: "QM.CLITester.iTestIntegration.CountryITestReader, QM.CLITester.iTestIntegration"
                };
            };


            api.load = function (payload) {
                if (payload != undefined) {
                    
                }
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
