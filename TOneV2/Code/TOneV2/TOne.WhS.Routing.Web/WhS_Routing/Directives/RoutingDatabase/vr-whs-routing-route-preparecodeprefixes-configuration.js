'use strict';

app.directive('vrWhsRoutingRoutePreparecodeprefixesConfiguration', ['UtilsService', 'WhS_Routing_TimeSettingsTypeEnum',
function (UtilsService, WhS_Routing_TimeSettingsTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new prepareCodePrefixesConfigurationCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/RoutingDatabase/Templates/PrepareCodePrefixesTemplate.html"
    };


    function prepareCodePrefixesConfigurationCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != undefined) {
                    ctrl.maxPrefixLength = payload.MaxPrefixLength;
                    ctrl.threshold = payload.Threshold;
                }
            }

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.Routing.Entities.PrepareCodePrefixes, TOne.WhS.Routing.Entities",
                    MaxPrefixLength: ctrl.maxPrefixLength,
                    Threshold: ctrl.threshold,
                }
                return obj;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);