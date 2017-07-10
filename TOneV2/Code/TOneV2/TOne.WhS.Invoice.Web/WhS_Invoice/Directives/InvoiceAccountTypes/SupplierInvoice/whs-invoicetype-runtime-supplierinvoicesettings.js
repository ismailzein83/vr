﻿"use strict";

app.directive("whsInvoicetypeRuntimeSupplierinvoicesettings", ["UtilsService", "VRNotificationService",
    function (UtilsService, VRNotificationService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new SupplierInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/InvoiceAccountTypes/SupplierInvoice/Templates/SupplierInvoiceSettingsRuntimeTemplate.html"

        };

        function SupplierInvoiceSettings($scope, ctrl, $attrs) {
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
                        $type: "TOne.WhS.Invoice.Business.Extensions.SupplierInvoiceAccountSettings ,TOne.WhS.Invoice.Business",
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);