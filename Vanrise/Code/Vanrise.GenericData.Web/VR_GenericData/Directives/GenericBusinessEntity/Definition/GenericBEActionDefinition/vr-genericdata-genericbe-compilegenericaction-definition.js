(function (app) {

    'use strict';

    CompileGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function CompileGenericBEDefinitionActionDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CompileGenericBEDefinitionActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/CompileGenericBEDefinitionActionTemplate.html'
        };

        function CompileGenericBEDefinitionActionCtor($scope, ctrl) {


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
                        context.showSecurityGridCallBack(false);

                    return UtilsService.waitPromiseNode({ promises: [] });
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.CompileGenericBEAction, Vanrise.GenericData.MainExtensions",
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrGenericdataGenericbeCompilegenericactionDefinition', CompileGenericBEDefinitionActionDirective);

})(app);