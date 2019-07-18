"use strict";

app.directive("vrInvoicetypeSerialnumberCompanysetting", ["UtilsService", "VRNotificationService", "VRUIUtilsService", "VR_Invoice_DateCounterTypeEnum",
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
            templateUrl: "/Client/Modules/VR_Invoice/Directives/InvoiceType/InvoiceSerialNumberSettings/MainExtensions/Templates/InvoiceCompanySettingSerialNumberPartTemplate.html"

        };

        function InvoiceCounterSerialNumberPart($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined && payload.concatenatedPartSettings != undefined) {
                        $scope.scopeModel.infoType = payload.concatenatedPartSettings.InfoType;
                        var promises = [];
                        return UtilsService.waitPromiseNode({ promises: promises });
                    }
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.VRConcatenatedPart.SerialNumberParts.CompanySettingSerialNumberPart ,Vanrise.Invoice.MainExtensions",
                        InfoType: $scope.scopeModel.infoType,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);