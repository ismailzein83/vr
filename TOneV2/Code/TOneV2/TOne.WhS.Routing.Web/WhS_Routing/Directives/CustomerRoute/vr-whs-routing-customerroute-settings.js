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
                if (payload != undefined) {
                    if (payload.CustomerRoute != undefined) {
                        ctrl.customerRouteNumberOfOptions = payload.CustomerRoute.NumberOfOptions;
                        ctrl.customerRouteIndexCommandTimeout = payload.CustomerRoute.IndexesCommandTimeoutInMinutes;
                        ctrl.customerRouteMaxDOP = payload.CustomerRoute.MaxDOP;
                        ctrl.customerRouteKeepBackupsForRemovedOptions = payload.CustomerRoute.KeepBackUpsForRemovedOptions;
                    }

                    if (payload.ProductRoute != undefined) {
                        ctrl.productRouteIndexCommandTimeout = payload.ProductRoute.IndexesCommandTimeoutInMinutes;
                        ctrl.productRouteMaxDOP = payload.ProductRoute.MaxDOP;
                        ctrl.productRouteKeepBackupsForRemovedOptions = payload.ProductRoute.KeepBackUpsForRemovedOptions;
                    }

                    if (payload.IncludedRules != undefined) {
                        ctrl.includeRateTypeRules = payload.IncludedRules.IncludeRateTypeRules;
                        ctrl.includeExtraChargeRules = payload.IncludedRules.IncludeExtraChargeRules;
                        ctrl.includeTariffRules = payload.IncludedRules.IncludeTariffRules;
                    }
                }
            };

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.Routing.Entities.RouteBuildConfiguration, TOne.WhS.Routing.Entities",
                    CustomerRoute: {
                        $type: "TOne.WhS.Routing.Entities.CustomerRouteBuildConfiguration, TOne.WhS.Routing.Entities",
                        NumberOfOptions: ctrl.customerRouteNumberOfOptions,
                        KeepBackUpsForRemovedOptions: ctrl.customerRouteKeepBackupsForRemovedOptions,
                        IndexesCommandTimeoutInMinutes: ctrl.customerRouteIndexCommandTimeout,
                        MaxDOP: ctrl.customerRouteMaxDOP
                    },
                    ProductRoute: {
                        $type: "TOne.WhS.Routing.Entities.ProductRouteBuildConfiguration, TOne.WhS.Routing.Entities",
                        KeepBackUpsForRemovedOptions: ctrl.productRouteKeepBackupsForRemovedOptions,
                        IndexesCommandTimeoutInMinutes: ctrl.productRouteIndexCommandTimeout,
                        MaxDOP: ctrl.productRouteMaxDOP
                    },
                    IncludedRules: {
                        $type: "TOne.WhS.Routing.Entities.IncludedRulesConfiguration, TOne.WhS.Routing.Entities",
                        IncludeRateTypeRules: ctrl.includeRateTypeRules,
                        IncludeExtraChargeRules: ctrl.includeExtraChargeRules,
                        IncludeTariffRules: ctrl.includeTariffRules
                    }
                };
                return obj;
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);