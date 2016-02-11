'use strict';
app.directive('vrGenericdataDatatransformationRatetyperulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new rateTypeRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/RateType/Templates/RateTypeRuleStepPreviewTemplate.html';
            }

        };

        function rateTypeRuleStepCtor(ctrl, $scope) {
            var stepObj = {};

            var commonDirectiveAPI;
            var commonDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.onCommonDirectiveReady = function (api) {
                    commonDirectiveAPI = api;
                    commonDirectiveReadyPromiseDeferred.resolve();
                }
                ctrl.ruleFieldsMappings = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.ruleFieldsMappings.length = 0;
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.normalRate = payload.stepDetails.NormalRate;
                            stepObj.ratesByRateType = payload.stepDetails.RatesByRateType;
                            stepObj.effectiveRate = payload.stepDetails.EffectiveRate;
                            stepObj.rateTypeId = payload.stepDetails.RateTypeId;

                            ctrl.normalRate = payload.stepDetails.NormalRate;
                            ctrl.ratesByRateType = payload.stepDetails.RatesByRateType;
                            ctrl.effectiveRate = payload.stepDetails.EffectiveRate;
                            ctrl.rateTypeId = payload.stepDetails.RateTypeId;
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

                }

                api.applyChanges = function (changes) {
                    if (commonDirectiveAPI != undefined)
                        commonDirectiveAPI.applyChanges(changes);

                    stepObj.normalRate = changes.NormalRate;
                    stepObj.ratesByRateType = changes.RatesByRateType;
                    stepObj.effectiveRate = changes.EffectiveRate;
                    stepObj.rateTypeId = changes.RateTypeId;

                    ctrl.normalRate = changes.NormalRate;
                    ctrl.ratesByRateType = changes.RatesByRateType;
                    ctrl.effectiveRate = changes.EffectiveRate;
                    ctrl.rateTypeId = changes.RateTypeId;
                }

                api.checkValidation = function () {
                    var validate;
                    if (commonDirectiveAPI != undefined)
                        validate = commonDirectiveAPI.checkValidation();
                    if (validate == undefined)
                        validate = checkValidation();
                    return validate;
                }

                api.getData = function () {
                    var stepDetails = commonDirectiveAPI.getData();
                    if (stepDetails != undefined) {
                        stepDetails.NormalRate = stepObj.normalRate;
                        stepDetails.RatesByRateType = stepObj.ratesByRateType;
                        stepDetails.EffectiveRate = stepObj.effectiveRate;
                        stepDetails.RateTypeId = stepObj.rateTypeId;
                    }
                    return stepDetails;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
               
                if (ctrl.normalRate == undefined) {
                    return "Missing normal rate mapping.";
                }
                if (ctrl.ratesByRateType == undefined) {
                    return "Missing rates by rate type mapping.";
                }
                if (ctrl.effectiveRate == undefined) {
                    return "Missing effective rate mapping.";
                }
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);