﻿'use strict';
app.directive('vrGenericdataDatatransformationRatevaluerulestep', ['UtilsService', 'VR_GenericData_GenericRuleTypeConfigAPIService', 'VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new rateValueRuleStepCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                }
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/RateValue/Templates/RateValueRuleStepTemplate.html';
            }

        };

        function rateValueRuleStepCtor(ctrl, $scope) {
            var ruleTypeName = "VR_RateValueRule";
            var ruleTypeEntity;

            var ruleStepCommonDirectiveAPI;
            var ruleStepCommonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var normalRateDirectiveAPI;
            var normalRateDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var ratesByRateTypeDirectiveAPI;
            var ratesByRateTypeDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                $scope.onRuleStepCommonReady = function (api) {
                    ruleStepCommonDirectiveAPI = api;
                    ruleStepCommonDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onNormalRateDirectiveReady = function (api) {
                    normalRateDirectiveAPI = api;
                    normalRateDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onRatesByRateTypeDirectiveReady = function (api) {
                    ratesByRateTypeDirectiveAPI = api;
                    ratesByRateTypeDirectiveReadyPromiseDeferred.resolve();
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];
                    var loadRuleStepCommonDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    ruleStepCommonDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRuleStep = { ruleTypeName: ruleTypeName };
                        if (payload != undefined && payload.Context != undefined)
                            payloadRuleStep.context = payload.Context;
                        if (payload != undefined && payload.stepDetails) {
                            payloadRuleStep.ruleFieldsMappings = payload.stepDetails.RuleFieldsMappings;
                            payloadRuleStep.effectiveTime = payload.stepDetails.EffectiveTime;
                            payloadRuleStep.ruleDefinitionId = payload.stepDetails.RuleDefinitionId;
                        }
                        VRUIUtilsService.callDirectiveLoad(ruleStepCommonDirectiveAPI, payloadRuleStep, loadRuleStepCommonDirectivePromiseDeferred);
                    });

                    promises.push(loadRuleStepCommonDirectivePromiseDeferred.promise);

                    var loadNormalRateDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    normalRateDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadNormalRate;
                        if (payload != undefined) {
                            payloadNormalRate = {};
                            if (payload != undefined && payload.Context != undefined)
                                payloadNormalRate.context = payload.Context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadNormalRate.selectedRecords = payload.stepDetails.NormalRate;
                        }
                        VRUIUtilsService.callDirectiveLoad(normalRateDirectiveAPI, payloadNormalRate, loadNormalRateDirectivePromiseDeferred);
                    });

                    promises.push(loadNormalRateDirectivePromiseDeferred.promise);

                    var loadRatesByRateTypeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    ratesByRateTypeDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRatesByRateType;
                        if (payload != undefined) {
                            payloadRatesByRateType = {};
                            if (payload != undefined && payload.Context != undefined)
                                payloadRatesByRateType.context = payload.Context;
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadRatesByRateType.selectedRecords = payload.stepDetails.RatesByRateType;
                        }
                        VRUIUtilsService.callDirectiveLoad(ratesByRateTypeDirectiveAPI, payloadRatesByRateType, loadRatesByRateTypeDirectivePromiseDeferred);
                    });

                    promises.push(loadRatesByRateTypeDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                }

                api.getData = function () {
                    var obj = {
                        $type: "Vanrise.GenericData.Pricing.RateValueMappingStep, Vanrise.GenericData.Pricing",
                        NormalRate: normalRateDirectiveAPI != undefined ? normalRateDirectiveAPI.getData() : undefined,
                        RatesByRateType: ratesByRateTypeDirectiveAPI != undefined ? ratesByRateTypeDirectiveAPI.getData() : undefined,
                    }
                    ruleStepCommonDirectiveAPI.setData(obj);
                    return obj;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);