'use strict';
app.directive('vrGenericdataDatatransformationdefinitionStepsequencedesign', ['UtilsService', '$compile', 'VRUIUtilsService', function (UtilsService, $compile, VRUIUtilsService) {

    var directiveDefinitionObject = {
        restrict: 'E',
        scope: {
            onReady: '=',
        },
        controller: function ($scope, $element, $attrs) {
            var ctrl = this;
            ctrl.childSteps = [];

            var ctor = new sequenceStepCtor(ctrl, $scope, $attrs);
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
        templateUrl: "/Client/Modules/VR_GenericData/Directives/DataTransformationDefinition/Templates/StepSequenceDesignTemplate.html"

    };
  

    function sequenceStepCtor(ctrl, $scope, $attrs) {
        var currentContext;
        var api = {};

        function initializeController() {

            $scope.editStep = function (stepItem) {
                if (currentContext != undefined)
                   currentContext.editStep(stepItem);
            };

            $scope.onCheckedChanged = function () {

                if (currentContext != undefined) {
                    if (ctrl.checkedSequence)
                        currentContext.setSelectedComposite(api);
                    else
                        currentContext.setSelectedComposite();
                }


            };

            $scope.onRemoveStep = function (dataItem) {
                var index = ctrl.childSteps.indexOf(dataItem);
                if (index != -1) {
                    ctrl.childSteps.splice(index, 1);
                    currentContext.removeStep(dataItem);
                }

            };

            defineAPI();
        }

        function defineAPI() {
          

            api.load = function (payload) {

                var promises = [];
                if (payload != undefined) {
                    currentContext = payload.context;
                    if (currentContext != undefined && payload.MappingSteps != undefined) {

                        for (var i = 0; i < payload.MappingSteps.length; i++) {
                            var stepEntity = payload.MappingSteps[i];
                            var stepItem = currentContext.createStepItem(null, stepEntity, getChildrenContext());
                            promises.push(stepItem.loadPromiseDeferred.promise);
                            ctrl.childSteps.push(stepItem);
                        }

                    }
                }



                return UtilsService.waitMultiplePromises(promises);
            };

            api.addStep = function (stepDefinition) {
                var stepItem = currentContext.createStepItem(stepDefinition, null, getChildrenContext());
                currentContext.setSelectedStep(stepItem);
                ctrl.childSteps.push(stepItem);
            };

            api.setCheckedSequence = function (value) {
                ctrl.checkedSequence = value;
            };

            api.getData = function () {
                return buildStepsData();
            };

            if (ctrl.onReady != null)
                ctrl.onReady(api);
        }

        function getChildrenContext()
        {
            var childContext = UtilsService.cloneObject(currentContext, false);
            return childContext;
        }

        function buildStepsData() {
            var steps = [];

            for (var i = 0; i < ctrl.childSteps.length; i++) {
                var stepItem = ctrl.childSteps[i];
                if (stepItem.previewAPI != undefined) {
                    var stepEntity = stepItem.previewAPI.getData();
                    if (stepEntity != undefined)
                    {
                        stepEntity.ConfigId = stepItem.dataTransformationStepConfigId;
                        steps.push(stepEntity);
                    }
                        
                }
            }
            return steps;
        }

        this.initializeController = initializeController;

    }
    return directiveDefinitionObject;
}]);

