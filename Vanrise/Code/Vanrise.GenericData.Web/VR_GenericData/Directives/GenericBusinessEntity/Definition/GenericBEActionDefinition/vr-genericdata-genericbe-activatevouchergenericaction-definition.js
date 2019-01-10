(function (app) {

    'use strict';

    ActivateVoucherGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function ActivateVoucherGenericBEDefinitionActionDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ActivateVoucherGenericBEDefinitionActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/ActivateVoucherGenericBEDefinitionActionTemplate.html'
        };

        function ActivateVoucherGenericBEDefinitionActionCtor($scope, ctrl) {
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
                        $type: "Vanrise.Voucher.Business.ActivateVouchersAction, Vanrise.Voucher.Business"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeActivatevouchergenericactionDefinition', ActivateVoucherGenericBEDefinitionActionDirective);

})(app);