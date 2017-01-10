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
            var counter = 0;
            function initializeController() {
                ctrl.data = [];
                ctrl.monthlyTypes = UtilsService.getArrayEnum(VR_Invoice_InvoiceMonthlyEnum);
                ctrl.addMonthlyPeriod = function () {
                    var obj = {
                        Id: counter++,
                        MonthlyId: ctrl.selectedMonthlyType.value,
                        MonthlyType: ctrl.selectedMonthlyType.description,
                        SpecificDay: ctrl.specificDay
                    };
                    ctrl.data.push({ Entity: obj });
                    ctrl.specificDay = undefined;
                    ctrl.selectedMonthlyType = undefined;
                };
                ctrl.isValid = function () {
                    if (ctrl.data != undefined && ctrl.data.length > 0) {

                        if (ctrl.data.length == 1)
                            return null;

                        var count = 0;
                        for (var i = 0; i < ctrl.data.length; i++) {

                            var item = ctrl.data[i].Entity;
                            if (item.MonthlyId == VR_Invoice_InvoiceMonthlyEnum.LastDay.value) {
                                if (ctrl.data.length != i +1)
                                    return "Wrong position to Last Day";
                            }
                            if (i + 1 < ctrl.data.length) {
                                var nextItem = ctrl.data[i + 1].Entity;
                                if (parseInt(item.SpecificDay) >= parseInt(nextItem.SpecificDay))
                                    return "Wrong position to specific day: " + nextItem.SpecificDay;
                            }
                        }
                        return null;
                    }
                    return "You Should add at least one day.";
                };

                ctrl.removeItem = function (dataItem) {
                    var index = ctrl.data.indexOf(dataItem);
                    ctrl.data.splice(index, 1);
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.data.length = 0;
                    if (payload != undefined && payload.MonthlyPeriods != undefined) {
                        for (var i = 0; i < payload.MonthlyPeriods.length; i++)
                        {
                            var item = payload.MonthlyPeriods[i];
                            var monthlyType = UtilsService.getItemByVal(ctrl.monthlyTypes, item.MonthlyType,"value")
                            ctrl.data.push({
                                Entity: {
                                    Id: counter++,
                                    MonthlyId: monthlyType.value,
                                    MonthlyType: monthlyType.description,
                                    SpecificDay: item.SpecificDay
                                }
                            });
                        }
                    }
                  
                };

                api.getData = function () 
                {
                    var monthlyPeriods;
                    if(ctrl.data != undefined)
                    {
                        monthlyPeriods = [];
                        for(var i=0;i<ctrl.data.length;i++)
                        {
                            var item = ctrl.data[i];
                            monthlyPeriods.push({
                                MonthlyType: item.Entity.MonthlyId,
                                SpecificDay: item.Entity.SpecificDay
                            });
                        }
                    }
                    return {
                        $type: "Vanrise.Invoice.MainExtensions.MonthlyBillingPeriod ,Vanrise.Invoice.MainExtensions",
                        MonthlyPeriods: monthlyPeriods,
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);