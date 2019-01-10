(function (app) {

    'use strict';

    DeleteGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function DeleteGenericBEDefinitionActionDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new DeleteGenericBEDefinitionActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/DeleteGenericBEDefinitionActionTemplate.html'
        };

        function DeleteGenericBEDefinitionActionCtor($scope, ctrl) {
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
                        $type: "Vanrise.GenericData.MainExtensions.DeleteGenericBEAction, Vanrise.GenericData.MainExtensions"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeDeletegenericactionDefinition', DeleteGenericBEDefinitionActionDirective);

})(app);