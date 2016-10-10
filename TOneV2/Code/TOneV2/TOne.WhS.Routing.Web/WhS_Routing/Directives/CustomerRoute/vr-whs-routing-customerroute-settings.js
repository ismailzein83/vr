'use strict';

app.directive('vrWhsRoutingCustomerrouteSettings', ['UtilsService', 'WhS_Routing_TimeSettingsTypeEnum',
function (UtilsService, WhS_Routing_TimeSettingsTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new settingsCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/CustomerRoute/Templates/CustomerRouteSettingsTemplate.html"
    };


    function settingsCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != undefined && payload.CustomerRoute != undefined) {
                    ctrl.customerRouteNumberOfOptions = payload.CustomerRoute.NumberOfOptions;
                    ctrl.customerRouteAddBlockedOptions = payload.CustomerRoute.AddBlockedOptions;
                }
                //if (payload != undefined && payload.ProductRoute != undefined) {
                //    ctrl.productRouteAddBlockedOptions = payload.ProductRoute.AddBlockedOptions;
                //}
            }

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.Routing.Entities.RouteBuildConfiguration, TOne.WhS.Routing.Entities",
                    CustomerRoute: {
                        $type: "TOne.WhS.Routing.Entities.CustomerRouteBuildConfiguration, TOne.WhS.Routing.Entities",
                        NumberOfOptions: ctrl.customerRouteNumberOfOptions,
                        AddBlockedOptions: ctrl.customerRouteAddBlockedOptions
                    },
                    //ProductRoute: {
                    //    $type: "TOne.WhS.Routing.Entities.ProductRouteBuildConfiguration, TOne.WhS.Routing.Entities",
                    //    AddBlockedOptions: ctrl.productRouteAddBlockedOptions
                    //}
                }
                return obj;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);