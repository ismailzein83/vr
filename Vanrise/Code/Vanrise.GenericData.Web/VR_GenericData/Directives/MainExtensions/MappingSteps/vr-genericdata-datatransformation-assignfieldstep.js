'use strict';
app.directive('vrGenericdataDatatransformationAssignfieldstep', ['UtilsService',
    function (UtilsService) {

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
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/MappingSteps/Templates/AssignFieldStepTemplate.html';
            }

        };

        function AssignFieldStepCtor(ctrl, $scope) {
            var sourceRecordNameDirectiveReadyAPI;
            var sourceRecordNameDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var sourceFieldNameDirectiveReadyAPI;
            var sourceFieldNameDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var targetRecordNameDirectiveReadyAPI;
            var targetRecordNameDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

            var targetFieldNameDirectiveReadyAPI;
            var targetFieldNameDirectiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();


            var mainPayload;
            function initializeController() {
                $scope.onSourceRecordNameReady = function (api) {
                    sourceRecordNameDirectiveReadyAPI = api;
                    sourceRecordNameDirectiveReadyPromiseDeferred.resolve();
                }
                $scope.onSourceFieldNameReady = function (api) {
                    sourceFieldNameDirectiveReadyAPI = api;
                    var setLoader = function (value) { $scope.isLoadingSourceFieldNameDirective = value };
                    var payload = { DataSource: mainPayload.getFieldsByRecordName($scope.selectedSourceRecordName) }
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, sourceFieldNameDirectiveReadyAPI, payload, setLoader, sourceFieldNameDirectiveReadyPromiseDeferred);
                }
                $scope.onTargetRecordName = function (api) {
                    targetRecordNameDirectiveReadyAPI = api;
                    targetRecordNameDirectiveReadyPromiseDeferred.resolve();
                }

                $scope.onTargetFieldNameReady = function (api) {
                    targetFieldNameDirectiveReadyAPI = api;
                    var setLoader = function (value) { $scope.isLoadingTargetFieldNameDirective = value };
                    var payload = { DataSource: mainPayload.getFieldsByRecordName($scope.selectedSourceRecordName) }
                    VRUIUtilsService.callDirectiveLoadOrResolvePromise($scope, targetFieldNameDirectiveReadyAPI, payload, setLoader, targetFieldNameDirectiveReadyPromiseDeferred);
                }

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    mainPayload = payload;
                    var promises = [];

                        var loadTargetRecordNameDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                      
                        targetRecordNameDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadTargetRecordName={DataSource:payload.RecordTypes};
                            VRUIUtilsService.callDirectiveLoad(targetRecordNameDirectiveReadyAPI, payloadTargetRecordName, loadTargetRecordNameDirectivePromiseDeferred);
                        });
                        promises.push(loadTargetRecordNameDirectivePromiseDeferred.promise);

                        var loadSourceRecordNameDirectivePromiseDeferred = UtilsService.createPromiseDeferred();
                      
                        sourceRecordNameDirectiveReadyPromiseDeferred.promise.then(function () {
                            var payloadSourceRecordName={DataSource:payload.RecordTypes};
                            VRUIUtilsService.callDirectiveLoad(sourceRecordNameDirectiveReadyAPI, payloadSourceRecordName, loadSourceRecordNameDirectivePromiseDeferred);
                        });
                        promises.push(loadSourceRecordNameDirectivePromiseDeferred.promise);

                    return UtilsService.waitMultiplePromises(promises);

                }

                api.getData = function () {
                    return {
                        $type: "Vanrise.GenericData.Transformation.MainExtensions.MappingSteps.AssignFieldStep, Vanrise.GenericData.Transformation.MainExtensions",
                        SourceRecordName: $scope.selectedSourceRecordName != undefined ? $scope.selectedSourceRecordName.Name : undefined,
                        SourceFieldName: $scope.selectedSourceFieldName != undefined ? $scope.selectedSourceFieldName.Name : undefined,
                        TargetRecordName: $scope.selectedTargetRecordName != undefined ? $scope.selectedTargetRecordName.Name : undefined,
                        TargetFieldName: $scope.selectedTargetFieldName != undefined ? $scope.selectedTargetFieldName.Name : undefined,
                    };
                }

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }

            this.initializeController = initializeController;
        }
        return directiveDefinitionObject;
    }
]);