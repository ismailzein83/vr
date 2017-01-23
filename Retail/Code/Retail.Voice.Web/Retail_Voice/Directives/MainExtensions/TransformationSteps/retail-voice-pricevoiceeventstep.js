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
            var stepPayload;

            //Input Fields
            var accountBEDefinitionIdDirectiveReadyAPI;
            var accountBEDefinitionIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var accountIdDirectiveReadyAPI;
            var accountIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var serviceTypeIdDirectiveReadyAPI;
            var serviceTypeIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var rawCDRDirectiveReadyAPI;
            var rawCDRDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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

            var rateDirectiveReadyAPI;
            var rateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var amountDirectiveReadyAPI;
            var amountDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var rateTypeIdDirectiveReadyAPI;
            var rateTypeIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var currencyIdDirectiveReadyAPI;
            var currencyIdDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

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
                $scope.scopeModel.onRawCDRReady = function (api) {
                    rawCDRDirectiveReadyAPI = api;
                    rawCDRDirectiveReadyPromiseDeferred.resolve();
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

                    //Loading RawCDR Directive
                    var rawCDRDirectivePromiseDeferred = getRawCDRDirectiveLoadPromiseDeferred();
                    promises.push(rawCDRDirectivePromiseDeferred.promise);

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

                    //Loading CahrgingPolicyId Directive
                    var chargingPolicyIdDirectivePromiseDeferred = getChargingPolicyIdDirectivePromiseDeferred();
                    promises.push(chargingPolicyIdDirectivePromiseDeferred.promise);

                    //Loading Rate Directive
                    var rateDirectivePromiseDeferred = getRateDirectivePromiseDeferred();
                    promises.push(rateDirectivePromiseDeferred.promise);

                    //Loading Amount Directive
                    var amountDirectivePromiseDeferred = getAmountDirectivePromiseDeferred();
                    promises.push(amountDirectivePromiseDeferred.promise);

                    //Loading RateTypeId Directive
                    var rateTypeIdDirectivePromiseDeferred = getRateTypeIdDirectivePromiseDeferred();
                    promises.push(rateTypeIdDirectivePromiseDeferred.promise);

                    //Loading CurrencyId Directive
                    var currencyIdDirectivePromiseDeferred = getCurrencyIdDirectivePromiseDeferred();
                    promises.push(currencyIdDirectivePromiseDeferred.promise);

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
                    function getRawCDRDirectiveLoadPromiseDeferred() {
                        var rawCDRDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        rawCDRDirectiveReadyPromiseDeferred.promise.then(function () {

                            var rawCDRPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                rawCDRPayload.selectedRecords = payload.stepDetails.RawCDR;

                            VRUIUtilsService.callDirectiveLoad(rawCDRDirectiveReadyAPI, rawCDRPayload, rawCDRDirectiveLoadPromiseDeferred);
                        });

                        return rawCDRDirectiveLoadPromiseDeferred;
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
                    function getRateDirectivePromiseDeferred() {
                        var rateDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        rateDirectiveReadyPromiseDeferred.promise.then(function () {

                            var ratePayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                ratePayload.selectedRecords = payload.stepDetails.Rate;

                            VRUIUtilsService.callDirectiveLoad(rateDirectiveReadyAPI, ratePayload, rateDirectiveLoadPromiseDeferred);
                        });

                        return rateDirectiveLoadPromiseDeferred;
                    }
                    function getAmountDirectivePromiseDeferred() {
                        var amountDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        amountDirectiveReadyPromiseDeferred.promise.then(function () {

                            var amountPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                amountPayload.selectedRecords = payload.stepDetails.Amount;

                            VRUIUtilsService.callDirectiveLoad(amountDirectiveReadyAPI, amountPayload, amountDirectiveLoadPromiseDeferred);
                        });

                        return amountDirectiveLoadPromiseDeferred;
                    }
                    function getRateTypeIdDirectivePromiseDeferred() {
                        var rateTypeIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        rateTypeIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var rateTypeIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                rateTypeIdPayload.selectedRecords = payload.stepDetails.RateTypeId;

                            VRUIUtilsService.callDirectiveLoad(rateTypeIdDirectiveReadyAPI, rateTypeIdPayload, rateTypeIdDirectiveLoadPromiseDeferred);
                        });

                        return rateTypeIdDirectiveLoadPromiseDeferred;
                    }
                    function getCurrencyIdDirectivePromiseDeferred() {
                        var currencyIdDirectiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();

                        currencyIdDirectiveReadyPromiseDeferred.promise.then(function () {

                            var currencyIdPayload = { context: payload.context };
                            if (payload.stepDetails != undefined)
                                currencyIdPayload.selectedRecords = payload.stepDetails.CurrencyId;

                            VRUIUtilsService.callDirectiveLoad(currencyIdDirectiveReadyAPI, currencyIdPayload, currencyIdDirectiveLoadPromiseDeferred);
                        });

                        return currencyIdDirectiveLoadPromiseDeferred;
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
                        AccountBEDefinitionID:accountBEDefinitionIdDirectiveReadyAPI.getData(),
                        AccountId: accountIdDirectiveReadyAPI.getData(),
                        ServiceTypeId: serviceTypeIdDirectiveReadyAPI.getData(),
                        RawCDR: rawCDRDirectiveReadyAPI.getData(),
                        MappedCDR: mappedCDRDirectiveReadyAPI.getData(),
                        Duration: durationDirectiveReadyAPI.getData(),
                        EventTime: eventTimeDirectiveReadyAPI.getData(),

                        PackageId: packageIdDirectiveReadyAPI.getData(),
                        UsageChargingPolicyId: chargingPolicyIdDirectiveReadyAPI.getData(),
                        Rate: rateDirectiveReadyAPI.getData(),
                        Amount: amountDirectiveReadyAPI.getData(),
                        RateTypeId: rateTypeIdDirectiveReadyAPI.getData(),
                        CurrencyId: currencyIdDirectiveReadyAPI.getData(),
                        VoiceEventPricedParts: voiceEventPricedPartsDirectiveReadyAPI.getData()
                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }

        return directiveDefinitionObject;
    }
]);

