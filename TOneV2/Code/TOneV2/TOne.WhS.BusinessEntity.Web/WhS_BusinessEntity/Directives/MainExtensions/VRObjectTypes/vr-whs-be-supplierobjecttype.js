(function (app) {

    'use strict';

    SupplierObjectType.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function SupplierObjectType(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var supplierObjectType = new SupplierDierctiveObjectType($scope, ctrl, $attrs);
                supplierObjectType.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: '/Client/Modules/WhS_BusinessEntity/Directives/MainExtensions/VRObjectTypes/Templates/SupplierObjectTypeTemplate.html'


        };
        function SupplierDierctiveObjectType($scope, ctrl, $attrs) {
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
                    var data = {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.SupplierObjectType, TOne.WhS.BusinessEntity.MainExtensions"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('vrWhsBeSupplierobjecttype', SupplierObjectType);

})(app);




