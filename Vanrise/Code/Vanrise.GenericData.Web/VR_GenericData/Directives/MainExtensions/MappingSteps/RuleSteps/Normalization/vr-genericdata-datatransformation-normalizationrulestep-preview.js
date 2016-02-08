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

            function initializeController() {
                ctrl.ruleFieldsMappings = [];
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    ctrl.ruleFieldsMappings.length = 0;
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;
                            ctrl.value = payload.stepDetails.Value;
                            ctrl.normalizedValue = payload.stepDetails.NormalizedValue;
                        }

                    }

                }

                api.applyChanges = function (changes) {
                    stepObj.stepDetails = changes;
                    ctrl.value = changes.Value;
                    ctrl.normalizedValue = changes.NormalizedValue;
                }

                api.checkValidation = function () {
                    return checkValidation();
                }

                api.getData = function () {
                    return stepObj.stepDetails
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            function checkValidation() {
                if (ctrl.ruleFieldsMappings != undefined) {
                    for (var i = 0 ; i < ctrl.ruleFieldsMappings.length; i++) {
                        if (ctrl.ruleFieldsMappings[i].Value == undefined)
                            return "All fields should be mapped.";
                    }
                } else {
                    return "All fields should be mapped.";
                }

                if (ctrl.value == undefined) {
                    return "Missing value mapping.";
                }
                if (ctrl.normalizedValue == undefined) {
                    return "Missing normalized value mapping.";
                }
                if (stepObj.stepDetails.EffectiveTime == undefined)
                    return "Missing effective time mapping.";

                return null;
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);