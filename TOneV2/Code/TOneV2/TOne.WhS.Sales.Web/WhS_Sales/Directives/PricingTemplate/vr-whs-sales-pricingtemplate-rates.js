"use strict";

app.directive("vrWhsSalesPricingtemplateRates", ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        return {
            restrict: "E",
            scope: {
                onReady: "=",
                normalColNum: '@',
                isrequired: '='
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new RatePricingTemplateCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/WhS_Sales/Directives/PricingTemplate/Templates/PricingTemplateRatesTemplate.html"
        };

        function RatePricingTemplateCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var marginRateCalculationDirectiveAPI;
            var marginRateCalculationDirectiveReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.pricingTemplateRates = [];

                $scope.scopeModel.selectedFields = [{ fieldTitle: 'AliAtoui' }, { fieldTitle: 'AliAtoui22' }];

                $scope.scopeModel.onMarginRateCalculationReady = function (api) {
                    marginRateCalculationDirectiveAPI = api;
                    marginRateCalculationDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddPricingTemplateRate = function () {
                    var entity = {
                        Margin: $scope.scopeModel.margin,
                        MarginPercentage: $scope.scopeModel.marginPercentage,
                        MarginRateCalculation: marginRateCalculationDirectiveAPI.getData(),
                        //MarginRateCalculationDescription: marginRateCalculationDirectiveAPI.getDescription() 
                        FromRate:$scope.scopeModel.fromRate,
                        ToRate: $scope.scopeModel.toRate,
                        MinRate: $scope.scopeModel.minRate,
                        MaxRate: $scope.scopeModel.maxRate
                    };
                    $scope.scopeModel.pricingTemplateRates.push({ Entity: entity });
                };

                $scope.scopeModel.validateRequiredMargin = function () {
                    if ($scope.scopeModel.margin == undefined && $scope.scopeModel.marginPercentage == undefined)
                        return "at least one margin should be set";
                    return null;

                };
                $scope.scopeModel.validateMargin = function () {
                    if ($scope.scopeModel.margin == undefined)
                        return;
                    if ($scope.scopeModel.margin == 0)
                        return 'Margin must be different than 0';
                    return null;
                };
                $scope.scopeModel.validateMarginPercentage = function () {
                    if ($scope.scopeModel.marginPercentage != undefined) {
                        if ($scope.scopeModel.marginPercentage < 0 || $scope.scopeModel.marginPercentage > 100)
                            return 'Margin % must be between 0 and 100';
                        if ($scope.scopeModel.margin == 0)
                            return 'Margin must be different than 0';
                    }
                    return null;
                };

                UtilsService.waitMultiplePromises([marginRateCalculationDirectiveReadyDeferred.promise]).then(function () {
                    defineAPI();
                });
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var rates;

                    if (payload != undefined) {
                        rates = payload.rates;
                    }

                    //Loading Grid
                    if (rates != undefined) {
                        for (var index = 0; index < rates.length; index++) {
                            var currentRate = rates[index];
                            //var entity = {
                            //    Margin: currentRate.Margin,
                            //    MarginPercentage: currentRate.MarginPercentage,
                            //    MarginRateCalculation: currentRate.MarginRateCalculation
                            //};
                            $scope.scopeModel.pricingTemplateRates.push({ Entity: currentRate });
                        }
                    }

                    var loadMarginRateCalculationDirectivePromise = loadMarginRateCalculationDirective();
                    promises.push(loadMarginRateCalculationDirectivePromise);

                    function loadMarginRateCalculationDirective() {
                        var marginRateCalculationDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        var marginRateCalculationDirectivePayload;
                        VRUIUtilsService.callDirectiveLoad(marginRateCalculationDirectiveAPI, marginRateCalculationDirectivePayload, marginRateCalculationDirectiveLoadDeferred);

                        return marginRateCalculationDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var rates = [];

                    for (var index = 0; index < $scope.scopeModel.pricingTemplateRates.length; index++) {
                        var currentRate = $scope.scopeModel.pricingTemplateRates[index].Entity;
                        rates.push(currentRate);
                        //rates.push({
                        //    Margin: currentRate.Margin,
                        //    MarginPercentage: currentRate.MarginPercentage,
                        //    MarginRateCalculation: currentRate.MarginRateCalculation
                        //});
                    }
                    return rates;
                };

                //ToRemove
                api.getDescription = function () {
                    var marginPercentageDescription;
                    if ($scope.scopeModel.marginPercentage != undefined && $scope.scopeModel.margin != undefined)
                        return 'Value: ' + $scope.scopeModel.margin + ' , Percentage: ' + $scope.scopeModel.marginPercentage;
                    else {
                        if ($scope.scopeModel.margin != undefined)
                            return 'Value: ' + $scope.scopeModel.margin;
                        if ($scope.scopeModel.marginPercentage != undefined)
                            return ' Precentage: ' + $scope.scopeModel.marginPercentage;
                    }
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }
    }]);