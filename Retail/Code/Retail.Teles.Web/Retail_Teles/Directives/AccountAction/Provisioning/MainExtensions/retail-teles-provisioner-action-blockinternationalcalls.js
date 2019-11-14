'use strict';

app.directive('retailTelesProvisionerActionBlockinternationalcalls', ['UtilsService', 'VRUIUtilsService', function (UtilsService, VRUIUtilsService) {
    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },

        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var blockInternationalCallsAction = new BlockInternationalCalls($scope, ctrl, $attrs);
            blockInternationalCallsAction.initializeController();
        },

        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/Retail_Teles/Directives/AccountAction/Provisioning/MainExtensions/Templates/BlockInternationalCallsActionTemplate.html"
    };

    function BlockInternationalCalls($scope, ctrl, $attrs) {

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
                    $type: 'Retail.Teles.Business.UserRGAction, Retail.Teles.Business',
                    ActionName: "Block International Calls"
                };
            };

            if (ctrl.onReady != null) {
                ctrl.onReady(api);
            }
        }
    }
}]);