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
                            var count2 = 0;
                            var item = ctrl.data[i].Entity;
                            if (item.MonthlyId == VR_Invoice_InvoiceMonthlyEnum.LastDay.value) {
                                count = count + 1;
                            } else {
                                for (var j = 0; j < ctrl.data.length; j++) {
                                    var item2 = ctrl.data[j].Entity;
                                    if (item2.SpecificDay == item.SpecificDay) {
                                        count2 = count2 + 1;
                                    }
                                }
                                if (count2 > 1)
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