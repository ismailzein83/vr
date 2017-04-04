"use strict";

app.directive("whsInvoicetypeDefinitionCustomerinvoicesettings", ["UtilsService", "VRNotificationService",
    function (UtilsService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CustomerInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/InvoiceAccountTypes/CustomerInvoice/Templates/CustomerInvoiceSettingsDefinitionTemplate.html"

        };

        function CustomerInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.extendedSettingsEntity != undefined) {
                        }
                    }
                };


                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.CustomerInvoiceSettings ,TOne.WhS.Invoice.Business",
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);