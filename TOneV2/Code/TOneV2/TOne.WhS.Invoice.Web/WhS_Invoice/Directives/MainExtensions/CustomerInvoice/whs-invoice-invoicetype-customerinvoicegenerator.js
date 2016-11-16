﻿"use strict";

app.directive("whsInvoiceInvoicetypeCustomerinvoicegenerator", ["UtilsService", "VRNotificationService",
    function (UtilsService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CustomerInvoiceGenerator($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/MainExtensions/CustomerInvoice/Templates/CustomerInvoiceGeneratorTemplate.html"

        };

        function CustomerInvoiceGenerator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.CustomerInvoiceGenerator ,TOne.WhS.Invoice.Business"
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);