"use strict";

app.directive("whsInvoiceInvoicetypeCarrierinvoicesettings", ["UtilsService", "VRNotificationService","WhS_Invoice_InvoiceTypeEnum",
    function (UtilsService, VRNotificationService, WhS_Invoice_InvoiceTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new CarrierInvoiceSettings($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/MainExtensions/CarrierInvoiceSettings/Templates/CarrierInvoiceSettingsTemplate.html"

        };

        function CarrierInvoiceSettings($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            $scope.scopeModel = {};
            function initializeController() {
                $scope.scopeModel.invoiceTypes = UtilsService.getArrayEnum(WhS_Invoice_InvoiceTypeEnum);
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined)
                    {
                        if(payload.extendedSettingsEntity != undefined)
                        {
                            $scope.scopeModel.selectedInvoiceType = UtilsService.getItemByVal($scope.scopeModel.invoiceTypes, payload.extendedSettingsEntity.InvoiceType, "value");
                        }
                    }
                }


                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.CarrierInvoiceSettings ,TOne.WhS.Invoice.Business",
                        InvoiceType: $scope.scopeModel.selectedInvoiceType.value,

                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);