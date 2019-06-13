(function (app) {

    'use strict';

    CreateSwapDealActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function CreateSwapDealActionDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CreateSwapDealActionConstructor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_Deal/Directives/SwapDealAnalysis/GenericBEActionDefinition/Templates/CreateSwapDealDefinitionActionTemplate.html'
        };

        function CreateSwapDealActionConstructor($scope, ctrl) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var context = payload.context;
                    if (context != undefined && context.showSecurityGridCallBack != undefined && typeof (context.showSecurityGridCallBack) == 'function')
                        context.showSecurityGridCallBack(true);
                };


                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Deal.MainExtensions.CreateSwapDealGenericBEAction, TOne.WhS.Deal.MainExtensions"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsDealCreateswapdealactionDefinition', CreateSwapDealActionDirective);

})(app);