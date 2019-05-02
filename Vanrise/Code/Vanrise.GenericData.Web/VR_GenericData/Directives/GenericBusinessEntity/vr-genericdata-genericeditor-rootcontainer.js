//"use strict";

//app.directive("vrGenericdataGenericeditorRootcontainer", ["UtilsService", "VRNotificationService", "VRUIUtilsService", 'VR_GenericData_GenericBEDefinitionAPIService', 'VR_GenericData_GenericBusinessEntityAPIService',
//    function (UtilsService, VRNotificationService, VRUIUtilsService, VR_GenericData_GenericBEDefinitionAPIService, VR_GenericData_GenericBusinessEntityAPIService) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope:
//            {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new GenericBusinessEntityRootEditorCtor($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            compile: function (element, attrs) {

//            },
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/GenericBusinessEntity/Templates/GenericBusinessEntityRootContainerEditor.html"
//        };
//        function GenericBusinessEntityRootEditorCtor($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var selectedValues;
//            var runtimeEditorAPI;
//            var runtimeEditorReadyDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};

//                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
//                    runtimeEditorAPI = api;
//                    runtimeEditorReadyDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];

//                    var dataRecordTypeId;
//                    var definitionSettings;
//                    var historyId;
//                    var parentFieldValues;

//                    if (payload != undefined) {
//                        selectedValues = payload.selectedValues;
//                        dataRecordTypeId = payload.dataRecordTypeId;
//                        definitionSettings = payload.definitionSettings;
//                        historyId = payload.historyId;
//                        parentFieldValues = payload.parentFieldValues;
//                        $scope.scopeModel.runtimeEditor = payload.runtimeEditor;
//                    }

//                    var loadEditorRuntimeDirectivePromise = getLoadEditorRuntimeDirective();
//                    promises.push(loadEditorRuntimeDirectivePromise);

//                    function getLoadEditorRuntimeDirective() {
//                        var runtimeEditorLoadDeferred = UtilsService.createPromiseDeferred();
//                        runtimeEditorReadyDeferred.promise.then(function () {
//                            var runtimeEditorPayload = {
//                                selectedValues: selectedValues,
//                                dataRecordTypeId: dataRecordTypeId,
//                                definitionSettings: definitionSettings,
//                                historyId: historyId,
//                                parentFieldValues: parentFieldValues
//                            };

//                            var context = {
//                                notifyFieldChanged: function (changedField) { // changedField = {fieldName : 'name', fieldValue : 'value' }
//                                    var _promises = [];
//                                    if (runtimeEditorAPI.onFieldValueChanged != undefined && typeof (runtimeEditorAPI.onFieldValueChanged) == "function") {
//                                        var promise = runtimeEditorAPI.onFieldValueChanged(changedField);
//                                        if (promise != undefined)
//                                            _promises.push(promise);
//                                    }

//                                    return UtilsService.waitMultiplePromises(_promises);
//                                },
//                                getFieldValues: function (dicFieldValuesByNames) {
//                                    runtimeEditorAPI.setData(dicFieldValuesByNames);
//                                    return dicFieldValuesByNames;
//                                },
//                                setFieldValues: function (dicFieldValuesByNames) {
//                                    var _promises = [];
//                                    var handledDicFieldValuesByNames = {};
//                                    if (selectedValues != undefined) {
//                                        for (var prop in dicFieldValuesByNames) {
//                                            if (selectedValues[prop] == undefined)
//                                                handledDicFieldValuesByNames[prop] = dicFieldValuesByNames[prop];
//                                        }
//                                    }
//                                    else {
//                                        handledDicFieldValuesByNames = dicFieldValuesByNames;
//                                    }

//                                    if (Object.keys(handledDicFieldValuesByNames).length > 0) {
//                                        var onFieldValueSettedPromise = runtimeEditorAPI.setFieldValues(dicFieldValuesByNames);
//                                        selectedValues = undefined;
//                                        if (onFieldValueSettedPromise != undefined)
//                                            _promises.push(onFieldValueSettedPromise);
//                                    }

//                                    return UtilsService.waitMultiplePromises(_promises);
//                                }
//                            };

//                            runtimeEditorPayload.genericContext = context;

//                            VRUIUtilsService.callDirectiveLoad(runtimeEditorAPI, runtimeEditorPayload, runtimeEditorLoadDeferred);
//                        });

//                        return runtimeEditorLoadDeferred.promise;
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.getData = function () {
//                    return runtimeEditorAPI.getData();
//                };

//                api.setData = function (dicData) {
//                    runtimeEditorAPI.setData(dicData);
//                };

//                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
//                    ctrl.onReady(api);
//            }
//        }

//        return directiveDefinitionObject;
//    }
//]);