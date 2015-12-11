"use strict";

app.directive("vrQmClitesterSuppliersettings", [function () {

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
        return "/Client/Modules/QM_CLITester/Directives/Supplier/Templates/AddPrefixDirectiveTemplate.html";
    }

    function DirectiveConstructor($scope, ctrl) {
        this.initializeController = initializeController;

        $scope.numberPrefix = undefined;
        
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            var currentSettings;

            api.getData = function () {
                var obj;
                if (currentSettings != null)
                    obj = currentSettings;
                else
                    obj = {
                        $type: "QM.CLITester.iTestIntegration.SupplierExtensionSettings, QM.CLITester.iTestIntegration"
                    };
                obj.Prefix = $scope.numberPrefix;
                return obj;
            };

            api.load = function (payload) {
                if (payload != undefined) {
                    currentSettings = payload;
                    $scope.numberPrefix = payload.Prefix;
                }
            }


            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);
