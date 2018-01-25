'use strict';

app.directive('retailVoicePricevoiceeventstep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new PriceVoiceEventStepCtor(ctrl, $scope);
                ctor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/Retail_Voice/Directives/MainExtensions/TransformationSteps/Templates/PriceVoiceEventStepTemplate.html';
            }
        };

        function PriceVoiceEventStepCtor(ctrl, $scope) {
            this.initializeController = initializeController;

            var stepPayload;

            //Input Fields
            var accountBEDefinitionIdDirectiveReadyAPI;
            var accountBEDefinitionIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountIdDirectiveReadyAPI;
            var accountIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var serviceTypeIdDirectiveReadyAPI;
            var serviceTypeIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var mappedCDRDirectiveReadyAPI;
            var mappedCDRDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var durationDirectiveReadyAPI;
            var durationDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var eventTimeDirectiveReadyAPI;
            var eventTimeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            //Output Fields
            var packageIdDirectiveReadyAPI;
            var packageIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var chargingPolicyIdDirectiveReadyAPI;
            var chargingPolicyIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleDurationInSecondsDirectiveReadyAPI;
            var saleDurationInSecondsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var rateDirectiveReadyAPI;
            var rateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var amountDirectiveReadyAPI;
            var amountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var rateTypeIdDirectiveReadyAPI;
            var rateTypeIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyIdDirectiveReadyAPI;
            var currencyIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleRateValueRuleIdDirectiveReadyAPI;
            var saleRateValueRuleIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleRateTypeRuleIdDirectiveReadyAPI;
            var saleRateTypeRuleIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleTariffRuleIdDirectiveReadyAPI;
            var saleTariffRuleIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var saleExtraChargeRuleIdDirectiveReadyAPI;
            var saleExtraChargeRuleIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var voiceEventPricedPartsDirectiveReadyAPI;
            var voiceEventPricedPartsDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                //Input Fields
                $scope.scopeModel.onAccountBEDefinitionIdReady = function (api) {
                    accountBEDefinitionIdDirectiveReadyAPI = api;
                    accountBEDefinitionIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAccountIdReady = function (api) {
                    accountIdDirectiveReadyAPI = api;
                    accountIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onServiceTypeIdReady = function (api) {
                    serviceTypeIdDirectiveReadyAPI = api;
                    serviceTypeIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onMappedCDRReady = function (api) {
                    mappedCDRDirectiveReadyAPI = api;
                    mappedCDRDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onDurationReady = function (api) {
                    durationDirectiveReadyAPI = api;
                    durationDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onEventTimeReady = function (api) {
                    eventTimeDirectiveReadyAPI = api;
                    eventTimeDirectiveReadyPromiseDeferred.resolve();
                };

                //Output
                $scope.scopeModel.onPackageIdReady = function (api) {
                    packageIdDirectiveReadyAPI = api;
                    packageIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onChargingPolicyIdReady = function (api) {
                    chargingPolicyIdDirectiveReadyAPI = api;
                    chargingPolicyIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleDurationInSecondsReady = function (api) {
                    saleDurationInSecondsDirectiveReadyAPI = api;
                    saleDurationInSecondsDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onRateReady = function (api) {
                    rateDirectiveReadyAPI = api;
                    rateDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onAmountReady = function (api) {
                    amountDirectiveReadyAPI = api;
                    amountDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onRateTypeIdReady = function (api) {
                    rateTypeIdDirectiveReadyAPI = api;
                    rateTypeIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onCurrencyIdReady = function (api) {
                    currencyIdDirectiveReadyAPI = api;
                    currencyIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleRateValueRuleIdReady = function (api) {
                    saleRateValueRuleIdDirectiveReadyAPI = api;
                    saleRateValueRuleIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleRateTypeRuleIdReady = function (api) {
                    saleRateTypeRuleIdDirectiveReadyAPI = api;
                    saleRateTypeRuleIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleTariffRuleIdReady = function (api) {
                    saleTariffRuleIdDirectiveReadyAPI = api;
                    saleTariffRuleIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onSaleExtraChargeRuleIdReady = function (api) {
                    saleExtraChargeRuleIdDirectiveReadyAPI = api;
                    saleExtraChargeRuleIdDirectiveReadyPromiseDeferred.resolve();
                };
                $scope.scopeModel.onVoiceEventPricedPartsReady = function (api) {
                    voiceEventPricedPartsDirectiveReadyAPI = api;
                    voiceEventPricedPartsDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    stepPayload = payload;

                    //Input
                    //Loading AccountBEDefinitionId Directive
                    var accountBEDefinitionIdDirectiveLoadPromiseDeferred = getAccountBEDefinitionIdDirectiveLoadPromiseDeferred();
                    promises.push(accountBEDefinitionIdDirectiveLoadPromiseDeferred.promise);

                    //Loading AccountId Directive
                    var accountIdDirectiveLoadPromiseDeferred = getAccountIdDirectiveLoadPromiseDeferred();
                    promises.push(accountIdDirectiveLoadPromiseDeferred.promise);

                    //Loading ServiceTypeId Directive
                    var serviceTypeIdDirectiveLoadPromiseDeferred = getServiceTypeIdDirectiveLoadPromiseDeferred();
                    promises.push(serviceTypeIdDirectiveLoadPromiseDeferred.promise);

                    //Loading MappedCDR Directive
                    var mappedCDRDirectivePromiseDeferred = getMappedCDRDirectiveLoadPromiseDeferred();
                    promises.push(mappedCDRDirectivePromiseDeferred.promise);

                    //Loading Duration Directive
                    var durationDirectivePromiseDeferred = getDurationDirectivePromiseDeferred();
                    promises.push(durationDirectivePromiseDeferred.promise);

                    //Loading EventTime Directive
                    var eventTimeDirectivePromiseDeferred = getEventTimeDirectivePromiseDeferred();
                    promises.push(eventTimeDirectivePromiseDeferred.promise);

                    //Output
                    //Loading PackageId Directive
                    var packageIdDirectivePromiseDeferred = getPackageIdDirectivePromiseDeferred();
                    promises.push(packageIdDirectivePromiseDeferred.promise);

                    //Loading ChargingPolicyId Directive
                    var chargingPolicyIdDirectivePromiseDeferred = getChargingPolicyIdDirectivePromiseDeferred();
                    promises.push(chargingPolicyIdDirectivePromiseDeferred.promise);

                    //Loading SaleDurationInSeconds Directive
                    var saleDurationInSecondsDirectivePromiseDeferred = getSaleDurationInSecondsDirectivePromiseDeferred();
                    promises.push(saleDurationInSecondsDirectivePromiseDeferred.promise);

                    //Loading SaleRate Directive
                    var rateDirectivePromiseDeferred = getRateDirectivePromiseDeferred();
                    promises.push(rateDirectivePromiseDeferred.promise);

                    //Loading SaleAmount Directive
                    var amountDirectivePromiseDeferred = getAmountDirectivePromiseDeferred();
                    promises.push(amountDirectivePromiseDeferred.promise);

                    //Loading SaleRateTypeId Directive
                    var rateTypeIdDirectivePromiseDeferred = getRateTypeIdDirectivePromiseDeferred();
                    promises.push(rateTypeIdDirectivePromiseDeferred.promise);

                    //Loading SaleCurrencyId Directive
                    var currencyIdDirectivePromiseDeferred = getCurrencyIdDirectivePromiseDeferred();
                    promises.push(currencyIdDirectivePromiseDeferred.promise);

                    //Loading SaleRateValueRuleId Directive
                    var saleRateValueRuleIdPromiseDeferred = getSaleRateValueRuleIdDirectivePromiseDeferred();
                    promises.push(saleRateValueRuleIdPromiseDeferred.promise);

                    //Loading SaleRateTypeRuleId Directive
                    var saleRateTypeRuleIdDirectivePromiseDeferred = getSaleRateTypeRuleIdDirectivePromiseDeferred();
                    promises.push(saleRateTypeRuleIdDirectivePromiseDeferred.promise);

                    //Loading SaleTariffRuleId Directive
                    var saleTariffRuleIdDirectivePromiseDeferred = getSaleTariffRuleIdDirectivePromiseDeferred();
                    promises.push(saleTariffRuleIdDirectivePromiseDeferred.promise);

                    //Loading SaleExtraChargeRuleId Directive
                    var saleExtraChargeRuleIdDirectivePromiseDeferred = getSaleExtraChargeRuleIdDirectivePromiseDeferred();
                    promises.push(saleExtraChargeRuleIdDirectivePromiseDeferred.promise);

                    //Loading VoiceEventPricedParts Directive
                    var voiceEventPricedPartsDirectivePromiseDeferred = getVoiceEventPricedPartsDirectivePromiseDeferred();
                    promises.push(voiceEventPricedPartsDirectivePromiseDeferred.promise);

                    function getAccountBEDefinitionIdDirectiveLoadPromiseDeferred() {
                        var accountBEDefinitionIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        accountBEDefinitionIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var accountBEDefinitionIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                accountBEDefinitionIdPayload.selectedRecords = payload.stepDetails.AccountBEDefinitionID;

                            VRUIUtilsService.callDirectiveLoad(accountBEDefinitionIdDirectiveReadyAPI, accountBEDefinitionIdPayload, accountBEDefinitionIdDirectiveLoadPromiseDeferred);
                        });

                        return accountBEDefinitionIdDirectiveLoadPromiseDeferred;
                    }
                    function getAccountIdDirectiveLoadPromiseDeferred() {
                        var accountIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        accountIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var accountIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                accountIdPayload.selectedRecords = payload.stepDetails.AccountId;

                            VRUIUtilsService.callDirectiveLoad(accountIdDirectiveReadyAPI, accountIdPayload, accountIdDirectiveLoadPromiseDeferred);
                        });

                        return accountIdDirectiveLoadPromiseDeferred;
                    }
                    function getServiceTypeIdDirectiveLoadPromiseDeferred() {
                        var serviceTypeIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        serviceTypeIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var serviceTypeIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                serviceTypeIdPayload.selectedRecords = payload.stepDetails.ServiceTypeId;

                            VRUIUtilsService.callDirectiveLoad(serviceTypeIdDirectiveReadyAPI, serviceTypeIdPayload, serviceTypeIdDirectiveLoadPromiseDeferred);
                        });

                        return serviceTypeIdDirectiveLoadPromiseDeferred;
                    }
                    function getMappedCDRDirectiveLoadPromiseDeferred() {
                        var mappedCDRDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        mappedCDRDirectiveReadyPromiseDeferred.promise.then(function () {

                            var mappedCDRPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                mappedCDRPayload.selectedRecords = payload.stepDetails.MappedCDR;

                            VRUIUtilsService.callDirectiveLoad(mappedCDRDirectiveReadyAPI, mappedCDRPayload, mappedCDRDirectiveLoadPromiseDeferred);
                        });

                        return mappedCDRDirectiveLoadPromiseDeferred;
                    }
                    function getDurationDirectivePromiseDeferred() {
                        var durationDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        durationDirectiveReadyPromiseDeferred.promise.then(function () {

                            var durationPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                durationPayload.selectedRecords = payload.stepDetails.Duration;

                            VRUIUtilsService.callDirectiveLoad(durationDirectiveReadyAPI, durationPayload, durationDirectiveLoadPromiseDeferred);
                        });

                        return durationDirectiveLoadPromiseDeferred;
                    }
                    function getEventTimeDirectivePromiseDeferred() {
                        var eventTimeDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        eventTimeDirectiveReadyPromiseDeferred.promise.then(function () {

                            var eventTimePayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                eventTimePayload.selectedRecords = payload.stepDetails.EventTime;

                            VRUIUtilsService.callDirectiveLoad(eventTimeDirectiveReadyAPI, eventTimePayload, eventTimeDirectiveLoadPromiseDeferred);
                        });

                        return eventTimeDirectiveLoadPromiseDeferred;
                    }

                    function getPackageIdDirectivePromiseDeferred() {
                        var packageIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        packageIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var packageIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                packageIdPayload.selectedRecords = payload.stepDetails.PackageId;

                            VRUIUtilsService.callDirectiveLoad(packageIdDirectiveReadyAPI, packageIdPayload, packageIdDirectiveLoadPromiseDeferred);
                        });

                        return packageIdDirectiveLoadPromiseDeferred;
                    }
                    function getChargingPolicyIdDirectivePromiseDeferred() {
                        var chargingPolicyIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        chargingPolicyIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var chargingPolicyIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                chargingPolicyIdPayload.selectedRecords = payload.stepDetails.UsageChargingPolicyId;

                            VRUIUtilsService.callDirectiveLoad(chargingPolicyIdDirectiveReadyAPI, chargingPolicyIdPayload, chargingPolicyIdDirectiveLoadPromiseDeferred);
                        });

                        return chargingPolicyIdDirectiveLoadPromiseDeferred;
                    }
                    function getSaleDurationInSecondsDirectivePromiseDeferred() {
                        var saleDurationInSecondsDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        chargingPolicyIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleDurationInSecondsPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleDurationInSecondsPayload.selectedRecords = payload.stepDetails.SaleDurationInSeconds;

                            VRUIUtilsService.callDirectiveLoad(saleDurationInSecondsDirectiveReadyAPI, saleDurationInSecondsPayload, saleDurationInSecondsDirectiveLoadPromiseDeferred);
                        });

                        return saleDurationInSecondsDirectiveLoadPromiseDeferred;
                    }
                    function getRateDirectivePromiseDeferred() {
                        var rateDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        rateDirectiveReadyPromiseDeferred.promise.then(function () {

                            var ratePayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                ratePayload.selectedRecords = payload.stepDetails.SaleRate;

                            VRUIUtilsService.callDirectiveLoad(rateDirectiveReadyAPI, ratePayload, rateDirectiveLoadPromiseDeferred);
                        });

                        return rateDirectiveLoadPromiseDeferred;
                    }
                    function getAmountDirectivePromiseDeferred() {
                        var amountDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        amountDirectiveReadyPromiseDeferred.promise.then(function () {

                            var amountPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                amountPayload.selectedRecords = payload.stepDetails.SaleAmount;

                            VRUIUtilsService.callDirectiveLoad(amountDirectiveReadyAPI, amountPayload, amountDirectiveLoadPromiseDeferred);
                        });

                        return amountDirectiveLoadPromiseDeferred;
                    }
                    function getRateTypeIdDirectivePromiseDeferred() {
                        var rateTypeIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        rateTypeIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var rateTypeIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                rateTypeIdPayload.selectedRecords = payload.stepDetails.SaleRateTypeId;

                            VRUIUtilsService.callDirectiveLoad(rateTypeIdDirectiveReadyAPI, rateTypeIdPayload, rateTypeIdDirectiveLoadPromiseDeferred);
                        });

                        return rateTypeIdDirectiveLoadPromiseDeferred;
                    }
                    function getCurrencyIdDirectivePromiseDeferred() {
                        var currencyIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var currencyIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                currencyIdPayload.selectedRecords = payload.stepDetails.SaleCurrencyId;

                            VRUIUtilsService.callDirectiveLoad(currencyIdDirectiveReadyAPI, currencyIdPayload, currencyIdDirectiveLoadPromiseDeferred);
                        });

                        return currencyIdDirectiveLoadPromiseDeferred;
                    }
                    function getSaleRateValueRuleIdDirectivePromiseDeferred() {
                        var saleRateValueRuleIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleRateValueRuleIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleRateValueRuleIdPayload.selectedRecords = payload.stepDetails.SaleRateValueRuleId;

                            VRUIUtilsService.callDirectiveLoad(saleRateValueRuleIdDirectiveReadyAPI, saleRateValueRuleIdPayload, saleRateValueRuleIdDirectiveLoadPromiseDeferred);
                        });

                        return saleRateValueRuleIdDirectiveLoadPromiseDeferred;
                    }
                    function getSaleRateTypeRuleIdDirectivePromiseDeferred() {
                        var saleRateTypeRuleIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        saleRateTypeRuleIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleRateTypeRuleIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleRateTypeRuleIdPayload.selectedRecords = payload.stepDetails.SaleRateTypeRuleId;

                            VRUIUtilsService.callDirectiveLoad(saleRateTypeRuleIdDirectiveReadyAPI, saleRateTypeRuleIdPayload, saleRateTypeRuleIdDirectiveLoadPromiseDeferred);
                        });

                        return saleRateTypeRuleIdDirectiveLoadPromiseDeferred;
                    }
                    function getSaleTariffRuleIdDirectivePromiseDeferred() {
                        var saleTariffRuleIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        saleRateTypeRuleIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleTariffRuleIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleTariffRuleIdPayload.selectedRecords = payload.stepDetails.SaleTariffRuleId;

                            VRUIUtilsService.callDirectiveLoad(saleTariffRuleIdDirectiveReadyAPI, saleTariffRuleIdPayload, saleTariffRuleIdDirectiveLoadPromiseDeferred);
                        });

                        return saleTariffRuleIdDirectiveLoadPromiseDeferred;
                    }
                    function getSaleExtraChargeRuleIdDirectivePromiseDeferred() {
                        var saleExtraChargeRuleIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        saleExtraChargeRuleIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var saleExtraChargeRuleIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                saleExtraChargeRuleIdPayload.selectedRecords = payload.stepDetails.SaleExtraChargeRuleId;

                            VRUIUtilsService.callDirectiveLoad(saleExtraChargeRuleIdDirectiveReadyAPI, saleExtraChargeRuleIdPayload, saleExtraChargeRuleIdDirectiveLoadPromiseDeferred);
                        });

                        return saleExtraChargeRuleIdDirectiveLoadPromiseDeferred;
                    }
                    function getVoiceEventPricedPartsDirectivePromiseDeferred() {
                        var voiceEventPricedPartsDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        voiceEventPricedPartsDirectiveReadyPromiseDeferred.promise.then(function () {

                            var voiceEventPricedPartsPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                voiceEventPricedPartsPayload.selectedRecords = payload.stepDetails.VoiceEventPricedParts;

                            VRUIUtilsService.callDirectiveLoad(voiceEventPricedPartsDirectiveReadyAPI, voiceEventPricedPartsPayload, voiceEventPricedPartsDirectiveLoadPromiseDeferred);
                        });

                        return voiceEventPricedPartsDirectiveLoadPromiseDeferred;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Retail.Voice.MainExtensions.TransformationSteps.PriceVoiceEventStep, Retail.Voice.MainExtensions",
                        AccountBEDefinitionID: accountBEDefinitionIdDirectiveReadyAPI.getData(),
                        AccountId: accountIdDirectiveReadyAPI.getData(),
                        ServiceTypeId: serviceTypeIdDirectiveReadyAPI.getData(),
                        MappedCDR: mappedCDRDirectiveReadyAPI.getData(),
                        Duration: durationDirectiveReadyAPI.getData(),
                        EventTime: eventTimeDirectiveReadyAPI.getData(),

                        PackageId: packageIdDirectiveReadyAPI.getData(),
                        UsageChargingPolicyId: chargingPolicyIdDirectiveReadyAPI.getData(),
                        SaleDurationInSeconds: saleDurationInSecondsDirectiveReadyAPI.getData(),
                        SaleRate: rateDirectiveReadyAPI.getData(),
                        SaleAmount: amountDirectiveReadyAPI.getData(),
                        SaleRateTypeId: rateTypeIdDirectiveReadyAPI.getData(),
                        SaleCurrencyId: currencyIdDirectiveReadyAPI.getData(),
                        SaleRateValueRuleId: saleRateValueRuleIdDirectiveReadyAPI.getData(),
                        SaleRateTypeRuleId: saleRateTypeRuleIdDirectiveReadyAPI.getData(),
                        SaleTariffRuleId: saleTariffRuleIdDirectiveReadyAPI.getData(),
                        SaleExtraChargeRuleId: saleExtraChargeRuleIdDirectiveReadyAPI.getData(),
                        VoiceEventPricedParts: voiceEventPricedPartsDirectiveReadyAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);

