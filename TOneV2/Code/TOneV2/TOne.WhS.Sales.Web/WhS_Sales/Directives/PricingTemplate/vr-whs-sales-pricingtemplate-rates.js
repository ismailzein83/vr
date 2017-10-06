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

                $scope.scopeModel.onMarginRateCalculationReady = function (api) {
                    marginRateCalculationDirectiveAPI = api;
                    marginRateCalculationDirectiveReadyDeferred.resolve();
                };

                $scope.scopeModel.onAddPricingTemplateRate = function () {

                    var entity = buildPricingTemplateRateEntity($scope.scopeModel.pricingTemplateRates.length, undefined);

                    entity.onMarginRateCalculationReady = function (api) {
                        entity.MarginRateCalculationDirectiveAPI = api;

                        var marginRateCalculationDirectivePayload;

                        var setLoader = function (value) {
                            $scope.scopeModel.isLoading = value;
                        };
                        VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, api, marginRateCalculationDirectivePayload, setLoader, undefined);
                    };

                    $scope.scopeModel.pricingTemplateRates.push({ Entity: entity });
                };

                $scope.scopeModel.onDeletePricingTemplateRate = function (deletedPricingTemplateRate) {
                    var index = UtilsService.getItemIndexByVal($scope.scopeModel.pricingTemplateRates, deletedPricingTemplateRate.pricingTemplateRateIndex, 'Entity.pricingTemplateRateIndex');
                    $scope.scopeModel.pricingTemplateRates.splice(index, 1);
                };

                $scope.scopeModel.validate = function () {
                    if ($scope.scopeModel.pricingTemplateRates.length == 0)
                        return "You Should define at least one rate!!";
                    return null;
                };

                defineAPI();
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
                            promises.push(loadPricingTemplateRate(buildPricingTemplateRateEntity(index, currentRate)));
                        }
                    }
                    else {
                        //Creating first rate @AddMode
                        promises.push(loadPricingTemplateRate(buildPricingTemplateRateEntity(0, undefined))); 
                    } 

                    function loadPricingTemplateRate(pricingTemplateRate) {

                        var currentRateLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        pricingTemplateRate.onMarginRateCalculationReady = function (api) {
                            pricingTemplateRate.MarginRateCalculationDirectiveAPI = api;

                            var marginRateCalculationDirectivePayload;
                            if (pricingTemplateRate.marginRateCalculation) {
                                marginRateCalculationDirectivePayload = { marginRateCalculation: pricingTemplateRate.marginRateCalculation };
                            }
                            VRUIUtilsService.callDirectiveLoad(api, marginRateCalculationDirectivePayload, currentRateLoadPromiseDeferred);
                        };

                        $scope.scopeModel.pricingTemplateRates.push({ Entity: pricingTemplateRate });

                        return currentRateLoadPromiseDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var rates = [];

                    for (var index = 0; index < $scope.scopeModel.pricingTemplateRates.length; index++) {
                        var currentRate = $scope.scopeModel.pricingTemplateRates[index].Entity;
                        rates.push({
                            MarginRateCalculation: currentRate.MarginRateCalculationDirectiveAPI.getData(),
                            Margin: currentRate.margin,
                            MarginPercentage: currentRate.marginPercentage,
                            FromRate: currentRate.fromRate,
                            ToRate: currentRate.toRate,
                            MinRate: currentRate.minRate,
                            MaxRate: currentRate.maxRate
                        });
                    }
                    return rates;
                };

                //ToBeRemoved
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

            function buildPricingTemplateRateEntity(pricingTemplateRateIndex, rate) {
                var entity = {
                    pricingTemplateRateIndex: pricingTemplateRateIndex,
                    validateRequiredMargin: validateRequiredMargin,
                    validateMargin: validateMargin,
                    validateMarginPercentage: validateMarginPercentage,
                    validateFromToRates: validateFromToRates
                };

                if (rate != undefined) {
                    entity.marginRateCalculation = rate.MarginRateCalculation;
                    entity.margin = rate.Margin;
                    entity.marginPercentage = rate.MarginPercentage;
                    entity.fromRate = rate.FromRate;
                    entity.toRate = rate.ToRate;
                    entity.minRate = rate.MinRate;
                    entity.maxRate = rate.MaxRate;
                }

                return entity;
            }

            function validateRequiredMargin(dataItem) {
                if (dataItem.margin == undefined && dataItem.marginPercentage == undefined)
                    return "at least one margin should be set";
                return null;
            }
            function validateMargin(dataItem) {
                if (dataItem.margin == undefined)
                    return null;
                if (dataItem.margin == 0)
                    return 'Margin must be different than 0';
                return null;
            }
            function validateMarginPercentage(dataItem) {
                if (dataItem.marginPercentage != undefined) {
                    if (dataItem.marginPercentage < 0 || dataItem.marginPercentage > 100)
                        return 'Margin % must be between 0 and 100';
                    if (dataItem.margin == 0)
                        return 'Margin must be different than 0';
                }
                return null;
            }
            function validateFromToRates(dataItem) {

                if (dataItem.fromRate == undefined || dataItem.toRate == undefined)
                    return null;

                if (dataItem.fromRate >= dataItem.toRate)
                    return "FromRate should be less than ToRate";

                if ($scope.scopeModel.pricingTemplateRates.length > 0) {
                    for (var index = 0; index < $scope.scopeModel.pricingTemplateRates.length; index++) {
                        var currentPrincingTemplateRate = $scope.scopeModel.pricingTemplateRates[index];
                        if (currentPrincingTemplateRate.Entity.pricingTemplateRateIndex == dataItem.pricingTemplateRateIndex)
                            continue;

                        if (dataItem.fromRate < currentPrincingTemplateRate.Entity.toRate && dataItem.toRate > currentPrincingTemplateRate.Entity.fromRate)
                            return "overlaps with another existing interval";
                    }
                }

                return null;
            }
        }
    }]);