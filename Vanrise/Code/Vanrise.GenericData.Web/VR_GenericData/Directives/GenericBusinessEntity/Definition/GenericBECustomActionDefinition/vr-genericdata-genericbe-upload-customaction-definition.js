(function (app) {

    'use strict';

    UploadGenericBEDefinitionCustomActionDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService'];

    function UploadGenericBEDefinitionCustomActionDirective(UtilsService, VRUIUtilsService, VRNotificationService) {
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
            templateUrl: '/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Definition/GenericBECustomActionDefinition/Templates/UploadGenericBEDefinitionCustomActionTemplate.html'
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
                    var promises = [];

                    return UtilsService.waitPromiseNode(promises);
                };

                api.getData = function () {
                   
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

        }
    }

    app.directive('vrGenericdataGenericbeUploadCustomactionDefinition', UploadGenericBEDefinitionCustomActionDirective);

})(app);