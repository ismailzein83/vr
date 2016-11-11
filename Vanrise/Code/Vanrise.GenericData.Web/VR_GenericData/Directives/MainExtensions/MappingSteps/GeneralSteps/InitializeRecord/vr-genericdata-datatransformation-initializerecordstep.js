'use strict';
app.directive('vrGenericdataDatatransformationInitializerecordstep', ['UtilsService', 'VRUIUtilsService',
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: 'E',
            scope:
            {
                onReady: '='
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new InitializeRecordStepCtor(ctrl, $scope);
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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/GeneralSteps/InitializeRecord/Templates/InitializeRecordStepTemplate.html';
            }

        };

        function InitializeRecordStepCtor(ctrl, $scope) {
            var recordNameSelectorDirectiveReadyAPI;
            var recordNameSelectorDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();
            function initializeController() {
                ctrl.onRecordNameSelectorReady = function (api) {
                    recordNameSelectorDirectiveReadyAPI = api;
                    recordNameSelectorDirectiveReadyPromiseDeferred.resolve();
                }
                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    if (payload != undefined) {
                        var promises = [];

                        var loadRecordNameSelectorDirectivePromiseDeferred = UtilsService.createPromiseDeferred();

                        recordNameSelectorDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadRecordName = { context: payload.context, getArray: true, getNonArray: true };
                            if (payload != undefined && payload.stepDetails != undefined)
                                payloadRecordName.selectedIds = payload.stepDetails.RecordName;
                            VRUIUtilsService.callDirectiveLoad(recordNameSelectorDirectiveReadyAPI, payloadRecordName, loadRecordNameSelectorDirectivePromiseDeferred);
                        });
                        promises.push(loadRecordNameSelectorDirectivePromiseDeferred.promise);
                        return UtilsService.waitMultiplePromises(promises);
                    }


                };

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.InitializeRecordStep, Vanrise.GenericData.Transformation.MainExtensions",
                        RecordName: recordNameSelectorDirectiveReadyAPI != undefined ? recordNameSelectorDirectiveReadyAPI.getSelectedIds() : undefined,

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