'use strict';
app.directive('vrGenericdataDatatransformationAssignfieldstep', ['UtilsService','VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new AssignFieldStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/AssignField/Templates/AssignFieldStepTemplate.html';
            }

        };

        function AssignFieldStepCtor(ctrl, $scope) {
            var stepPayload;
            var sourceDirectiveReadyAPI;
            var sourceDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var targetDirectiveReadyAPI;
            var targetDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                $scope.onSourceReady = function (api) {
                    sourceDirectiveReadyAPI = api;
                    sourceDirectiveReadyPromiseDeferred.resolve();
                };

                $scope.onTargetReady = function (api) {
                    targetDirectiveReadyAPI = api;
                    targetDirectiveReadyPromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    stepPayload = payload;
                    var promises = [];

                    var loadTargetDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    targetDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadTarget = { context: payload.context };
                        if (payload.stepDetails != undefined)
                            payloadTarget.selectedRecords = payload.stepDetails.Target;
                        VRUIUtilsService.callDirectiveLoad(targetDirectiveReadyAPI, payloadTarget, loadTargetDirectivePromiseDeferred);
                    });
                    promises.push(loadTargetDirectivePromiseDeferred.promise);

                    var loadSourceDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    sourceDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadSource = { context: payload.context };
                        if (payload.stepDetails != undefined)
                            payloadSource.selectedRecords = payload.stepDetails.Source;
                        VRUIUtilsService.callDirectiveLoad(sourceDirectiveReadyAPI, payloadSource, loadSourceDirectivePromiseDeferred);
                    });
                    promises.push(loadSourceDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);

                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions",
                        Source: sourceDirectiveReadyAPI.getData(),
                        Target: targetDirectiveReadyAPI.getData(),

                    };
                };

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);