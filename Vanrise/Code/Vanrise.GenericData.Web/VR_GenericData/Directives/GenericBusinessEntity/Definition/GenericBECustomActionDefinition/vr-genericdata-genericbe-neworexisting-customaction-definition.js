(function (app) {

    'use strict';

    NewOrExistingGenericBEDefinitionCustomActionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function NewOrExistingGenericBEDefinitionCustomActionDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new AddCustomActionCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBECustomActionDefinition/Templates/NewOrExistingGenericBEDefinitionCustomActionTemplate.html'
        };

        function AddCustomActionCtor($scope, ctrl) {

            this.initializeController = initializeController;

            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }


            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBECustomAction.GenericBENewOrExistingCustomAction, Vanrise.GenericData.MainExtensions"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrGenericdataGenericbeNeworexistingCustomactionDefinition', NewOrExistingGenericBEDefinitionCustomActionDirective);

})(app);