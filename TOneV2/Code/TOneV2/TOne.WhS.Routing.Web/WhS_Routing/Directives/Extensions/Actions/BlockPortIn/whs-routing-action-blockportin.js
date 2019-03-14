'use strict';

app.directive('whsRoutingActionBlockportin', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockPortInAction = new BlockPortIn($scope, ctrl, $attrs);
            blockPortInAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockPortIn/Templates/BlockPortInAction.html"
    };

    function BlockPortIn($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockPortInAction, TOne.WhS.Routing.MainExtensions',
                    ActionName: "Block Port In"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);