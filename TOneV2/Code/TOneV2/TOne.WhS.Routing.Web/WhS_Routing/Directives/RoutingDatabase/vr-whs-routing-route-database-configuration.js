'use strict';

app.directive('vrWhsRoutingRouteDatabaseConfiguration', [
function () {

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
                    FutureDBToKeep: ctrl.future
                }
                return obj;
            }
            api.load = function (payload) {
                if (payload != undefined) {
                    //ctrl.specific = payload.SpecificDBToKeep;
                    ctrl.current = payload.CurrentDBToKeep,
                    ctrl.future = payload.FutureDBToKeep
                }
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
        this.initializeController = initializeController;
    }
    return directiveDefinitionObject;
}]);