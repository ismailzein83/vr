"use strict";

app.directive("vrInvoicetypeSerialnumberInvoicedate", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceDateEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceDateEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceDateSerialNumberPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSerialNumberSettings/MainExtensions/Templates/InvoiceDateSerialNumberPartTemplate.html"

        };

        function InvoiceDateSerialNumberPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.invoiceDates = UtilsService.getArrayEnum(VR_Invoice_InvoiceDateEnum);
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.concatenatedPartSettings != undefined) {
                            $scope.scopeModel.dateFormat = payload.concatenatedPartSettings.DateFormat;
                            $scope.scopeModel.selectedInvoiceDate = UtilsService.getItemByVal($scope.scopeModel.invoiceDates, payload.concatenatedPartSettings.InvoiceDate, "value");

                        }
                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts.InvoiceDateSerialNumberPart ,Vanrise.Invoice.MainExtensions",
                        DateFormat: $scope.scopeModel.dateFormat,
                        InvoiceDate: $scope.scopeModel.selectedInvoiceDate != undefined ? $scope.scopeModel.selectedInvoiceDate.value : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);