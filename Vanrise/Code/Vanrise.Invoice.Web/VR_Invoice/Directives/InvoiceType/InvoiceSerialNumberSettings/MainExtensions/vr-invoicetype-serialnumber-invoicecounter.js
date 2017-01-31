"use strict";

app.directive("vrInvoicetypeSerialnumberInvoicesequence", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_DateCounterTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_Invoice_DateCounterTypeEnum) {

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
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSerialNumberSettings/MainExtensions/Templates/InvoiceCounterSerialNumberPartTemplate.html"

        };

        function InvoiceCounterSerialNumberPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.initialSequenceValue = 1;
                $scope.scopeModel.dateCounterTypes = UtilsService.getArrayEnum(VR_Invoice_DateCounterTypeEnum);
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        context = payload.context;
                        if (payload.concatenatedPartSettings != undefined) {
                            $scope.scopeModel.includePartnerId = payload.concatenatedPartSettings.IncludePartnerId;
                            $scope.scopeModel.selectedDateCounterType = UtilsService.getItemByVal($scope.scopeModel.dateCounterTypes, payload.concatenatedPartSettings.DateCounterType, "value");
                            $scope.scopeModel.paddingLeft = payload.concatenatedPartSettings.PaddingLeft;
                        }
                        var promises = [];
                        return UtilsService.waitMultiplePromises(promises);
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts.InvoiceSequenceSerialNumberPart ,Vanrise.Invoice.MainExtensions",
                        IncludePartnerId: $scope.scopeModel.includePartnerId,
                        DateCounterType: $scope.scopeModel.selectedDateCounterType != undefined ? $scope.scopeModel.selectedDateCounterType.value : undefined,
                        PaddingLeft: $scope.scopeModel.paddingLeft
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);