'use strict';
app.directive('vrWhsRoutingPolicyRateLowest', ['$compile', 'UtilsService',
function ($compile, UtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var ctor = new routingPolicyRateLowestCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/SupplierZoneToRPOptionPolicies/Templates/SupplierZoneToRPOptionLowestRatePolicy.html"
    };

    function routingPolicyRateLowestCtor(ctrl, $scope, $attrs) {
        function initializeController() {
            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.Routing.Business.SupplierZoneToRPOptionLowestRatePolicy, TOne.WhS.Routing.Business"
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