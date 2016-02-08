﻿'use strict';
app.directive('vrGenericdataDatatransformationRatevaluerulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/RateValue/Templates/RateValueRuleStepPreviewTemplate.html';
            }

        };

        function rateValueRuleStepCtor(ctrl, $scope) {
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
                            ctrl.normalRate = payload.stepDetails.NormalRate;
                            ctrl.ratesByRateType = payload.stepDetails.RatesByRateType;
                        }

                    }

                }

                api.applyChanges = function (changes) {
                    stepObj.stepDetails = changes;
                    ctrl.normalRate = changes.NormalRate;
                    ctrl.ratesByRateType = changes.RatesByRateType;
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
                if (ctrl.normalRate == undefined) {
                    return "Missing normal rate mapping.";
                }
                if (ctrl.ratesByRateType == undefined) {
                    return "Missing rates by rate type mapping.";
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