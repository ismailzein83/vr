'use strict';

app.directive('whsRoutingActionBlockportout', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockPortOutAction = new BlockPortOut($scope, ctrl, $attrs);
            blockPortOutAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockPortOut/Templates/BlockPortOutAction.html"
    };

    function BlockPortOut($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockPortOutAction, TOne.WhS.Routing.MainExtensions',
                    ActionName: "Block Port Out"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);