'use strict';

app.directive('whsRoutingActionBlocksupplieraccount', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockSupplierAccountAction = new BlockSupplierAccountAction($scope, ctrl, $attrs);
            blockSupplierAccountAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockSupplierAccount/Templates/BlockSupplierAccountAction.html'
    };

    function BlockSupplierAccountAction($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockSupplierAccountAction,TOne.WhS.Routing.MainExtensions',
                    ActionName: "Block Supplier"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);