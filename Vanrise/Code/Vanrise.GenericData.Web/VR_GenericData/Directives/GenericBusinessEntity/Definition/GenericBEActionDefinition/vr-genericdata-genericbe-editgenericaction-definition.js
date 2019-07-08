(function (app) {

    'use strict';

    EditGenericBEDefinitionActionDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function EditGenericBEDefinitionActionDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new EditGenericBEDefinitionActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBEActionDefinition/Templates/EditGenericBEDefinitionActionTemplate.html'
        };

        function EditGenericBEDefinitionActionCtor($scope, ctrl) {
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
                    var settings = payload.settings;
                    if (settings != undefined)
                        $scope.scopeModel.onlyView = settings.OnlyView;
                };


                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.EditGenericBEAction, Vanrise.GenericData.MainExtensions",
                        OnlyView: $scope.scopeModel.onlyView
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeEditgenericactionDefinition', EditGenericBEDefinitionActionDirective);

})(app);