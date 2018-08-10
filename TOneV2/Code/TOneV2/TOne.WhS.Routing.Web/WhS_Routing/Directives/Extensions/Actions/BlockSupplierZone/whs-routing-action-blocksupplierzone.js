'use strict';

app.directive('whsRoutingActionBlocksupplierzone', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockSupplierZoneAction = new BlockSupplierZoneAction($scope, ctrl, $attrs);
            blockSupplierZoneAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockSupplierZone/Templates/BlockSupplierZoneAction.html'
    };

    function BlockSupplierZoneAction($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockSupplierZoneAction,TOne.WhS.Routing.MainExtensions',
                    ActionName: "Block Supplier Zone"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);