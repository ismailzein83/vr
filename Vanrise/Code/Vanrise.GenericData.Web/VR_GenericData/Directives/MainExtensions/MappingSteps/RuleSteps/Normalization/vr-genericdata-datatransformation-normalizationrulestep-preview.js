'use strict';
app.directive('vrGenericdataDatatransformationNormalizationrulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new normalizationruleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Normalization/Templates/NormalizationRuleStepPreviewTemplate.html';
            }

        };

        function normalizationruleStepCtor(ctrl, $scope) {
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
                            stepObj.value = payload.stepDetails.Value;
                            stepObj.normalizedValue = payload.stepDetails.NormalizedValue;
                            ctrl.value = payload.stepDetails.Value;
                            ctrl.normalizedValue = payload.stepDetails.NormalizedValue;
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
                    stepObj.value = changes.Value;
                    stepObj.normalizedValue = changes.NormalizedValue;
                    ctrl.value = changes.Value;
                    ctrl.normalizedValue = changes.NormalizedValue;
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
                        stepDetails.Value = stepObj.value;
                        stepDetails.NormalizedValue = stepObj.normalizedValue;
                    }
                    return stepDetails;
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.value == undefined) {
                    return "Missing value mapping.";
                }
                if (ctrl.normalizedValue == undefined) {
                    return "Missing normalized value mapping.";
                }
               
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);