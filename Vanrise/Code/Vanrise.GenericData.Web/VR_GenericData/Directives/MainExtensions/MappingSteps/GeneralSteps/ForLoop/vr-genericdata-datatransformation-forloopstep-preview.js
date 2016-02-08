'use strict';
app.directive('vrGenericdataDatatransformationForloopstepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new ForLoopStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/ForLoop/Templates/ForLoopStepPreviewTemplate.html';
            }

        };

        function ForLoopStepCtor(ctrl, $scope) {
            var stepObj = {};
            ctrl.childSteps = [];
            var currentContext;

            function initializeController() {
                $scope.editStep = function (stepItem) {
                    currentContext.editStep(stepItem);
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.isCompositeStep = true;
                api.addStep = function (stepDefinition) {
                    var stepItem = currentContext.createStepItem(stepDefinition, null, getChildrenContext())
                    ctrl.childSteps.push(stepItem);
                };

                api.load = function (payload) {
                    if(payload != undefined)
                    {
                        currentContext = payload.Context;
                        if (payload.stepDetails != undefined)
                        {
                            stepObj.stepDetails = payload.stepDetails;
                            stepObj.configId = payload.stepDetails.ConfigId;
                            currentContext = payload.Context;
                            ctrl.arrayVariableName = payload.stepDetails.ArrayVariableName;
                            ctrl.iterationVariableName = payload.stepDetails.IterationVariableName;
                            if (payload.stepDetails.Steps != null) {
                                for (var i = 0; i < payload.stepDetails.Steps.length; i++) {
                                    var stepEntity = payload.stepDetails.Steps[i];
                                    var stepItem = currentContext.createStepItem(null, stepEntity, getChildrenContext())
                                    ctrl.childSteps.push(stepItem);
                                }
                            }
                        }
                       checkValidation();
                    }
                 
                }

                api.applyChanges = function (changes) {                    
                    stepObj.stepDetails = changes;
                    ctrl.iterationVariableName = changes.IterationVariableName;
                }

                api.checkValidation = function ()
                {
                  return  checkValidation();
                }

                api.getData = function () {
                    console.log(stepObj.stepDetails);
               //     stepObj.stepDetails.ConfigId = stepObj.configId;
                    return stepObj.stepDetails;
                }
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {                
                return null;
            }

            function getChildrenContext()
            {
                var childrenContext = {
                    getRecordNames: function () {
                        var recordNames = [];
                        if(ctrl.iterationVariableName != undefined)
                            recordNames.push({ Name: ctrl.iterationVariableName });
                        var parentRecords = currentContext.getRecordNames();
                        if (parentRecords != null) {
                            for (var i = 0; i < parentRecords.length; i++) {
                                recordNames.push(parentRecords[i]);
                            }
                        }
                        return recordNames;
                    },
                    getRecordFields: function (recordName) {

                    },
                    createStepItem: currentContext.createStepItem,
                    editStep: currentContext.editStep
                };
                return childrenContext; 
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);