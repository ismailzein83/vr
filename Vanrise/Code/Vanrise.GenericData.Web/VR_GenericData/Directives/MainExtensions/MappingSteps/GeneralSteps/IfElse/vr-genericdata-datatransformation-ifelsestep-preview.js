'use strict';
app.directive('vrGenericdataDatatransformationIfelsestepPreview', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new IfElseStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/IfElse/Templates/IfElseStepPreviewTemplate.html';
            }

        };

        function IfElseStepCtor(ctrl, $scope) {
            var stepObj = {};
            ctrl.childSteps = [];
            var currentContext;

            var thenSequenceDirectiveAPI;
            var thenSequenceReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var elseSequenceDirectiveAPI;
            var elseSequenceReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {

                $scope.onThenSequenceDirectiveReady = function (api) {
                    thenSequenceDirectiveAPI = api;
                    thenSequenceReadyPromiseDeferred.resolve();
                };

                $scope.onElseSequenceDirectiveReady = function (api) {
                    elseSequenceDirectiveAPI = api;
                    elseSequenceReadyPromiseDeferred.resolve();
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
                            currentContext = payload.context;
                            ctrl.condition = payload.stepDetails.Condition;
                        }
                        checkValidation();

                        var promises = [];
                        var loadThenSequencePromiseDeferred = UtilsService.createPromiseDeferred();

                        thenSequenceReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = { context: getChildrenContext() };
                                if (payload.stepDetails != undefined && payload.stepDetails.ThenSteps != null)
                                    directivePayload.MappingSteps = payload.stepDetails.ThenSteps;

                                VRUIUtilsService.callDirectiveLoad(thenSequenceDirectiveAPI, directivePayload, loadThenSequencePromiseDeferred);
                            });
                        promises.push(loadThenSequencePromiseDeferred.promise);

                        var loadElseSequencePromiseDeferred = UtilsService.createPromiseDeferred();

                        elseSequenceReadyPromiseDeferred.promise
                            .then(function () {
                                var directivePayload = { context: getChildrenContext() };
                                if (payload.stepDetails != undefined && payload.stepDetails.ElseSteps != null)
                                    directivePayload.MappingSteps = payload.stepDetails.ElseSteps;

                                VRUIUtilsService.callDirectiveLoad(elseSequenceDirectiveAPI, directivePayload, loadElseSequencePromiseDeferred);
                            });
                        promises.push(loadElseSequencePromiseDeferred.promise);

                        return UtilsService.waitMultiplePromises(promises);
                    }

                };

                api.applyChanges = function (changes) {
                    stepObj.stepDetails = changes;
                    ctrl.condition = changes.Condition;
                };

                api.checkValidation = function () {
                    return checkValidation();
                };

                api.getData = function () {
                    var forLoobStepDetails = stepObj.stepDetails;
                    if (forLoobStepDetails != undefined) {
                        forLoobStepDetails.ThenSteps = thenSequenceDirectiveAPI != undefined ? thenSequenceDirectiveAPI.getData() : undefined;
                        forLoobStepDetails.ElseSteps = elseSequenceDirectiveAPI != undefined ? elseSequenceDirectiveAPI.getData() : undefined;
                    }
                    return forLoobStepDetails;
                };
                
                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            function checkValidation() {                
                if (ctrl.condition == undefined) {
                    return "Missing condition.";
                }
                return null;
            }

            function getChildrenContext()
            {
                var childrenContext = UtilsService.cloneObject(currentContext, false);
               
                return childrenContext; 
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);