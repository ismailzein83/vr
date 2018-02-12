(function (app) {

    'use strict';

    supplierFaultTicketSettings.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function supplierFaultTicketSettings(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new SettingsCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            template: function (element, attrs) {
                return "";
            }
        };
        function SettingsCtor($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.BusinessEntity.Business.SupplierFaultTicketsSettings, TOne.WhS.BusinessEntity.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsBeSupplierfaultticketSettings', supplierFaultTicketSettings);

})(app);
