(function (app) {

    'use strict';

    VoiceServiceTypeDirective.$inject = ['UtilsService', 'VRUIUtilsService', 'VRNotificationService', 'Retail_BE_PricingPackageSettingsService', 'Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum'];

    function VoiceServiceTypeDirective(UtilsService, VRUIUtilsService, VRNotificationService, Retail_BE_PricingPackageSettingsService, Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;

                var voiceServiceTypeCtor = new VoiceServiceTypeCtor($scope, ctrl);
                voiceServiceTypeCtor.initializeController();
            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: '/Client/Modules/Retail_Voice/Directives/MainExtensions/VoiceChargingPolicyEvaluators/Templates/StandardPolicyEvaluatorTemplate.html'
        };

        function VoiceServiceTypeCtor($scope, ctrl) {
            this.initializeController = initializeController;
            
            var tariffRuleSelectorAPI;
            var tariffRuleSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var rateTypeRuleSelectorAPI;
            var rateTypeRuleSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var rateValueRuleSelectorAPI;
            var rateValueRuleSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            var extraChargeRuleSelectorAPI;
            var extraChargeRuleSelectorReadyDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onTariffRuleSelectorReady = function (api) {
                    tariffRuleSelectorAPI = api;
                    tariffRuleSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onRateTypeRuleSelectorReady = function (api) {
                    rateTypeRuleSelectorAPI = api;
                    rateTypeRuleSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onRateValueRuleSelectorReady = function (api) {
                    rateValueRuleSelectorAPI = api;
                    rateValueRuleSelectorReadyDeferred.resolve();
                };
                $scope.scopeModel.onExtraChargeRuleSelectorReady = function (api) {
                    extraChargeRuleSelectorAPI = api;
                    extraChargeRuleSelectorReadyDeferred.resolve();
                };

                defineAPI();
            }
            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    var standardPolicyEvaluator;

                    if (payload != undefined) {
                        standardPolicyEvaluator = payload.voiceChargingPolicyEvaluator;
                    }

                    //Loading TariffRule Seletor
                    var tariffRuleSelectorLoadPromise = getTariffRuleSelectorLoadPromise();
                    promises.push(tariffRuleSelectorLoadPromise);

                    //Loading RateTypeRule Seletor
                    var rateTypeRuleSelectorLoadPromise = getRateTypeRuleSelectorLoadPromise();
                    promises.push(rateTypeRuleSelectorLoadPromise);

                    //Loading RateTypeRule Seletor
                    var rateValueRuleSelectorLoadPromise = getRateValueRuleSelectorLoadPromise();
                    promises.push(rateValueRuleSelectorLoadPromise);

                    //Loading ExtraChargeRule Seletor
                    var extraChargeRuleSelectorLoadPromise = getExtraChargeRuleSelectorLoadPromise();
                    promises.push(extraChargeRuleSelectorLoadPromise);


                    function getTariffRuleSelectorLoadPromise() {
                        var tariffRuleSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        tariffRuleSelectorReadyDeferred.promise.then(function () {

                            var tariffRuleSelectorPayload = {
                                showaddbutton: false,
                                specificTypeName: Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum.TariffRule.name,
                                filter: {
                                    RuleTypeId: Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum.TariffRule.id
                                }
                            };

                            if (standardPolicyEvaluator != undefined) {
                                tariffRuleSelectorPayload.selectedIds = standardPolicyEvaluator.TariffRuleDefinitionId;
                            }

                            VRUIUtilsService.callDirectiveLoad(tariffRuleSelectorAPI, tariffRuleSelectorPayload, tariffRuleSelectorLoadDeferred);
                        });

                        return tariffRuleSelectorLoadDeferred.promise
                    }
                    function getRateTypeRuleSelectorLoadPromise() {
                        var rateTypeRuleSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        rateTypeRuleSelectorReadyDeferred.promise.then(function () {

                            var rateTypeRuleSelectorPayload = {
                                showaddbutton: false,
                                specificTypeName: Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum.RateTypeRule.name,
                                filter: {
                                    RuleTypeId: Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum.RateTypeRule.id
                                }
                            };

                            if (standardPolicyEvaluator != undefined) {
                                rateTypeRuleSelectorPayload.selectedIds = standardPolicyEvaluator.RateTypeRuleDefinitionId;
                            }

                            VRUIUtilsService.callDirectiveLoad(rateTypeRuleSelectorAPI, rateTypeRuleSelectorPayload, rateTypeRuleSelectorLoadDeferred);
                        });

                        return rateTypeRuleSelectorLoadDeferred.promise
                    }
                    function getRateValueRuleSelectorLoadPromise() {
                        var rateValueRuleSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        rateValueRuleSelectorReadyDeferred.promise.then(function () {

                            var rateValueRuleSelectorPayload = {
                                showaddbutton: false,
                                specificTypeName: Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum.RateValueRule.name,
                                filter: {
                                    RuleTypeId: Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum.RateValueRule.id
                                }
                            };

                            if (standardPolicyEvaluator != undefined) {
                                rateValueRuleSelectorPayload.selectedIds = standardPolicyEvaluator.RateValueRuleDefinitionId;
                            }

                            VRUIUtilsService.callDirectiveLoad(rateValueRuleSelectorAPI, rateValueRuleSelectorPayload, rateValueRuleSelectorLoadDeferred);
                        });

                        return rateValueRuleSelectorLoadDeferred.promise
                    }
                    function getExtraChargeRuleSelectorLoadPromise() {
                        var extraChargeRuleSelectorLoadDeferred = UtilsService.createPromiseDeferred();

                        extraChargeRuleSelectorReadyDeferred.promise.then(function () {

                            var extraChargeRuleSelectorPayload = {
                                showaddbutton: false,
                                specificTypeName: Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum.ExtraChargeRule.name,
                                filter: {
                                    RuleTypeId: Retail_Voice_ChargingPolicyRuleDefinitionTypesEnum.ExtraChargeRule.id
                                }
                            };

                            if (standardPolicyEvaluator != undefined) {
                                extraChargeRuleSelectorPayload.selectedIds = standardPolicyEvaluator.ExtraChargeRuleDefinitionId;
                            }

                            VRUIUtilsService.callDirectiveLoad(extraChargeRuleSelectorAPI, extraChargeRuleSelectorPayload, extraChargeRuleSelectorLoadDeferred);
                        });

                        return extraChargeRuleSelectorLoadDeferred.promise
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {

                    var tariffRuleDefinitionId = $scope.scopeModel.selectedTariffRule ? $scope.scopeModel.selectedTariffRule.GenericRuleDefinitionId : undefined;
                    var rateTypeRuleDefinitionId = $scope.scopeModel.selectedRateTypeRule ? $scope.scopeModel.selectedRateTypeRule.GenericRuleDefinitionId : undefined;
                    var rateValueRuleDefinitionId = $scope.scopeModel.selectedRateValueRule ? $scope.scopeModel.selectedRateValueRule.GenericRuleDefinitionId : undefined;
                    var extraChargeRuleDefinitionId = $scope.scopeModel.selectedExtraChargeRule ? $scope.scopeModel.selectedExtraChargeRule.GenericRuleDefinitionId : undefined;

                    var obj = {
                        $type: "Retail.Voice.MainExtensions.VoiceChargingPolicyEvaluators.StandardPolicyEvaluator, Retail.Voice.MainExtensions",
                        TariffRuleDefinitionId: tariffRuleDefinitionId,
                        RateTypeRuleDefinitionId: rateTypeRuleDefinitionId,
                        RateValueRuleDefinitionId: rateValueRuleDefinitionId,
                        ExtraChargeRuleDefinitionId: extraChargeRuleDefinitionId
                    };
                    return obj;
                };

                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }
        }
    }

    app.directive('retailVoiceStandardpolicyevaluator', VoiceServiceTypeDirective);

})(app);