'use strict';
app.directive('vrGenericdataGenericfieldsActionauditchangeRuntime', ['UtilsService','VRUIUtilsService','VR_GenericData_DataRecordFieldAPIService',
    function (UtilsService, VRUIUtilsService, VR_GenericData_DataRecordFieldAPIService) {
        return {
            restrict: 'E',
            scope: {
                onReady: '=',
                normalColNum: '@'
            },
            controller: function ($scope, $element, $attrs) {

                var ctrl = this;

                var ctor = new genericFieldsActionAuditChangeInfoRuntimeCtor(ctrl, $scope);
                ctor.initializeController();

            },
            controllerAs: 'ctrl',
            bindToController: true,
            compile: function (element, attrs) {
                return {
                    pre: function ($scope, iElem, iAttrs, ctrl) {

                    }
                };
            },
            templateUrl: function (element, attrs) {
                return '/Client/Modules/VR_GenericData/Directives/MainExtensions/ActionAuditChangeInfo/Templates/GenericFieldsActionAuditChangeInfoRuntime.html';
            }
        };

        function genericFieldsActionAuditChangeInfoRuntimeCtor(ctrl, $scope) {

            function initializeController() {
                $scope.scopeModel = {};
                $scope.scopeModel.simpleViewers = [];
                $scope.scopeModel.differenceViewers = [];

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var changeInfoDefinition;
                    var changeInfo;
                    var loggableEntityUniqueName;
                    var promises = [];

                    if (payload != undefined) {
                        loggableEntityUniqueName = payload.loggableEntityUniqueName;
                        changeInfoDefinition = payload.changeInfoDefinition;
                        changeInfo = payload.changeInfo;
                        if (loggableEntityUniqueName != undefined && changeInfo != undefined) {
                            VR_GenericData_DataRecordFieldAPIService.TryResolveDifferences({
                                LoggableEntityUniqueName: loggableEntityUniqueName,
                                FieldValues: changeInfo.FieldChanges
                            }).then(function (response) {
                                if (response != undefined) {
                                    if (response.SimpleChanges != undefined) {
                                        for (var i = 0; i < response.SimpleChanges.length ; i++) {
                                            var simpleChange = response.SimpleChanges[i];
                                            var fieldType = changeInfoDefinition.FieldTypes[simpleChange.FieldName];
                                            if (fieldType != undefined) {
                                                var fieldItem = {
                                                    payload: simpleChange,
                                                    fieldType: fieldType,
                                                    oldReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                    oldLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                    newReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                    newLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                };
                                                promises.push(fieldItem.newReadyPromiseDeferred.promise);
                                                promises.push(fieldItem.newLoadPromiseDeferred.promise);
                                                AddSimpleViewerAPI(fieldItem);
                                            }
                                        }
                                    }

                                    if (response.Differences != undefined) {
                                        for (var i = 0; i < response.Differences.length ; i++) {
                                            var difference = response.Differences[i];
                                            var fieldType = changeInfoDefinition.FieldTypes[difference.FieldName];
                                            if (fieldType != undefined) {
                                                var fieldItem = {
                                                    payload: difference,
                                                    fieldType: fieldType,
                                                    readyPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                    loadPromiseDeferred: UtilsService.createPromiseDeferred(),
                                                };
                                                promises.push(fieldItem.readyPromiseDeferred.promise);
                                                AddDifferenceViewerAPI(fieldItem);
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                    return UtilsService.waitMultiplePromises(promises);
                };
                function AddDifferenceViewerAPI(fieldItem) {
                    var viewer = {
                        fieldName: fieldItem.payload.FieldName,
                        directive: fieldItem.fieldType.Type.DifferenceEditor,
                    };
                    viewer.onDirectiveReady = function (api) {
                        viewer.directiveAPI = api;
                        fieldItem.readyPromiseDeferred.resolve();
                    };

                    fieldItem.readyPromiseDeferred.promise.then(function () {
                        var directivePayload = {
                            fieldName: fieldItem.payload.FieldName,
                            changes: fieldItem.payload.Changes,
                        };
                        VRUIUtilsService.callDirectiveLoad(viewer.directiveAPI, directivePayload, fieldItem.loadPromiseDeferred);
                    });
                    $scope.scopeModel.differenceViewers.push(viewer);
                }

                function AddSimpleViewerAPI(fieldItem)
                {
                    var viewer = {
                        fieldName: fieldItem.fieldType.Title,
                        oldDirective:fieldItem.fieldType.Type.ViewerEditor,
                        newDirective: fieldItem.fieldType.Type.ViewerEditor,
                    };

                    viewer.onOldDirectiveReady = function (api) {
                        viewer.oldDirectiveAPI = api;
                        fieldItem.oldReadyPromiseDeferred.resolve();
                    };

                    fieldItem.oldReadyPromiseDeferred.promise.then(function () {
                        var directiveOldPayload = {
                            fieldValue: fieldItem.payload.OldValue,
                            fieldValueDescription: fieldItem.payload.OldValueDescription,
                        };
                        VRUIUtilsService.callDirectiveLoad(viewer.oldDirectiveAPI, directiveOldPayload, fieldItem.oldLoadPromiseDeferred);
                    });

                    viewer.onNewDirectiveReady = function (api) {
                        viewer.newDirectiveAPI = api;
                        fieldItem.newReadyPromiseDeferred.resolve();
                    };
                    fieldItem.newReadyPromiseDeferred.promise.then(function () {
                        var directiveNewPayload = {
                            fieldValue: fieldItem.payload.NewValue,
                            fieldValueDescription: fieldItem.payload.NewValueDescription,
                        };
                        VRUIUtilsService.callDirectiveLoad(viewer.newDirectiveAPI, directiveNewPayload, fieldItem.newLoadPromiseDeferred);
                    });

                    $scope.scopeModel.simpleViewers.push(viewer);
                    
                }
                if (ctrl.onReady != undefined && typeof (ctrl.onReady) == 'function') {
                    ctrl.onReady(api);
                }
            }

            this.initializeController = initializeController;
        }
    }]);