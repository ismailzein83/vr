'use strict';

app.directive('vrWhsRoutingRouteSubprocesssettings', ['UtilsService', 'WhS_Routing_TimeSettingsTypeEnum',
function (UtilsService, WhS_Routing_TimeSettingsTypeEnum) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {

            var ctrl = this;
            var ctor = new subProcessSettingsCtor(ctrl, $scope, $attrs);
            ctor.initializeController();
        },
        controllerAs: 'ctrl',
        bindToController: true,
        compile: function (element, attrs) {

        },
        templateUrl: "/Client/Modules/WhS_Routing/Directives/RoutingDatabase/Templates/SubProcessSettingsTemplate.html"
    };


    function subProcessSettingsCtor(ctrl, $scope, $attrs) {
        this.initializeController = initializeController;

        function initializeController() {

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                if (payload != undefined) {
                    ctrl.codeRangeCountThreshold = payload.CodeRangeCountThreshold;
                    ctrl.maximumCodePrefixLength = payload.MaxCodePrefixLength;
                }
            }

            api.getData = function () {
                var obj = {
                    $type: "TOne.WhS.Routing.Entities.SubProcessSettings, TOne.WhS.Routing.Entities",
                    CodeRangeCountThreshold: ctrl.codeRangeCountThreshold,
                    MaxCodePrefixLength: ctrl.maximumCodePrefixLength,
                }
                return obj;
            }

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }

    return directiveDefinitionObject;
}]);