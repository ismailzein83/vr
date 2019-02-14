'use strict';

app.directive('recAnalActionBlockoriginationnumber', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
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
        templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/VRActions/BlockOriginationNumber/Templates/BlockOriginationNumberAction.html"
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
                    $type: 'RecordAnalysis.MainExtensions.VRActions.BlockOriginationNumber.BlockOriginationNumberAction, RecordAnalysis.MainExtensions',
                    ActionName: "Block Origination Number"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);