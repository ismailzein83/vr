"use strict";

app.directive("vrGenericdataStaticeditorRuntime", ["UtilsService", "VRUIUtilsService",
    function (UtilsService, VRUIUtilsService) {

        var directiveDefinitionObject = {
            restrict: "E",
            scope: {
                onReady: "="
            },
            controller: function ($scope, $element, $attrs) {
                var ctrl = this;
                var ctor = new StaticEditorRuntimeSetting($scope, ctrl, $attrs);
                ctor.initializeController();
            },
            controllerAs: "ctrl",
            bindToController: true,
            templateUrl: "/Client/Modules/VR_GenericData/Directives/BusinessEntityDefinition/MainExtensions/GenericEditorDefinitionSetting/StaticEditor/Templates/StaticEditorRuntimeSettingTemplate.html"
        };

        function StaticEditorRuntimeSetting($scope, ctrl, $attrs) {
            this.initializeController = initializeController;

            var selectedValues;
            var allFieldValuesByName;
            var definitionSettings;
            var dataRecordTypeId;
            var historyId;
            var parentFieldValues;
            var genericContext;

            var editorRuntimeDirectiveAPI;
            var editorRuntimeDirectivePromiseDeferred = UtilsService.createPromiseDeferred();


            function initializeController() {
                $scope.scopeModel = {};

                $scope.scopeModel.onEditorRuntimeDirectiveReady = function (api) {
                    editorRuntimeDirectiveAPI = api;
                    editorRuntimeDirectivePromiseDeferred.resolve();
                };

                defineAPI();
            }

            function defineAPI() {
                var api = {};

                api.load = function (payload) {
                    var promises = [];

                    if (payload != undefined) {
                        selectedValues = payload.selectedValues;
                        allFieldValuesByName = payload.allFieldValuesByName;
                        definitionSettings = payload.definitionSettings;
                        dataRecordTypeId = payload.dataRecordTypeId;
                        historyId = payload.historyId;
                        parentFieldValues = payload.parentFieldValues;
                        genericContext = payload.genericContext;

                        if (definitionSettings != undefined) {
                            $scope.scopeModel.runtimeEditor = definitionSettings.DirectiveName;
                        }
                    }

                    var loadStaticEditorDirectivePromise = loadStaticEditorDirective();
                    promises.push(loadStaticEditorDirectivePromise);

                    function loadStaticEditorDirective() {
                        var editorRuntimeDirectiveLoadDeferred = UtilsService.createPromiseDeferred();

                        editorRuntimeDirectivePromiseDeferred.promise.then(function () {
                            var directivePayload = {
                                definitionSettings: definitionSettings,
                                selectedValues: selectedValues,
                                allFieldValuesByName: allFieldValuesByName,
                                dataRecordTypeId: dataRecordTypeId,
                                historyId: historyId,
                                parentFieldValues: parentFieldValues,
                                genericContext: genericContext
                            };
                            VRUIUtilsService.callDirectiveLoad(editorRuntimeDirectiveAPI, directivePayload, editorRuntimeDirectiveLoadDeferred);
                        });

                        return editorRuntimeDirectiveLoadDeferred.promise;
                    }

                    return UtilsService.waitMultiplePromises(promises);
                };

                api.setData = function (dicData) {
                    editorRuntimeDirectiveAPI.setData(dicData);
                };

                api.onFieldValueChanged = function (allFieldValuesByFieldNames, changedField) {
                    var _promises = [];

                    if (editorRuntimeDirectiveAPI != undefined && editorRuntimeDirectiveAPI.onFieldValueChanged != undefined && typeof (editorRuntimeDirectiveAPI.onFieldValueChanged) == "function") {
                        var onFieldValueChangedPromise = editorRuntimeDirectiveAPI.onFieldValueChanged(allFieldValuesByFieldNames, changedField);
                        if (onFieldValueChangedPromise != undefined)
                            _promises.push(onFieldValueChangedPromise));
                    }

                    return UtilsService.waitMultiplePromises(_promises);
                };

                //api.setFieldValues = function (fieldValuesByNames) {
                //    var _promises = [];

                //    if (editorRuntimeDirectiveAPI.setFieldValues != undefined && typeof (editorRuntimeDirectiveAPI.setFieldValues) == "function") {
                //        var onFieldValueSettedPromise = editorRuntimeDirectiveAPI.setFieldValues(fieldValuesByNames);
                //        if (onFieldValueSettedPromise != undefined)
                //            _promises.push(onFieldValueSettedPromise);
                //    }

                //    return UtilsService.waitMultiplePromises(_promises);
                //};

                if (ctrl.onReady != null)
                    ctrl.onReady(api);
            }
        }

        return directiveDefinitionObject;
    }
]);