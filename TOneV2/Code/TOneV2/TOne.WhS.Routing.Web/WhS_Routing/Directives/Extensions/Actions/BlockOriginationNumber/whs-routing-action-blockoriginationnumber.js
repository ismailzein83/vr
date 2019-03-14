'use strict';

app.directive('whsRoutingActionBlockoriginationnumber', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockOriginationNumberAction = new BlockOriginationNumber($scope, ctrl, $attrs);
            blockOriginationNumberAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockOriginationNumber/Templates/BlockOriginationNumberAction.html"
    };

    function BlockOriginationNumber($scope, ctrl, $attrs) {

        this.initializeController = initializeController;

        function initializeController() {
            defineAPI();
        }

        function defineAPI() {

            var api = {};


            api.load = function (payload) {

                var promises = [];

                return UtilsService.waitMultiplePromises(promises);
            };

            api.getData = function () {
                return {
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockOriginationNumberAction, TOne.WhS.Routing.MainExtensions',
                    ActionName: "Block Origination Number"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);