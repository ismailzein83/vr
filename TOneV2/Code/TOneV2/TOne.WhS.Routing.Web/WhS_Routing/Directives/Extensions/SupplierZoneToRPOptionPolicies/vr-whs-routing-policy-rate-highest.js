'use strict';
app.directive('vrWhsRoutingPolicyRateHighest', ['$compile', 'UtilsService',
function ($compile, UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new routingPolicyRateHighestCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/SupplierZoneToRPOptionPolicies/Templates/SupplierZoneToRPOptionHighestRatePolicy.html"
    };

    function routingPolicyRateHighestCtor(ctrl, $scope, $attrs) {
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.Routing.Business.SupplierZoneToRPOptionHighestRatePolicy, TOne.WhS.Routing.Business"
                };
                return obj;
            };

            api.load = function (payload) {

            };
            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        this.initializeController = initializeController;
    }

    return directiveDefinitionObject;
}]);