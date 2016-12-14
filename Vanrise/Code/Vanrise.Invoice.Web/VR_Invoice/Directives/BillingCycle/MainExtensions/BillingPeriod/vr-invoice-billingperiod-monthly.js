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
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.data = [];

                    ctrl.monthlyTypes = UtilsService.getArrayEnum(VR_Invoice_InvoiceMonthlyEnum);
                    if (payload != undefined) {
                        ctrl.specificDay = payload.SpecificDay;
                    }

                    ctrl.addMonthlyPeriod = function () {
                        var obj = {
                            Id: ctrl.data.length + 1,
                            monthlyId: ctrl.selectedMonthlyType.value,
                            monthlyType: ctrl.selectedMonthlyType.description,
                            specificDay: ctrl.specificDay
                        }
                        ctrl.data.push(obj);
                        ctrl.specificDay = undefined;
                        ctrl.selectedMonthlyType = undefined;
                    };

                    ctrl.isValid = function () {
                        if (ctrl.data != undefined && ctrl.data.length > 0) {

                            if (ctrl.data.length == 1)
                                return null;

                            var count = 0;
                            for (var i = 0; i < ctrl.data.length; i++) {
                                var count2 = 0;
                                var item = ctrl.data[i];
                                if (item.monthlyId == 1) {
                                    count = count + 1;
                                } else {
                                    for (var j = 0; j < ctrl.data.length; j++) {
                                        var item2 = ctrl.data[j];
                                        if (item2.specificDay == item.specificDay) {
                                            count2 = count2 + 1;
                                        }
                                    }
                                    if(count2 > 1)
                                        return "Same Day added";
                                }
                            }

                            if (count > 1)
                                return "Same Day added";
                            return null;
                        }
                        return "You Should add at least one day.";
                    };

                    ctrl.removeItem = function (dataItem) {
                        var index = ctrl.data.indexOf(dataItem);
                        ctrl.data.splice(index, 1);
                    };
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