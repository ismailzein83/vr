"use strict";

app.directive("businessprocessBpBusinessRuleSelectminimumbedAction", [function () {
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
        templateUrl: "/Client/Modules/WhS_SupplierPriceList/Directives/BusinessRuleAction/Templates/EmptyTemplate.html"
    };

    function DirectiveConstructor($scope, ctrl) {

        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {

            var api = {};
            api.getData = function () {
                return {
                    action: {
                        $type: "TOne.WhS.SupplierPriceList.Business.GetServiceWithMinimumBEDAction, TOne.WhS.SupplierPriceList.Business",
                        BPBusinessRuleActionTypeId : 5
                    }
                };
            };

            api.load = function (payload) {

            }

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }

    return directiveDefinitionObject;
}]);
