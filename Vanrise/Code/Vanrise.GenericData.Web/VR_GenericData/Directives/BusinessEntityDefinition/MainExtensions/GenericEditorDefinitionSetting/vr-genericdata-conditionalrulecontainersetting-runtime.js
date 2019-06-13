//"use strict";

//app.directive("vrGenericdataConditionalrulecontainersettingRuntime", ["UtilsService", "VRUIUtilsService",
//    function (UtilsService, VRUIUtilsService) {

//        var directiveDefinitionObject = {
//            restrict: "E",
//            scope: {
//                onReady: "="
//            },
//            controller: function ($scope, $element, $attrs) {
//                var ctrl = this;
//                var ctor = new GenericEditorConditionalRuleRuntimeSetting($scope, ctrl, $attrs);
//                ctor.initializeController();
//            },
//            controllerAs: "ctrl",
//            bindToController: true,
//            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/Templates/ConditionalRuleContainerRuntimeSettingTemplate.html"
//        };

//        function GenericEditorConditionalRuleRuntimeSetting($scope, ctrl, $attrs) {
//            this.initializeController = initializeController;

//            var selectedValues;
//            var allFieldValuesByName;
//            var definitionSettings;
//            var dataRecordTypeId;
//            var historyId;
//            var parentFieldValues;
//            var genericContext;

//            var editorRuntimeAPI;
//            var directiveReadyPromiseDeferred = UtilsService.createPromiseDeferred();

//            function initializeController() {
//                $scope.scopeModel = {};
//                $scope.scopeModel.tabContainers = [];

//                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
//                    editorRuntimeAPI = api;
//                    directiveReadyPromiseDeferred.resolve();
//                };

//                defineAPI();
//            }

//            function defineAPI() {
//                var api = {};

//                api.load = function (payload) {
//                    var promises = [];

//                    if (payload != undefined) {
//                        selectedValues = payload.selectedValues;
//                        allFieldValuesByName = payload.allFieldValuesByName;
//                        definitionSettings = payload.definitionSettings;
//                        dataRecordTypeId = payload.dataRecordTypeId;
//                        historyId = payload.historyId;
//                        parentFieldValues = payload.parentFieldValues;
//                        genericContext = payload.genericContext;
//                    }

//                    if (definitionSettings != undefined) {
//                        if (definitionSettings.TabContainers != undefined) {
//                            for (var i = 0; i < definitionSettings.TabContainers.length; i++) {
//                                var tabContainer = definitionSettings.TabContainers[i];
//                                var tabContainerDef = {
//                                    payload: tabContainer,
//                                    directiveReadyPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                    directiveLoadPromiseDeferred: UtilsService.createPromiseDeferred(),
//                                };
//                                addTabContainer(tabContainerDef);
//                                promises.push(tabContainerDef.directiveLoadPromiseDeferred.promise);
//                            }
//                        }
//                    }

//                    function loadEditorDirective() {
//                        var directiveLoadPromiseDeferred = UtilsService.createPromiseDeferred();
//                        directiveReadyPromiseDeferred.promise.then(function () {
//                            var directivePayload = {
//                                dataRecordTypeId: dataRecordTypeId,
//                                definitionSettings: ,
//                                selectedValues: selectedValues,
//                                allFieldValuesByName: allFieldValuesByName,
//                                historyId: historyId,
//                                parentFieldValues: parentFieldValues,
//                                genericContext: genericContext
//                            };
//                            VRUIUtilsService.callDirectiveLoad(editorRuntimeAPI, directivePayload, directiveLoadPromiseDeferred);
//                        });
//                        return directiveLoadPromiseDeferred.promise;
//                    }

//                    return UtilsService.waitMultiplePromises(promises);
//                };

//                api.setData = function (dicData) {
//                    if (editorRuntimeAPI != undefined) {
//                        editorRuntimeAPI.setData(dicData);
//                    }
//                };

//                api.onFieldValueChanged = function (allFieldValuesByFieldNames) {
//                    if (editorRuntimeAPI == undefined)
//                        return null;

//                    var _promises = [];

//                    if (editorRuntimeAPI.onFieldValueChanged != undefined && typeof (editorRuntimeAPI.onFieldValueChanged) == "function") {
//                        var onFieldValueChangedPromise = editorRuntimeAPI.onFieldValueChanged(allFieldValuesByFieldNames);
//                        if (onFieldValueChangedPromise != undefined)
//                            _promises.push(onFieldValueChangedPromise);
//                    }

//                    return UtilsService.waitMultiplePromises(_promises);
//                };

//                api.setFieldValues = function (fieldValuesByNames) {
//                    if (editorRuntimeAPI == undefined)
//                        return null;

//                    var _promises = [];

//                    if (editorRuntimeAPI.setFieldValues != undefined && typeof (editorRuntimeAPI.setFieldValues) == "function") {
//                        var onFieldValueSettedPromise = editorRuntimeAPI.setFieldValues(fieldValuesByNames);
//                        if (onFieldValueSettedPromise != undefined)
//                            _promises.push(onFieldValueSettedPromise);
//                    }

//                    return UtilsService.waitMultiplePromises(_promises);
//                };

//                if (ctrl.onReady != null && typeof (ctrl.onReady) == "function")
//                    ctrl.onReady(api);
//            }
//        }

//        return directiveDefinitionObject;
//    }
//]);