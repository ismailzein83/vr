(function (app) {

    'use strict';

    ANumberSupplierCodeGridDefinitionViewDirective.$inject = ['UtilsService', 'VRNotificationService'];

    function ANumberSupplierCodeGridDefinitionViewDirective(UtilsService, VRNotificationService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new ANumberSupplierCodeGridDefinitionViewCtor($scope, ctrl);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/ANumber/Definition/Templates/ANumberSupplierCodeGridDefinitionViewTemplate.html'
        };

        function ANumberSupplierCodeGridDefinitionViewCtor($scope, ctrl) {
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
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.ANumberSupplierCodeDefinitionView, TOne.WhS.BusinessEntity.MainExtensions"
                    };
                };
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsBeAnumberSuppliercodegridviewDefinition', ANumberSupplierCodeGridDefinitionViewDirective);

})(app);