'use strict';
app.directive('vrGenericdataDatatransformationAdditemtoarraystep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new AddItemToArrayStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/AddItemToArray/Templates/AddItemToArrayStepTemplate.html';
            }

        };

        function AddItemToArrayStepCtor(ctrl, $scope) {

            var variableDirectiveReadyAPI;
            var variableDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var recordNameSelectorDirectiveReadyAPI;
            var recordNameSelectorDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                ctrl.onVariableReady = function (api) {
                    variableDirectiveReadyAPI = api;
                    variableDirectiveReadyPromiseDeferred.resolve();
                };
                ctrl.onRecordNameSelectorReady = function (api) {
                    recordNameSelectorDirectiveReadyAPI = api;
                    recordNameSelectorDirectiveReadyPromiseDeferred.resolve();
                };
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined)
                    {
                        if(payload.context != undefined)
                            ctrl.records = payload.context.getArrayRecordNames();
                        if (payload.stepDetails != undefined)
                            ctrl.selectedRecordName = UtilsService.getItemByVal(ctrl.records, payload.stepDetails.RecordName, "Name");
                    }
                    var promises = [];

                    var loadVariableDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    variableDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadVariable = { context: payload.context };
                        if (payload != undefined &&  payload.stepDetails != undefined)
                            payloadVariable.selectedRecords = payload.stepDetails.VariableName;
                        VRUIUtilsService.callDirectiveLoad(variableDirectiveReadyAPI, payloadVariable, loadVariableDirectivePromiseDeferred);
                    });
                    promises.push(loadVariableDirectivePromiseDeferred.promise);

                    var loadRecordNameSelectorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                    recordNameSelectorDirectiveReadyPromiseDeferred.promise.then(function () {
                        var payloadRecordName = { context: payload.context,getArray:true };
                        if (payload != undefined && payload.stepDetails != undefined)
                            payloadRecordName.selectedIds = payload.stepDetails.ArrayVariableName;
                        VRUIUtilsService.callDirectiveLoad(recordNameSelectorDirectiveReadyAPI, payloadRecordName, loadRecordNameSelectorDirectivePromiseDeferred);
                    });
                    promises.push(loadRecordNameSelectorDirectivePromiseDeferred.promise);
                    return UtilsService.waitMultiplePromises(promises);

                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AddItemToArrayStep, Vanrise.GenericData.Transformation.MainExtensions",
                        ArrayVariableName: ctrl.selectedRecordName != undefined ? ctrl.selectedRecordName.Name : undefined,
                        VariableName: variableDirectiveReadyAPI != undefined ? variableDirectiveReadyAPI.getData() : undefined,

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