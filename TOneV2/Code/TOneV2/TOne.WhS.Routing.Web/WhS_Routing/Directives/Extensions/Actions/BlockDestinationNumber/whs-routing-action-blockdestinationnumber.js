'use strict';

app.directive('whsRoutingActionBlockdestinationnumber', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockDestinationNumberAction = new BlockDestinationNumber($scope, ctrl, $attrs);
            blockDestinationNumberAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockDestinationNumber/Templates/BlockDestinationNumberAction.html"
    };

    function BlockDestinationNumber($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockDestinationNumberAction, TOne.WhS.Routing.MainExtensions',
                    ActionName: "Block Destination Number"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);