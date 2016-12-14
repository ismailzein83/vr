"use strict";

app.directive("vrInvoiceBillingperiodBiweekly", ["UtilsService", "VR_Invoice_InvoiceDailyEnum",
    function (UtilsService, VR_Invoice_InvoiceDailyEnum) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new BiWeeklyBillingPeriod($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/VR_Invoice/Directives/BillingCycle/MainExtensions/BillingPeriod/Templates/BiWeeklyBillingPeriodTemplate.html"

        };

        function BiWeeklyBillingPeriod($scope, ctrl, $attrs) {
            this.initializeController = initializeController;
            function initializeController() {
                $scope.scopeModel = {};
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    $scope.scopeModel.dailyTypes = UtilsService.getArrayEnum(VR_Invoice_InvoiceDailyEnum);
                    if (payload != undefined) {
                    }
                    var promises = [];
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.BiWeeklyBillingPeriod ,Vanrise.Invoice.MainExtensions",
                        dailyTypes: $scope.scopeModel.dailyTypes
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);