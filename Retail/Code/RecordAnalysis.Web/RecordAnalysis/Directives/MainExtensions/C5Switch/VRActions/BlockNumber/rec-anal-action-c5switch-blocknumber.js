'use strict';

app.directive('recAnalActionC5switchBlocknumber', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockNumberAction = new BlockNumber($scope, ctrl, $attrs);
            blockNumberAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/RecordAnalysis/Directives/MainExtensions/C5Switch/VRActions/BlockNumber/Templates/BlockNumberAction.html"
    };

    function BlockNumber($scope, ctrl, $attrs) {

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
                    $type: 'RecordAnalysis.MainExtensions.C5Switch.VRActions.BlockNumber.BlockNumberAction, RecordAnalysis.MainExtensions',
                    ActionName: "Block Number"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);