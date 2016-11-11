'use strict';
app.directive('vrGenericdataDatatransformationTariffrulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new TariffRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Tariff/Templates/TariffRuleStepPreviewTemplate.html';
            }

        };

        function TariffRuleStepCtor(ctrl, $scope) {
            var stepObj = {};

            var commonDirectiveAPI;
            var commonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.onCommonDirectiveReady = function (api) {
                    commonDirectiveAPI = api;
                    commonDirectiveReadyPromiseDeferred.resolve();
                };
                ctrl.ruleFieldsMappings = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.ruleFieldsMappings.length = 0;
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.initialRate = payload.stepDetails.InitialRate;
                            stepObj.durationInSeconds = payload.stepDetails.DurationInSeconds;
                            stepObj.effectiveRate = payload.stepDetails.EffectiveRate;
                            stepObj.effectiveDurationInSeconds = payload.stepDetails.EffectiveDurationInSeconds;
                            stepObj.totalAmount = payload.stepDetails.TotalAmount;
                            stepObj.extraChargeRate = payload.stepDetails.ExtraChargeRate;
                            stepObj.extraChargeValue = payload.stepDetails.ExtraChargeValue;
                            stepObj.currencyId = payload.stepDetails.CurrencyId;

                            ctrl.initialRate = payload.stepDetails.InitialRate;
                            ctrl.durationInSeconds = payload.stepDetails.DurationInSeconds;
                            ctrl.effectiveRate = payload.stepDetails.EffectiveRate;
                            ctrl.effectiveDurationInSeconds = payload.stepDetails.EffectiveDurationInSeconds;
                            ctrl.totalAmount = payload.stepDetails.TotalAmount;
                            ctrl.extraChargeRate = payload.stepDetails.ExtraChargeRate;
                            ctrl.extraChargeValue = payload.stepDetails.ExtraChargeValue;
                            ctrl.currencyId = payload.stepDetails.CurrencyId;
                        }

                    }
                    var promises = [];
                    var loadCommonDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                    commonDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadCommon;
                        if (payload != undefined) {
                            payloadCommon = {};
                            if (payload != undefined)
                                payloadCommon.stepDetails = payload.stepDetails;
                        }
                        VRUIUtilsService.callDirectiveLoad(commonDirectiveAPI, payloadCommon, loadCommonDirectivePromiseDeferred);
                    });

                    promises.push(loadCommonDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.applyChanges = function (changes) {
                    if (commonDirectiveAPI != undefined)
                        commonDirectiveAPI.applyChanges(changes);

                    stepObj.initialRate = changes.InitialRate;
                    stepObj.durationInSeconds = changes.DurationInSeconds;
                    stepObj.effectiveRate = changes.EffectiveRate;
                    stepObj.effectiveDurationInSeconds = changes.EffectiveDurationInSeconds;
                    stepObj.totalAmount = changes.TotalAmount;
                    stepObj.extraChargeRate = changes.ExtraChargeRate;
                    stepObj.extraChargeValue = changes.ExtraChargeValue;
                    stepObj.currencyId = changes.CurrencyId;

                    ctrl.initialRate = changes.InitialRate;
                    ctrl.durationInSeconds = changes.DurationInSeconds;
                    ctrl.effectiveRate = changes.EffectiveRate;
                    ctrl.effectiveDurationInSeconds = changes.EffectiveDurationInSeconds;
                    ctrl.totalAmount = changes.TotalAmount;
                    ctrl.extraChargeRate = changes.ExtraChargeRate;
                    ctrl.extraChargeValue = changes.ExtraChargeValue;
                    ctrl.currencyId = changes.CurrencyId;
                };

                api.checkValidation = function () {
                    var validate;
                    if (commonDirectiveAPI != undefined)
                        validate = commonDirectiveAPI.checkValidation();
                    if (validate == undefined)
                        validate = checkValidation();
                    return validate;
                };

                api.getData = function () {
                    var stepDetails = commonDirectiveAPI.getData();
                    if (stepDetails != undefined) {
                        stepDetails.InitialRate = stepObj.initialRate;
                        stepDetails.DurationInSeconds = stepObj.durationInSeconds;
                        stepDetails.EffectiveRate = stepObj.effectiveRate;
                        stepDetails.EffectiveDurationInSeconds = stepObj.effectiveDurationInSeconds;
                        stepDetails.TotalAmount = stepObj.totalAmount;
                        stepDetails.ExtraChargeRate = stepObj.extraChargeRate;
                        stepDetails.ExtraChargeValue = stepObj.extraChargeValue;
                        stepDetails.CurrencyId = stepObj.currencyId;
                    }
                    return stepDetails;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
               
                if (ctrl.initialRate == undefined) {
                    return "Missing initial rate mapping.";
                }
                //if (ctrl.effectiveRate == undefined) {
                //    return "Missing effective rate mapping.";
                //}
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);