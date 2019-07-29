'use strict';
app.directive('vrWhsSalesSellingrulesettingsThresholdMinmarginpercentage', ['$compile',
    function ($compile) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;
                var ctor = new sellingRulePercentageConstructor(ctrl, $scope, $attrs);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/SellingRuleSettings/Percentage/Templates/MinMarginPercentageThresholdSellingRule.html"

        };


        function sellingRulePercentageConstructor(ctrl, $scope, $attrs) {
            function initializeController() {
                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.getData = function () {
                    var obj = {
                        $type: "TOne.WhS.Sales.MainExtensions.MinMarginPercentageRateThreshold,TOne.WhS.Sales.MainExtensions",
                        Percentage: ctrl.percentage
                    };
                    return obj;
                };

                api.load = function (payload) {
                    if (payload != undefined && payload.threshold) {
                        ctrl.percentage = payload.threshold.Percentage;
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }]);