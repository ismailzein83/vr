'use strict';

app.directive('vrWhsRoutingRouteDatabaseConfiguration', ['UtilsService', 'WhS_Routing_TimeSettingsTypeEnum',
function (UtilsService, WhS_Routing_TimeSettingsTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new routeDatabaseConfigurationCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/RoutingDatabase/Templates/RouteDatabaseConfigurationTemplate.html"
    };


    function routeDatabaseConfigurationCtor(ctrl, $scope, $attrs) {

        function initializeController() {

            defineAPI();
        }

        function defineAPI() {
            var api = {};

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.Routing.Entities.RouteDatabaseConfiguration, TOne.WhS.Routing.Entities",
                    //SpecificDBToKeep: ctrl.specific,
                    CurrentDBToKeep: ctrl.current,
                    FutureDBToKeep: ctrl.future,
                    MaximumEstimatedExecutionTime: ctrl.time,
                    TimeUnit: ctrl.selectedTimeUnit.value
                };
                return obj;
            };
            api.load = function (payload) {
                $scope.timeUnits = UtilsService.getArrayEnum(WhS_Routing_TimeSettingsTypeEnum);

                if (payload != undefined) {
                    //ctrl.specific = payload.SpecificDBToKeep;
                    ctrl.current = payload.CurrentDBToKeep;
                    ctrl.future = payload.FutureDBToKeep;
                    ctrl.time = payload.MaximumEstimatedExecutionTime;
                    ctrl.selectedTimeUnit = UtilsService.getEnum(WhS_Routing_TimeSettingsTypeEnum, "value", payload.TimeUnit);
                }
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);