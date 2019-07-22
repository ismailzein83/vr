(function (app) {

    'use strict';

    customerFaultTicketOnBeforeSaveHandler.$inject = ["UtilsService", 'VRUIUtilsService', 'VRNotificationService'];

    function customerFaultTicketOnBeforeSaveHandler(UtilsService, VRUIUtilsService, VRNotificationService) {
        return {
            restrict: "E",
            scope: {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new CustomerFaultTicketCtor($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "Ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_BusinessEntity/Directives/CaseManagement/CustomerCase/Templates/CustomerFaultTicketOnBeforeSaveHandlerTemplate.html"

        };
        function CustomerFaultTicketCtor($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.BusinessEntity.Business.CustomerFaultTicketBeforeInsertHandler, TOne.WhS.BusinessEntity.Business"
                    };
                    return data;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('whsBeCustomerfaultticketOnbeforesavehandler', customerFaultTicketOnBeforeSaveHandler);

})(app);
