'use strict';

app.directive('recAnalActionBlockinboundtrunk', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockInboundTrunkAction = new BlockInboundTrunk($scope, ctrl, $attrs);
            blockInboundTrunkAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/VRActions/BlockInBoundTrunk/Templates/BlockInboundTrunkAction.html"
    };

    function BlockInboundTrunk($scope, ctrl, $attrs) {

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
                    $type: 'RecordAnalysis.MainExtensions.VRActions.BlockInBoundTrunk.BlockInBoundTrunkAction, RecordAnalysis.MainExtensions',
                    ActionName: "Block Inbound Trunk"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);