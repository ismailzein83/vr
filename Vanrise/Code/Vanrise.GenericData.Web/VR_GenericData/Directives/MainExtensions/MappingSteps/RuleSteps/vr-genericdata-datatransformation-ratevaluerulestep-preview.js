'use strict';
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/RuleSteps/Templates/RateValueRuleStepPreviewTemplate.html';
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
                            stepObj.configId = payload.stepDetails.ConfigId;
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

                }

                api.getData = function () {
                    stepObj.stepDetails.ConfigId = stepObj.configId;
                    return stepObj.stepDetails
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);