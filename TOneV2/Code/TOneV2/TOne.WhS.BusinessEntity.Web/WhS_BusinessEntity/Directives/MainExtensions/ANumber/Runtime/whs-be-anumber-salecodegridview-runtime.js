(function (app) {

    'use strict';

    ANumberSaleCodeGridRuntimeViewDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function ANumberSaleCodeGridRuntimeViewDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ANumberSaleCodeGridRuntimeViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/ANumber/Runtime/Templates/ANumberSaleCodeGridRuntimeViewTemplate.html'
        };

        function ANumberSaleCodeGridRuntimeViewCtor($scope, ctrl) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };


                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.ANumberSaleCodeRuntimeView, TOne.WhS.BusinessEntity.MainExtensions"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsBeAnumberSalecodegridviewRuntime', ANumberSaleCodeGridRuntimeViewDirective);

})(app);