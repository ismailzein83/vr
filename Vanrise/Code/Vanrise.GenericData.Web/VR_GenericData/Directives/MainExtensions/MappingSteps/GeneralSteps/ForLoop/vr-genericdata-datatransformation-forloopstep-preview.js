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

            var sequenceDirectiveAPI;
            var sequenceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {

                $scope.onSequenceDirectiveReady = function (api) {
                    sequenceDirectiveAPI = api;
                    sequenceReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        currentContext = payload.context;
                        if (payload.stepDetails != undefined) {
                            stepObj.stepDetails = payload.stepDetails;
                            stepObj.configId = payload.stepDetails.ConfigId;
                            currentContext = payload.context;
                            ctrl.arrayVariableName = payload.stepDetails.ArrayVariableName;
                            ctrl.iterationVariableName = payload.stepDetails.IterationVariableName;

                        }
                        checkValidation();
                        var loadSequencePromiseDeferred = UtilsService.createPromiseDeferred();

                        sequenceReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = { context: getChildrenContext() };
                                if (payload.stepDetails != undefined && payload.stepDetails.Steps != null)
                                    directivePayload.MappingSteps = payload.stepDetails.Steps;

                                VRUIUtilsService.callDirectiveLoad(sequenceDirectiveAPI, directivePayload, loadSequencePromiseDeferred);
                            });

                        return loadSequencePromiseDeferred.promise;
                    }

                };

                api.applyChanges = function (changes) {
                    stepObj.stepDetails = changes;
                    ctrl.arrayVariableName = changes.ArrayVariableName;
                    ctrl.iterationVariableName = changes.IterationVariableName;
                };

                api.checkValidation = function () {
                    return checkValidation();
                };

                api.getData = function () {
                    var forLoobStepDetails = stepObj.stepDetails;
                    if (forLoobStepDetails != undefined)
                        forLoobStepDetails.Steps = sequenceDirectiveAPI != undefined ? sequenceDirectiveAPI.getData() : undefined;
                    return forLoobStepDetails;
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {                
                if (ctrl.arrayVariableName == undefined) {
                    return "Missing array variable name.";
                }
                if (ctrl.iterationVariableName == undefined) {
                    return "Missing variable name.";
                }
                return null;
            }

            function getChildrenContext()
            {
                var childrenContext = UtilsService.cloneObject(currentContext, false);
                if(childrenContext == undefined)
                    childrenContext = {};
                childrenContext.getRecordNames= function () {
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
                };
                childrenContext.getRecordFields= function (recordName) {

                    if (recordName == ctrl.iterationVariableName)
                    {
                        if (ctrl.iterationVariableName != undefined)
                            return currentContext.getRecordFields(ctrl.arrayVariableName);
                    }
                    else
                    {
                        return currentContext.getRecordFields(recordName);

                    }
                        
                };
                return childrenContext; 
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);