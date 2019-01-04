﻿"use strict";

app.directive("vrWhsSalesFixedratecalculation", ['WhS_Sales_BulkActionUtilsService', 'UtilsService', function (WhS_Sales_BulkActionUtilsService, UtilsService) {

    return {
        restrict: "E",
        scope: {
            onReady: "=",
            normalColNum: '@',
            isrequired: '='
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            var fixedRateCalculation = new FixedRateCalculation(ctrl, $scope);
            fixedRateCalculation.initializeController();
        },
        controllerAs: "ctrl",
        bindToController: true,
        templateUrl: "/Client/Modules/WhS_Sales/Directives/Extensions/RateCalculation/Templates/FixedRateCalculationTemplate.html"
    };

    function FixedRateCalculation(ctrl, $scope) {

        this.initializeController = initializeController;

        var bulkActionContext;

        function initializeController() {

            ctrl.validate = function () {
                if (ctrl.fixedRate == undefined)
                    return null;
                var fixedRateAsNumber = Number(ctrl.fixedRate);
                if (isNaN(fixedRateAsNumber))
                    return 'Value is an invalid number';
                if (ctrl.allowRateZero && fixedRateAsNumber < 0)
                    return 'Value must be greater than or equal to zero';
                if (!ctrl.allowRateZero && fixedRateAsNumber <= 0)
                    return 'Value must be greater than zero';
                return null;
            };

            defineAPI();
        }
        function defineAPI() {
            var api = {};

            api.load = function (payload) {

                var rateCalculationMethod;

                if (payload != undefined) {
                    rateCalculationMethod = payload.rateCalculationMethod;
                    bulkActionContext = payload.bulkActionContext;
                }

                if (bulkActionContext != undefined) {
                    ctrl.longPrecision = bulkActionContext.longPrecision;
                    ctrl.allowRateZero = bulkActionContext.allowRateZero;
                }

                if (rateCalculationMethod != undefined) {
                    ctrl.fixedRate = rateCalculationMethod.FixedRate;
                }
            };

            api.getData = function () {
                return {
                    $type: "TOne.WhS.Sales.MainExtensions.RateCalculation.FixedRateCalculationMethod, TOne.WhS.Sales.MainExtensions",
                    FixedRate: ctrl.fixedRate
                };
            };

            api.getDescription = function () {
                return 'Value: ' + UtilsService.round(ctrl.fixedRate, bulkActionContext.longPrecision);
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }
    }
}]);