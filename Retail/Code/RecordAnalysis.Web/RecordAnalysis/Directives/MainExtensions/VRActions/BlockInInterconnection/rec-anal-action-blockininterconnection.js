'use strict';

app.directive('recAnalActionBlockininterconnection', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockInInterconnectionAction = new BlockInInterconnection($scope, ctrl, $attrs);
            blockInInterconnectionAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/VRActions/BlockInInterconnection/Templates/BlockInInterconnectionAction.html"
    };

    function BlockInInterconnection($scope, ctrl, $attrs) {

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
                    $type: 'RecordAnalysis.MainExtensions.VRActions.BlockInInterconnection.BlockInInterconnectionAction, RecordAnalysis.MainExtensions',
                    ActionName: "Block In Interconnection"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);