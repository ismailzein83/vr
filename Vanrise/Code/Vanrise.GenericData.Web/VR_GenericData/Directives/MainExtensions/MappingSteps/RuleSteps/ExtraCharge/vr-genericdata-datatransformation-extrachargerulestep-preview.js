﻿'use strict';
app.directive('vrGenericdataDatatransformationExtrachargerulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new ExtraChargeRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/ExtraCharge/Templates/ExtraChargeRuleStepPreviewTemplate.html';
            }

        };

        function ExtraChargeRuleStepCtor(ctrl, $scope) {
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
                            stepObj.initialRate = payload.stepDetails.InitialRate;
                            stepObj.effectiveRate = payload.stepDetails.EffectiveRate;

                            ctrl.initialRate = payload.stepDetails.InitialRate;
                            ctrl.effectiveRate = payload.stepDetails.EffectiveRate;
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

                    stepObj.initialRate = changes.InitialRate;
                    stepObj.effectiveRate = changes.EffectiveRate;

                    ctrl.initialRate = changes.InitialRate;
                    ctrl.effectiveRate = changes.EffectiveRate;
                }

                api.checkValidation = function () {
                    var validate = commonDirectiveAPI.checkValidation();
                    if (validate === null)
                        validate = checkValidation();
                    return validate;
                }

                api.getData = function () {
                    var stepDetails = commonDirectiveAPI !=undefined? commonDirectiveAPI.getData():undefined;
                    if (stepDetails != undefined) {
                        stepDetails.InitialRate = stepObj.initialRate;
                        stepDetails.EffectiveRate = stepObj.effectiveRate;
                    }
                    return stepDetails;
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
               
                if (ctrl.initialRate == undefined) {
                    return "Missing initial rate mapping.";
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