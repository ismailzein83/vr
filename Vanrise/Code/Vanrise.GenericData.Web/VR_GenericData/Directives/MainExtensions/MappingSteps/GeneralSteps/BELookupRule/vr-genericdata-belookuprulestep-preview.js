'use strict';
app.directive('vrGenericdataBelookuprulestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new BELookupRuleStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/BELookupRule/Templates/BELookupRuleStepPreviewTemplate.html';
            }

        };

        function BELookupRuleStepCtor(ctrl, $scope) {
            var stepObj = {};

            function initializeController() {
                ctrl.criteriaFieldsMappings = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;
                            if (payload.stepDetails.CriteriaFieldsMappings != undefined && payload.stepDetails.CriteriaFieldsMappings.length > 0) {
                                ctrl.recordsMapping = payload.stepDetails.CriteriaFieldsMappings;
                            }
                            ctrl.businessEntity = payload.stepDetails.BusinessEntity;
                        }
                        checkValidation();
                    }

                };

                api.applyChanges = function (changes) {
                    ctrl.criteriaFieldsMappings = changes.CriteriaFieldsMappings;
                    ctrl.businessEntity = changes.BusinessEntity;
                    stepObj.stepDetails = changes;
                };

                api.checkValidation = function () {
                    return checkValidation();
                };

                api.getData = function () {
                    return stepObj.stepDetails;
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {
                if (ctrl.criteriaFieldsMappings != undefined) {
                    for (var i = 0 ; i < ctrl.criteriaFieldsMappings.length; i++) {
                        if (ctrl.criteriaFieldsMappings[i].Value == undefined)
                            return "All fields should be mapped.";
                    }
                } else {
                    return "All fields should be mapped.";
                }
                if (ctrl.businessEntity == undefined) {
                    return "Missing business entity mapping.";
                }
                return null;
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);