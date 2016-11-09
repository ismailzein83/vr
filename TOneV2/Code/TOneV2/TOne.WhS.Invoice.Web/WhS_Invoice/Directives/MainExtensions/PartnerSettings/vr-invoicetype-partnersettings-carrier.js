"use strict";

app.directive("vrInvoicetypePartnersettingsCarrier", ["UtilsService", "VRNotificationService",
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
            templateUrl: "/Client/Modules/WhS_Invoice/Directives/MainExtensions/PartnerSettings/Templates/CarrierPartnerSettingsTemplate.html"

        };

        function CustomerInvoiceGenerator($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if(payload != undefined)
                    {
                        $scope.useMaskInfo = payload.UseMaskInfo;
                        $scope.partnerSelector = payload.PartnerSelector;
                        $scope.partnerManagerFQTN = UtilsService.serializetoJson(payload.PartnerManagerFQTN);
                        $scope.partnerFilterSelector = payload.PartnerFilterSelector;
                    }
                }

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.Invoice.Business.Extensions.CarrierPartnerSettings ,TOne.WhS.Invoice.Business",
                        UseMaskInfo: $scope.useMaskInfo,
                        PartnerSelector: $scope.partnerSelector,
                        PartnerManagerFQTN: UtilsService.parseStringToJson($scope.partnerManagerFQTN),
                        PartnerFilterSelector: $scope.partnerFilterSelector
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);