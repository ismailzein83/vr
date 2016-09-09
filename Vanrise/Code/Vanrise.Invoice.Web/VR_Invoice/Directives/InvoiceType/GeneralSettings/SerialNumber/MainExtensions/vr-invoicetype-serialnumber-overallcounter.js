"use strict";

app.directive("vrInvoicetypeSerialnumberOverallcounter", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_InvoiceFieldEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_InvoiceFieldEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new OverallInvoiceCounterSerialNumberPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/GeneralSettings/SerialNumber/MainExtensions/Templates/OverallInvoiceCounterSerialNumberPartTemplate.html"

        };

        function OverallInvoiceCounterSerialNumberPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.concatenatedPartSettings != undefined) {
                            $scope.scopeModel.usePartnerCount = payload.concatenatedPartSettings.UsePartnerCount;
                            $scope.scopeModel.overAllStartUpCounter = payload.concatenatedPartSettings.OverAllStartUpCounter;
                        }
                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts.OverallInvoiceCounterSerialNumberPart ,Vanrise.Invoice.MainExtensions",
                        UsePartnerCount: $scope.scopeModel.usePartnerCount,
                        OverAllStartUpCounter: $scope.scopeModel.overAllStartUpCounter
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);