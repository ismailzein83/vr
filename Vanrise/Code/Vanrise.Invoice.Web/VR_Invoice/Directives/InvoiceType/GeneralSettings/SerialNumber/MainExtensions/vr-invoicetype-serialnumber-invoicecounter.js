"use strict";

app.directive("vrInvoicetypeSerialnumberInvoicecounter", ["UtilsService", "VRNotificationService", "VRUIUtilsService","VR_Invoice_CounterTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_CounterTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new InvoiceCounterSerialNumberPart($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/GeneralSettings/SerialNumber/MainExtensions/Templates/InvoiceCounterSerialNumberPartTemplate.html"

        };

        function InvoiceCounterSerialNumberPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.counterTypes = UtilsService.getArrayEnum(VR_Invoice_CounterTypeEnum);
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
                            $scope.scopeModel.selectedCounterType = UtilsService.getItemByVal($scope.scopeModel.counterTypes, payload.concatenatedPartSettings.Type, "value");

                        }
                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts.InvoiceCounterSerialNumberPart ,Vanrise.Invoice.MainExtensions",
                        UsePartnerCount: $scope.scopeModel.usePartnerCount,
                        OverAllStartUpCounter: $scope.scopeModel.overAllStartUpCounter,
                        Type: $scope.scopeModel.selectedCounterType != undefined ? $scope.scopeModel.selectedCounterType.value : undefined
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);