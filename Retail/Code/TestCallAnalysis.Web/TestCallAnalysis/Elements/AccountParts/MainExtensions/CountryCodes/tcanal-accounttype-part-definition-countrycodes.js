"use strict";
app.directive("tcanalAccounttypePartDefinitionCountrycodes", ["UtilsService",
    function (UtilsService) {
        var directiveDefinitionObject = {
            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CountryCodesPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/TestCallAnalysis/Elements/AccountParts/MainExtensions/CountryCodes/Templates/CountryCodesPartDefinitionTemplate.html"

        };

        function CountryCodesPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};


                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };


                api.getData = function () {

                    return {
                        $type: "TestCallAnalysis.Business.CountryCodesPartDefinition, TestCallAnalysis.Business"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
        return directiveDefinitionObject;
    }
]);