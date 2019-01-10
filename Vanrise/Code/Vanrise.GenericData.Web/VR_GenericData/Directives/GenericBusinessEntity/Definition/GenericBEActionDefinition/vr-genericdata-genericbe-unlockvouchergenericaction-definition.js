(function (app) {

    'use strict';

    UnlockVoucherGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function UnlockVoucherGenericBEDefinitionActionDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new UnlockVoucherGenericBEDefinitionActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/UnlockVoucherGenericBEDefinitionActionTemplate.html'
        };

        function UnlockVoucherGenericBEDefinitionActionCtor($scope, ctrl) {
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
                        $type: "Vanrise.Voucher.Business.UnlockVoucherAction, Vanrise.Voucher.Business"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeUnlockvouchergenericactionDefinition', UnlockVoucherGenericBEDefinitionActionDirective);

})(app);