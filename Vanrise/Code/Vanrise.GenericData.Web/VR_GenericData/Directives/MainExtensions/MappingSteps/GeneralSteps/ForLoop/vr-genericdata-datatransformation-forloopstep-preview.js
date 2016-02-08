'use strict';
app.directive('vrGenericdataDatatransformationforloopstepPreview', ['UtilsService', 'VRUIUtilsService',
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
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.isCompositeStep = true;
                api.addStep = function (stepObj) {
                    stepObj.Context = getChildrenContext();
                    ctrl.childSteps.push(stepObj);
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
                        }
                       checkValidation();
                    }
                 
                }

                api.applyChanges = function (changes) {
                    ctrl.target = changes.Target;
                    ctrl.source = changes.Source;
                    stepObj.stepDetails = changes;
                }

                api.checkValidation = function ()
                {
                  return  checkValidation();
                }

                api.getData = function () {
                    stepObj.stepDetails.ConfigId = stepObj.configId;
                    return stepObj.stepDetails      
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

                    },
                    getRecordFields: function (typeName) {

                    }
                };
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);