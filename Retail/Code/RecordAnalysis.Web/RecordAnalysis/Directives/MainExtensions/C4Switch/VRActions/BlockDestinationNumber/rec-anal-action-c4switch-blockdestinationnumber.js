'use strict';

app.directive('recAnalActionC4switchBlockdestinationnumber', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
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
        templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/C4Switch/VRActions/BlockDestinationNumber/Templates/BlockDestinationNumberAction.html"
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
                    $type: 'RecordAnalysis.MainExtensions.C4Switch.VRActions.BlockDestinationNumber.BlockDestinationNumberAction, RecordAnalysis.MainExtensions',
                    ActionName: "Block Destination Number"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);