'use strict';
app.directive('vrWhsSalesSellingrulesettingsFixed', ['UtilsService',
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
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/WhS_Sales/Directives/Extensions/SellingRuleSettings/Fixed/Templates/FixedRuleDirective.html';
            }

        };

        function marginCtor(ctrl, $scope) {

            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload.SellingRuleSettings != undefined) {
                        ctrl.fromRate = payload.SellingRuleSettings.FromRate;
                        ctrl.toRate = payload.SellingRuleSettings.ToRate;
                    }
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Sales.Business.SellingRules.FixedRule, TOne.WhS.Sales.Business",
                        FromRate: ctrl.fromRate,
                        ToRate: ctrl.toRate,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);