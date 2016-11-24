"use strict";

app.directive("vrInvoicetypeInvoicegeneratorfilterconditionPartner", ["UtilsService", "VRNotificationService", "VRUIUtilsService","WhS_Invoice_CarrierTypeEnum",
    function (UtilsService, VRNotificationService, VRUIUtilsService, WhS_Invoice_CarrierTypeEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new PartnerCondition($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/Extensions/InvoiceGeneratorFilterCondition/Templates/PartnerConditionTemplate.html"

        };

        function PartnerCondition($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var context;
            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.carrierTypes = UtilsService.getArrayEnum(WhS_Invoice_CarrierTypeEnum);
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var invoiceFilterConditionEntity;
                    if (payload != undefined) {
                        invoiceFilterConditionEntity = payload.invoiceFilterConditionEntity;
                        context = payload.context;
                        if (invoiceFilterConditionEntity != undefined) {
                            $scope.scopeModel.selectedCarrierType = UtilsService.getItemByVal($scope.scopeModel.carrierTypes, invoiceFilterConditionEntity.CarrierType, "value");
                        }
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.InvoiceGeneratorActionPartnerCondition ,TOne.WhS.Invoice.Business",
                        CarrierType: $scope.scopeModel.selectedCarrierType.value
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function getContext() {
                var currentContext = context;
                if (currentContext == undefined)
                    currentContext = {};
                return currentContext;
            }
        }

        return directiveDefinitionObject;

    }
]);