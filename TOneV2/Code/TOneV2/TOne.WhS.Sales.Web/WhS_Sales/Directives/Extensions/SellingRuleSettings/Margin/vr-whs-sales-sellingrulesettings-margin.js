'use strict';
app.directive('vrWhsSalesSellingrulesettingsMargin', ['UtilsService',
    function (UtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new marginCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Sales/Directives/Extensions/SellingRuleSettings/Margin/Templates/MarginRuleDirective.html';
            }

        };

        function marginCtor(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                        ctrl.margin = payload.margin;
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Sales.Business.SellingRules.MarginRule, TOne.WhS.Sales.Business",
                        Margin: ctrl.margin
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);