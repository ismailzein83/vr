﻿"use strict";

app.directive("retailBeRecurringchargeperiodsettingsMonthly", ["UtilsService", "VRNotificationService", "VRUIUtilsService",
    function (UtilsService, VRNotificationService, VRUIUtilsService) {

        var directiveDefinitionObject = {

            restrict: "E",
            scope:
            {
                onReady: "=",
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var ctor = new MonthlyRecurringCharge($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            compile: function (element, attrs) {

            },
            templateUrl: "/Client/Modules/Retail_BusinessEntity/Directives/RecurringChargePeriod/MainExtensions/Templates/MonthlyRecurringChargePeriodTemplate.html"
        };

        function MonthlyRecurringCharge($scope, ctrl, $attrs) {
            this.initializeController = initializeController;


            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.inAdvance = false;
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    if (payload != undefined) {
                        if (payload.extendedSettings != undefined) {
                            $scope.scopeModel.inAdvance = payload.extendedSettings.InAdvance;
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "TOne.WhS.BusinessEntity.MainExtensions.RecurringCharges.MonthlyRecuringCharge ,TOne.WhS.BusinessEntity.MainExtensions",
                        InAdvance: $scope.scopeModel.inAdvance
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;

    }
]);