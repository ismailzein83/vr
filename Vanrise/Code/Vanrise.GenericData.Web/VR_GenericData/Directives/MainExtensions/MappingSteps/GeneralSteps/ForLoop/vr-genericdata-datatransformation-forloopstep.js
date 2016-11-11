'use strict';
app.directive('vrGenericdataDatatransformationForloopstep', ['UtilsService','VR_GenericData_GenericRuleTypeConfigAPIService','VRUIUtilsService',
    function (UtilsService, VR_GenericData_GenericRuleTypeConfigAPIService, VRUIUtilsService) {

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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/ForLoop/Templates/ForLoopStepTemplate.html';
            }

        };

        function ForLoopStepCtor(ctrl, $scope) {
            var recordNameSelectorDirectiveReadyAPI;
            var recordNameSelectorDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            function initializeController() {
                ctrl.onRecordNameSelectorReady = function (api) {
                    recordNameSelectorDirectiveReadyAPI = api;
                    recordNameSelectorDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload.stepDetails != undefined) {
                        ctrl.iterationVariableName = payload.stepDetails.IterationVariableName;
                    }
                    var promises = [];
                    var loadRecordNameSelectorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    recordNameSelectorDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRecordName = { context: payload.context, getArray: true };
                        if (payload != undefined && payload.stepDetails != undefined)
                            payloadRecordName.selectedIds = payload.stepDetails.ArrayVariableName;
                        VRUIUtilsService.callDirectiveLoad(recordNameSelectorDirectiveReadyAPI, payloadRecordName, loadRecordNameSelectorDirectivePromiseDeferred);
                    });
                    promises.push(loadRecordNameSelectorDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);
                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.ForLoopStep, Vanrise.GenericData.Transformation.MainExtensions",
                        IterationVariableName: ctrl.iterationVariableName,
                        ArrayVariableName: ctrl.selectedArray != undefined ? ctrl.selectedArray.Name : undefined
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