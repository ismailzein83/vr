'use strict';

app.directive('whsRoutingActionBlockcustomeraccount', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockCustomerAccountAction = new BlockCustomerAccountAction($scope, ctrl, $attrs);
            blockCustomerAccountAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: '/Client/Modules/WhS_Routing/Directives/Extensions/Actions/BlockCustomeAccount/Templates/BlockCustomerAccountAction.html'
    };

    function BlockCustomerAccountAction($scope, ctrl, $attrs) {

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
                    $type: 'TOne.WhS.Routing.MainExtensions.VRActions.BlockCustomerAccountAction,TOne.WhS.Routing.MainExtensions',
                    ActionName: "Block Customer"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);