"use strict";

app.directive("vrInvoiceBillingperiodMonthly", ["UtilsService", "VR_Invoice_InvoiceMonthlyEnum",
    function (UtilsService, VR_Invoice_InvoiceMonthlyEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new MonthlyBillingPeriod($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/BillingCycle/MainExtensions/BillingPeriod/Templates/MonthlyBillingPeriodTemplate.html"

        };

        function MonthlyBillingPeriod($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.monthlyTypes = UtilsService.getArrayEnum(VR_Invoice_InvoiceMonthlyEnum);
                    if (payload != undefined) {
                        $scope.scopeModel.specificDay = payload.SpecificDay;
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.MonthlyBillingPeriod ,Vanrise.Invoice.MainExtensions",
                        monthlyTypes: $scope.scopeModel.monthlyTypes,
                        SpecificDay: $scope.scopeModel.specificDay
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);